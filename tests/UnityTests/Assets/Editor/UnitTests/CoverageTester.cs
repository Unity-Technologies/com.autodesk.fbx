// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
//
// This script depends on the Mono.Reflection.MethodBodyReader by the JB Evain,
// see https://github.com/jbevain/mono.reflection (it's under an MIT license).
// ***********************************************************************
#if ENABLE_COVERAGE_TEST

using System.Collections.Generic;
using System.Reflection;

static class CoverageTester
{
    static Dictionary<MethodInfo, HashSet<MethodInfo>> s_calls = new Dictionary<MethodInfo, HashSet<MethodInfo>>();
    static Dictionary<MethodInfo, HashSet<MethodInfo>> s_reflectionCalls = new Dictionary<MethodInfo, HashSet<MethodInfo>>();

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
    public static HashSet<MethodInfo> GetCalls(MethodInfo method)
    {
        if(s_calls.ContainsKey(method)) {
            return s_calls[method];
        }

        // Look at the current method and in DFS, find all the methods it calls.
        var stack = new List<MethodInfo>();
        var visited = new HashSet<MethodInfo>();
        stack.Add(method);
        while(stack.Count > 0) {
            var top = stack[stack.Count - 1];
            stack.RemoveAt(stack.Count - 1);

            if (visited.Contains(top)) {
                continue;
            }
            visited.Add(top);

            // If we have already seen this method, we can copy all its calls in and stop
            // our search here.
            if (s_calls.ContainsKey(top)) {
                visited.UnionWith(s_calls[top]);
                continue;
            }


            List<Mono.Reflection.Instruction> instructions;
            try {
                instructions = Mono.Reflection.MethodBodyReader.GetInstructions(top);
            } catch (System.ArgumentException xcp) {
                // ignore the method having no body
                continue;
            }
            foreach (var instruction in instructions) {
                // look at the instructions of this method; see if they are a function call.
                // if so, recursively look into that function
                var calledMethod = instruction.Operand as MethodInfo;
                if (calledMethod == null) { continue; }

                // It's a function call, so recursively look into it.
                // TODO: we should recursively call GetCalls on each method
                // we call here, but in a way that doesn't hit infinite loops.
                // Then we'd hit in cache more often.
                stack.Add(calledMethod);
            }

            // Also add in the calls that have been registered to be made.
            HashSet<MethodInfo> reflectionCalls;
            if (s_reflectionCalls.TryGetValue(top, out reflectionCalls)) {
                stack.AddRange(reflectionCalls);
            }
        }

        // Every function we visited is a function that this function calls!
        s_calls[method] = visited;
        return visited;
    }

    /// <summary>
    /// Clear the cache; useful if you just registered a new reflection call.
    /// </summary>
    public static void ClearCache()
    {
        s_calls = new Dictionary<MethodInfo, HashSet<MethodInfo>>();
    }

    /// <summary>
    /// If you're using reflection to call methods, they won't be picked
    /// up automatically. This lets you add that information.
    ///
    /// You might want to clear the cache afterwards.
    /// </summary>
    public static void RegisterReflectionCall(MethodInfo from, MethodInfo to)
    {
        HashSet<MethodInfo> calls;
        if (s_reflectionCalls.TryGetValue(from, out calls)) {
            calls.Add(to);
        } else {
            HashSet<MethodInfo> tos = new HashSet<MethodInfo>();
            tos.Add(to);
            s_reflectionCalls[from] = tos;
        }
    }

    /// <summary>
    /// Statically analyze the root methods, and check whether their static
    /// call graph might cover all the methods to cover.
    ///
    /// Every function in 'methods to cover' that *might* get called will
    /// be added to the 'hit' output. Functions that *definitely* won't be
    /// called get added to the 'missed' output.
    ///
    /// "Definite" fails in the face of reflection; use RegisterReflectionCall
    /// to handle that.
    ///
    /// "Might" includes virtual function dispatch. If there's a call to a base
    /// method, we say that call covers any derived method.
    ///
    /// The static analysis is very simplistic: we don't fold constants or
    /// eliminate dead code or devirtualize calls.
    /// </summary>
    /// <returns><c>true</c>, if coverage was tested, <c>false</c> otherwise.</returns>
    /// <param name="MethodsToCover">Methods to cover.</param>
    /// <param name="RootMethods">Root methods.</param>
    /// <param name="out_HitMethods">Out hit methods.</param>
    /// <param name="out_MissedMethods">Out missed methods.</param>
    /// <param name="allowVirtual">If set to <c>true</c> allow virtual.</param>
    public static bool TestCoverage(IEnumerable<MethodInfo> MethodsToCover,
            IEnumerable<MethodInfo> RootMethods,
            out HashSet<MethodInfo> out_HitMethods,
            out HashSet<MethodInfo> out_MissedMethods
            )
    {
        var calledMethods = new HashSet<MethodInfo>();
        foreach(var rootMethod in RootMethods) {
            calledMethods.UnionWith(GetCalls(rootMethod));
        }

        out_MissedMethods = new HashSet<MethodInfo>();
        out_HitMethods = new HashSet<MethodInfo>();
        foreach(var methodToCover in MethodsToCover) {
            // Did we call the method?
            if (calledMethods.Contains(methodToCover)) {
                out_HitMethods.Add(methodToCover);
                continue;
            }

            // Did we call a base class declaration of the method?
            // If so, we might call the method we're looking for through
            // virtual function dispatch.
            var didHit = false;
            var baseBaseMethod = methodToCover.GetBaseDefinition();
            if(baseBaseMethod != methodToCover) {
                if (calledMethods.Contains(methodToCover.GetBaseDefinition())) {
                    out_HitMethods.Add(methodToCover);
                    didHit = true;
                } else {
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
                    MethodInfo baseMethod;
                    do {
                        baseClass = baseClass.BaseType;
                        baseMethod = baseClass.GetMethod(methodToCover.Name, parameterTypes);
                        if (calledMethods.Contains(baseMethod)) {
                            out_HitMethods.Add(methodToCover);
                            didHit = true;
                            break;
                        }
                    } while (baseMethod != baseBaseMethod);
                }
            }
            if (!didHit) {
                out_MissedMethods.Add(methodToCover);
            }
        }

        if (out_MissedMethods.Count == 0) {
            return true;
        } else {
            return false;
        }
    }

    public static string GetMethodSignature(MethodInfo info)
    {
        var builder = new System.Text.StringBuilder();
        builder.Append(info.ReturnType.Name);
        builder.Append(' ');
        builder.Append(info.DeclaringType.Name);
        builder.Append('.');
        builder.Append(info.Name);
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
            HashSet<MethodInfo> HitMethods,
            HashSet<MethodInfo> MissedMethods)
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
