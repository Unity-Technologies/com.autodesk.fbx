// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
//
// This script depends on the Mono.Reflection.Disassembler by the JB Evain,
// see https://github.com/jbevain/mono.reflection (it's under an MIT license).
// ***********************************************************************
#if ENABLE_COVERAGE_TEST

using System.Collections.Generic;
using System.Reflection;

static class CoverageTester
{
    // Important fact: MethodBase doesn't implement equality like you'd expect.
    // So we need to use RuntimeMethodHandle for keys.
    static Dictionary<System.RuntimeMethodHandle, List<MethodBase>> s_calls = new Dictionary<System.RuntimeMethodHandle, List<MethodBase>>();
    static Dictionary<System.RuntimeMethodHandle, List<MethodBase>> s_reflectionCalls = new Dictionary<System.RuntimeMethodHandle, List<MethodBase>>();

    static void PreloadCache()
    {
        if (s_calls.Count != 0) { return; }

        var types = new System.Type[] {
                typeof(object),
                typeof(string),
                typeof(int),
                typeof(System.Array),
                typeof(System.Type),
                typeof(System.Text.StringBuilder),
                typeof(CoverageTester),
                typeof(NUnit.Framework.Assert)
        };
        foreach(var typ in types) {
            foreach(var method in typ.GetMethods()) {
                GetCalls(method);
            }
        }
    }

    /// <summary>
    /// Dig through the instructions for 'method' and see which methods it
    /// calls, and what methods those methods call in turn recursively.
    ///
    /// This is cached, so the first call for any given function will be
    /// expensive but subsequent ones will be cheap.
    ///
    /// This is very weak analysis: we do zero devirtualization, dead code
    /// elimination, or constant propagation, or anything.
    /// </summary>
    public static List<MethodBase> GetCalls(MethodBase method)
    {
        if (s_calls.ContainsKey(method.MethodHandle)) {
            return s_calls[method.MethodHandle];
        }

        // Look at the current method and in DFS, find all the methods it calls.
        var stack = new List<MethodBase>();
        var calls = new List<MethodBase>();
        var visited = new HashSet<System.RuntimeMethodHandle>();
        stack.Add(method);
        while(stack.Count > 0) {
            var top = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            if (visited.Contains(top.MethodHandle)) {
                continue;
            }
            visited.Add(top.MethodHandle);
            calls.Add(top);

            // If we have already seen this method, we can copy all its calls in and stop
            // our search here.
            if (s_calls.ContainsKey(top.MethodHandle)) {
                foreach(var calledMethod in s_calls[top.MethodHandle]) {
                    visited.Add(calledMethod.MethodHandle);
                    calls.Add(calledMethod);
                }
                continue;
            }

            IEnumerable<Mono.Reflection.Instruction> instructions;
            try {
                instructions = Mono.Reflection.Disassembler.GetInstructions(top);
            } catch (System.ArgumentException) {
                // ignore the method having no body
                continue;
            }
            foreach (var instruction in instructions) {
                // look at the instructions of this method; see if they are a function call.
                // if so, recursively look into that function
                var calledMethod = instruction.Operand as MethodBase;
                if (calledMethod == null) { continue; }

                // It's a function call, so recursively look into it.
                // TODO: we should recursively call GetCalls on each method
                // we call here, but in a way that doesn't hit infinite loops.
                // Then we'd hit in cache more often.
                stack.Add(calledMethod);
            }

            // Also add in the calls that have been registered to be made.
            List<MethodBase> reflectionCalls;
            if (s_reflectionCalls.TryGetValue(top.MethodHandle, out reflectionCalls)) {
                stack.AddRange(reflectionCalls);
            }
        }

        // Store the result and return it.
        s_calls[method.MethodHandle] = calls;
        return calls;
    }

    /// <summary>
    /// Clear the cache; useful if you just registered a new reflection call.
    /// </summary>
    public static void ClearCache()
    {
        s_calls = new Dictionary<System.RuntimeMethodHandle, List<MethodBase>>();
    }

    /// <summary>
    /// If you're using reflection to call methods, they won't be picked
    /// up automatically. This lets you add that information.
    ///
    /// You might want to clear the cache afterwards.
    /// </summary>
    public static void RegisterReflectionCall(MethodBase from, MethodBase to)
    {
        List<MethodBase> calls;
        if (s_reflectionCalls.TryGetValue(from.MethodHandle, out calls)) {
            calls.Add(to);
        } else {
            List<MethodBase> tos = new List<MethodBase>();
            tos.Add(to);
            s_reflectionCalls[from.MethodHandle] = tos;
        }
    }

    /// <summary>
    /// Helper for TestCoverage, determines whether we called a function
    /// we're looking for -- just via a base class.
    /// </summary>
    private static bool DidCallBaseMethod(MethodInfo methodToCover,
        HashSet<System.RuntimeMethodHandle> calledMethods)
    {
        if ((object)methodToCover == null) {
            // mTC is a constructor, which means it can't have been called via
            // virtual function dispatch.
            return false;
        }

        var baseBaseMethod = methodToCover.GetBaseDefinition();
        if(baseBaseMethod.MethodHandle == methodToCover.MethodHandle) {
            // mTC is the base definition, which means it can't have been called
            // via virtual function dispatch.
            return false;
        }

        if (calledMethods.Contains(baseBaseMethod.MethodHandle)) {
            // mTC's base method got called, so it might have been called via
            // virtual function dispatch.
            return true;
        }

        // The base-base method is at the top of the hierarchy. We might have
        // called something in the middle of the hierarchy, and neither test above
        // will have noticed.
        //
        // Look up the parent class. Find the method with the same name and types.
        // Repeat.
        var parameters = methodToCover.GetParameters();
        var parameterTypes = new System.Type[parameters.Length];
        for(int i = 0; i < parameters.Length; ++i) {
            parameterTypes[i] = parameters[i].ParameterType;
        }
        System.Type baseClass = methodToCover.DeclaringType;
        MethodBase baseMethod;
        do {
            baseClass = baseClass.BaseType;
            baseMethod = baseClass.GetMethod(methodToCover.Name, parameterTypes);
            if (calledMethods.Contains(baseMethod.MethodHandle)) {
                return true;
            }
        } while (baseMethod != baseBaseMethod);

        return false;
    }

    /// <summary>
    /// Statically analyze the root methods, and check whether their static
    /// call graph might cover all the methods to cover.
    ///
    /// Every function in 'methods to cover' that *might* get called will
    /// be added to the 'hit' output. Functions that *definitely* won't be
    /// called get added to the 'missed' output.
    ///
    /// "Definite" fails in the face of reflection and virtual functions.
    /// Use RegisterReflectionCall to handle reflection.
    ///
    /// For virtuals, we rely on the tests being simple (not calling virtualized
    /// hierarchies of test frameworks). If there's a call to a base method,
    /// we say that call covers any derived method.
    ///
    /// The static analysis is very simplistic: we don't fold constants or
    /// eliminate dead code or devirtualize calls.
    /// </summary>
    public static bool TestCoverage(IEnumerable<MethodBase> MethodsToCover,
            IEnumerable<MethodBase> RootMethods,
            out List<MethodBase> out_HitMethods,
            out List<MethodBase> out_MissedMethods
            )
    {
        PreloadCache();

        // Collect up the handles we called.
        var calledMethods = new HashSet<System.RuntimeMethodHandle>();
        foreach(var rootMethod in RootMethods) {
            foreach(var called in GetCalls(rootMethod)) {
                calledMethods.Add(called.MethodHandle);
            }
        }

        out_MissedMethods = new List<MethodBase>();
        out_HitMethods = new List<MethodBase>();
        foreach(var methodToCover in MethodsToCover) {
            // Did we call the method?
            if (calledMethods.Contains(methodToCover.MethodHandle)) {
                out_HitMethods.Add(methodToCover);
                continue;
            }

            // Did we call a base class declaration of the method?
            // If so, we might call the method we're looking for through
            // virtual function dispatch.
            if (DidCallBaseMethod(methodToCover as MethodInfo, calledMethods)) {
                out_HitMethods.Add(methodToCover);
                continue;
            }

            // No other excuses? We must have missed it.
            out_MissedMethods.Add(methodToCover);
        }

        if (out_MissedMethods.Count == 0) {
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Simple interface for running an NUnit test.
    /// <code>
    ///    [Test]
    ///    public void TestCoverage() { CoverageTester.TestCoverage(typeof(ThingWeAreTesting), this.GetType()); }
    /// </code>
    /// </summary>
    public static void TestCoverage(System.Type TypeToCover, System.Type NUnitTestFramework)
    {
        // Make sure the EqualityTester for the type we should cover has been initialized.
        var eqTester = typeof(UnitTests.EqualityTester<>).MakeGenericType(TypeToCover);
        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(eqTester.TypeHandle);

        // We want to call all the methods of the proxy, including all the constructors.
        var methodsToCover = new List<MethodBase>(TypeToCover.GetMethods());
        methodsToCover.AddRange(TypeToCover.GetConstructors());

        // Our public test functions are what we can use to call that with.
        var testMethods = new List<MethodBase>();
        foreach(var method in NUnitTestFramework.GetMethods()) {
            // Check that the method is tagged [Test]
            if (method.GetCustomAttributes(typeof(NUnit.Framework.TestAttribute), true).Length > 0) {
                testMethods.Add(method);
            }
        }

        List<MethodBase> hitMethods;
        List<MethodBase> missedMethods;

        var coverageComplete = CoverageTester.TestCoverage(methodsToCover, testMethods, out hitMethods, out missedMethods);

        NUnit.Framework.Assert.That(
                () => coverageComplete,
                () => CoverageTester.MakeCoverageMessage(hitMethods, missedMethods));
    }

    public static string GetMethodSignature(MethodBase info)
    {
        var builder = new System.Text.StringBuilder();
        if (info.IsConstructor) {
            builder.Append(info.DeclaringType.Name);
        } else {
            var method = info as MethodInfo;
            if (method != null) {
                builder.Append(method.ReturnType.Name);
                builder.Append(' ');
            }
            if (info.DeclaringType != null) {
                builder.Append(info.DeclaringType.Name);
                builder.Append('.');
            }
            builder.Append(info.Name);
        }
        builder.Append('(');
        var args = info.GetParameters();
        if (args.Length > 0) {
            builder.Append(args[0].ParameterType.Name);
        }
        for(var i = 1; i < args.Length; ++i) {
            builder.Append(", ");
            builder.Append(args[i].ParameterType.Name);
        }
        builder.Append(')');
        return builder.ToString();
    }

    public static string MakeCoverageMessage(
            IEnumerable<MethodBase> HitMethods,
            IEnumerable<MethodBase> MissedMethods)
    {
        var missed = new List<string>();
        var hit = new List<string>();
        foreach(var h in HitMethods) { hit.Add(GetMethodSignature(h)); }
        foreach(var m in MissedMethods) { missed.Add(GetMethodSignature(m)); }
        missed.Sort();
        hit.Sort();
        return string.Format("Failed to call:\n\t{0}\nSucceeded to call:\n\t{1}",
                string.Join("\n\t", missed.ToArray()),
                string.Join("\n\t", hit.ToArray()));
    }
}
#endif
