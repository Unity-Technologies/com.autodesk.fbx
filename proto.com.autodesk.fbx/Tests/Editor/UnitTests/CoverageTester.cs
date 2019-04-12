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

namespace Autodesk.Fbx
{

    static class CoverageTester
    {
        // Important fact: MethodBase doesn't implement equality like you'd expect.
        // So we need to use RuntimeMethodHandle for keys.

        // Maps a method to the calls somebody told us we'll do through reflection
        // when we call that method. See RegisterReflectionCall.
        static Dictionary<System.RuntimeMethodHandle, List<MethodBase>> s_reflectionCalls = new Dictionary<System.RuntimeMethodHandle, List<MethodBase>> ();

        // Cache. Maps a method to the calls it may directly make, determined by
        // looking at the instructions of the method (and s_reflectionCalls).
        static Dictionary<System.RuntimeMethodHandle, List<MethodBase>> s_directCalls = new Dictionary<System.RuntimeMethodHandle, List<MethodBase>> ();

        // Cache. Maps a method to the calls it may recursively make. This is the
        // transitive closure of s_directCalls.
        static Dictionary<System.RuntimeMethodHandle, List<MethodBase>> s_calls = new Dictionary<System.RuntimeMethodHandle, List<MethodBase>> ();

        static void PreloadCache ()
        {
            if (s_calls.Count != 0) {
                return;
            }

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
            foreach (var typ in types) {
                foreach (var method in typ.GetMethods()) {
                    GetCalls (method);
                }
            }
        }

        /// <summary>
        /// Dig through the instructions for 'method' and see which methods it
        /// calls directly. Includes calls invoked through reflection (if they were registered).
        ///
        /// This is cached, so the first call for any given function will be
        /// expensive but subsequent ones will be cheap.
        /// </summary>
        private static List<MethodBase> GetDirectCalls (MethodBase method)
        {
            // See if we've cached this already.
            List<MethodBase> calls;
            if (s_directCalls.TryGetValue (method.MethodHandle, out calls)) {
                return calls;
            }
            calls = new List<MethodBase> ();

            // Analyze the function and add those calls.
            IEnumerable<Mono.Reflection.Instruction> instructions;
            try {
                instructions = Mono.Reflection.Disassembler.GetInstructions (method);
            } catch (System.ArgumentException) {
                instructions = new Mono.Reflection.Instruction[0];
            }

            // We devirtualize calls using the 'constrained'
            // instruction hint, which is the instruction before the call
            // instruction.
            //
            // That trick only works in the context of 'top' being a generic
            // function (or a member function in a generic type), and
            // 'calledMethod' is being called on the generic type. In that
            // specific case, the CIL requires a 'constraint' instruction to be
            // emitted as a prefix to the 'callvirt' instruction. In other cases,
            // we don't get the prefix.
            //
            // We could get better devirtualization by interpreting the
            // instruction stream, but that would be much harder!
            //
            System.Type constraintType = null;
            foreach (var instruction in instructions) {
                // Is this a constraint instruction? If so, store it.
                if (instruction.OpCode == System.Reflection.Emit.OpCodes.Constrained) {
                    constraintType = instruction.Operand as System.Type;
                    continue;
                }

                // Otherwise it's maybe a call?
                MethodBase calledMethod = instruction.Operand as MethodBase;
                if (calledMethod == null) {
                    continue;
                }

                // Devirtualize the function if we can.
                if (constraintType != null && calledMethod.DeclaringType != constraintType) {
                    var parameters = calledMethod.GetParameters ();
                    var types = new System.Type[parameters.Length];
                    for (int i = 0, n = parameters.Length; i < n; ++i) {
                        types [i] = parameters [i].ParameterType;
                    }
                    var specificMethod = constraintType.GetMethod (calledMethod.Name, types);
                    if (specificMethod != null) {
                        calledMethod = specificMethod;
                    }
                }

                // We called something. Push it on the search stack, and
                // clear the constraint since we've used it up.
                calls.Add (calledMethod);
                constraintType = null;
            }

            // Also get the calls invoked through reflection, if any.
            List<MethodBase> reflectionCalls;
            s_reflectionCalls.TryGetValue (method.MethodHandle, out reflectionCalls);
            if (reflectionCalls != null) {
                calls.AddRange (reflectionCalls);
            }

            s_directCalls.Add (method.MethodHandle, calls);
            return calls;
        }

        /// <summary>
        /// Dig through the instructions for 'method' and see which methods it
        /// calls, and what methods those methods call in turn recursively.
        ///
        /// This is cached, so the first call for any given function will be
        /// expensive but subsequent ones will be cheap.
        /// </summary>
        public static List<MethodBase> GetCalls (MethodBase method)
        {
            // See if we've cached this already.
            List<MethodBase> calls;
            if (s_calls.TryGetValue (method.MethodHandle, out calls)) {
                return calls;
            }
            calls = new List<MethodBase> ();

            // Look at the current method and in DFS, find all the methods it calls.
            var stack = new List<MethodBase> ();
            var visited = new HashSet<System.RuntimeMethodHandle> ();
            stack.Add (method);
            while (stack.Count > 0) {
                var top = stack [stack.Count - 1];
                stack.RemoveAt (stack.Count - 1);

                if (!visited.Add (top.MethodHandle)) {
                    continue;
                }
                calls.Add (top);

                // If we have already seen this method, we can copy all its calls
                // in and stop this branch of our search here.
                if (s_calls.ContainsKey (top.MethodHandle)) {
                    foreach (var calledMethod in s_calls[top.MethodHandle]) {
                        visited.Add (calledMethod.MethodHandle);
                        calls.Add (calledMethod);
                    }
                    continue;
                }

                // Otherwise we get all the direct calls it makes, and add them to
                // the stack for recursive processing.
                stack.AddRange (GetDirectCalls (top));
            }

            // Store the result and return it.
            s_calls.Add (method.MethodHandle, calls);
            return calls;
        }

        /// <summary>
        /// Clear the cache; useful if you just registered a new reflection call.
        /// </summary>
        public static void ClearCache ()
        {
            s_calls = new Dictionary<System.RuntimeMethodHandle, List<MethodBase>> ();
            s_directCalls = new Dictionary<System.RuntimeMethodHandle, List<MethodBase>> ();
        }

        /// <summary>
        /// If you're using reflection to call methods, they won't be picked
        /// up automatically. This lets you add that information.
        ///
        /// You might want to clear the cache afterwards.
        /// </summary>
        public static void RegisterReflectionCall (MethodBase from, MethodBase to)
        {
            List<MethodBase> calls;
            if (s_reflectionCalls.TryGetValue (from.MethodHandle, out calls)) {
                calls.Add (to);
            } else {
                List<MethodBase> tos = new List<MethodBase> ();
                tos.Add (to);
                s_reflectionCalls [from.MethodHandle] = tos;
            }
        }

        /// <summary>
        /// Helper for TestCoverage, determines whether we called a function
        /// we're looking for -- just via a base class.
        /// </summary>
        private static bool DidCallBaseMethod (MethodInfo methodToCover,
                                          HashSet<System.RuntimeMethodHandle> calledMethods)
        {
            if ((object)methodToCover == null) {
                // mTC is a constructor, which means it can't have been called via
                // virtual function dispatch.
                return false;
            }

            var baseBaseMethod = methodToCover.GetBaseDefinition ();
            if (baseBaseMethod.MethodHandle == methodToCover.MethodHandle) {
                // mTC is the base definition, which means it can't have been called
                // via virtual function dispatch.
                return false;
            }

            if (calledMethods.Contains (baseBaseMethod.MethodHandle)) {
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
            var parameters = methodToCover.GetParameters ();
            var parameterTypes = new System.Type[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i) {
                parameterTypes [i] = parameters [i].ParameterType;
            }
            System.Type baseClass = methodToCover.DeclaringType;
            MethodBase baseMethod;
            do {
                baseClass = baseClass.BaseType;
                baseMethod = baseClass.GetMethod (methodToCover.Name, parameterTypes);
                if (calledMethods.Contains (baseMethod.MethodHandle)) {
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
        public static bool TestCoverage (IEnumerable<MethodBase> MethodsToCover,
                                    IEnumerable<MethodBase> RootMethods,
                                    out List<MethodBase> out_HitMethods,
                                    out List<MethodBase> out_MissedMethods
        )
        {
            PreloadCache ();

            // MethodsToCover and RootMethods may have duplicates;
            // use 'unique' to avoid doing the work twice.
            var unique = new HashSet<System.RuntimeMethodHandle> ();

            // Collect up the handles we called.
            var calledMethods = new HashSet<System.RuntimeMethodHandle> ();
            unique.Clear ();
            foreach (var rootMethod in RootMethods) {
                if (!unique.Add (rootMethod.MethodHandle)) {
                    continue;
                }
                foreach (var called in GetCalls(rootMethod)) {
                    calledMethods.Add (called.MethodHandle);
                }
            }

            out_MissedMethods = new List<MethodBase> ();
            out_HitMethods = new List<MethodBase> ();
            unique.Clear ();
            foreach (var methodToCover in MethodsToCover) {
                if (!unique.Add (methodToCover.MethodHandle)) {
                    continue;
                }

                // Did we call the method?
                if (calledMethods.Contains (methodToCover.MethodHandle)) {
                    out_HitMethods.Add (methodToCover);
                    continue;
                }

                // Did we call a base class declaration of the method?
                // If so, we might call the method we're looking for through
                // virtual function dispatch.
                if (DidCallBaseMethod (methodToCover as MethodInfo, calledMethods)) {
                    out_HitMethods.Add (methodToCover);
                    continue;
                }

                // No other excuses? We must have missed it.
                out_MissedMethods.Add (methodToCover);
            }

            if (out_MissedMethods.Count == 0) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Collect all the methods of the type that we want to cover.
        /// Includes all public instance methods, all public static methods,
        /// all public constructors, all public property getters and setters.
        /// </summary>
        public static void CollectMethodsToCover (System.Type TypeToCover, List<MethodBase> MethodsToCover)
        {
            // Don't cover anything for enums, they're basically compiler-generated
            // types.
            if (TypeToCover.IsEnum) {
                return;
            }

            // We want to call all the methods of the proxy, including all the constructors.
            int firstIndex = MethodsToCover.Count;
            MethodsToCover.AddRange (TypeToCover.GetMethods ());
            MethodsToCover.AddRange (TypeToCover.GetConstructors ());

            // Testers will often use EqualityTester on the type. Register its
            // reflection calls.
            var eqTester = typeof(UnitTests.EqualityTester<>).MakeGenericType (TypeToCover);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor (eqTester.TypeHandle);

            // In calling any method on the type, the static initializer will be invoked.
            if (TypeToCover.TypeInitializer != null) {
                for (int i = firstIndex, n = MethodsToCover.Count; i < n; ++i) {
                    RegisterReflectionCall (MethodsToCover [i], TypeToCover.TypeInitializer);
                }
            }
        }

        /// <summary>
        /// Filter a list of methods to get the methods that the unit test
        /// framework will interpret as tests.
        /// </summary>
        public static void CollectTestMethods (IEnumerable<MethodBase> PotentialTestMethods, List<MethodBase> TestMethods)
        {
            foreach (var method in PotentialTestMethods) {
                // Check that the method is tagged [Test]
                if (method.GetCustomAttributes (typeof(NUnit.Framework.TestAttribute), true).Length == 0) {
                    continue;
                }

                TestMethods.Add (method);

                /* Invoke the declaring type's static init so it registers its
             * reflection calls. */
                System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor (method.DeclaringType.TypeHandle);
            }
        }

        public static void CollectTestMethods (System.Type TestClass, List<MethodBase> TestMethods)
        {
            CollectTestMethods (TestClass.GetMethods (), TestMethods);
        }

        /// <summary>
        /// Simple interface for running an NUnit test.
        /// <code>
        ///    [Test]
        ///    public void TestCoverage() { CoverageTester.TestCoverage(typeof(ThingWeAreTesting), this.GetType()); }
        /// </code>
        /// </summary>
        public static void TestCoverage (System.Type TypeToCover, System.Type NUnitTestFramework)
        {
            var methodsToCover = new List<MethodBase> ();
            CollectMethodsToCover (TypeToCover, methodsToCover);

            // Our public test functions are what we can use to call that with.
            var testMethods = new List<MethodBase> ();
            CollectTestMethods (NUnitTestFramework.GetMethods (), testMethods);

            List<MethodBase> hitMethods;
            List<MethodBase> missedMethods;

            var coverageComplete = CoverageTester.TestCoverage (methodsToCover, testMethods, out hitMethods, out missedMethods);

            NUnit.Framework.Assert.That (
                () => coverageComplete,
                () => CoverageTester.MakeCoverageMessage (hitMethods, missedMethods));
        }

        public static string GetMethodSignature (MethodBase info)
        {
            var builder = new System.Text.StringBuilder ();
            if (info.IsConstructor) {
                builder.Append (info.DeclaringType.Name);
            } else {
                var method = info as MethodInfo;
                if (method != null) {
                    builder.Append (method.ReturnType.Name);
                    builder.Append (' ');
                }
                if (info.DeclaringType != null) {
                    builder.Append (info.DeclaringType.Name);
                    builder.Append ('.');
                }
                builder.Append (info.Name);
            }
            builder.Append ('(');
            var args = info.GetParameters ();
            if (args.Length > 0) {
                builder.Append (args [0].ParameterType.Name);
            }
            for (var i = 1; i < args.Length; ++i) {
                builder.Append (", ");
                builder.Append (args [i].ParameterType.Name);
            }
            builder.Append (')');
            return builder.ToString ();
        }

        static string[] GetUniqueSortedSignatures (IEnumerable<MethodBase> Methods)
        {
            var unique = new HashSet<System.RuntimeMethodHandle> ();

            // Eliminate duplicates
            var methods = new List<MethodBase> ();
            foreach (var method in Methods) {
                if (!unique.Add (method.MethodHandle)) {
                    continue;
                }
                methods.Add (method);
            }
            unique.Clear ();

            // Sort first by declaring type name, then by method name, then by signature
            methods.Sort ((MethodBase a, MethodBase b) => {
                var aname = a.DeclaringType == null ? "" : a.DeclaringType.Name;
                var bname = b.DeclaringType == null ? "" : b.DeclaringType.Name;
                var namecompare = aname.CompareTo (bname);
                if (namecompare != 0) {
                    return namecompare;
                }

                aname = a.Name;
                bname = b.Name;
                namecompare = aname.CompareTo (bname);
                if (namecompare != 0) {
                    return namecompare;
                }

                aname = GetMethodSignature (a);
                bname = GetMethodSignature (b);
                namecompare = aname.CompareTo (bname);
                if (namecompare != 0) {
                    return namecompare;
                }

                return 0;
            });

            // Convert to an array of string
            var signatures = new string[methods.Count];
            for (int i = 0, n = methods.Count; i < n; ++i) {
                signatures [i] = GetMethodSignature (methods [i]);
            }
            return signatures;
        }

        public static string MakeCoverageMessage (
            IEnumerable<MethodBase> HitMethods,
            IEnumerable<MethodBase> MissedMethods)
        {
            return string.Format ("Failed to call:\n\t{0}\nSucceeded to call:\n\t{1}",
                string.Join ("\n\t", GetUniqueSortedSignatures (MissedMethods)),
                string.Join ("\n\t", GetUniqueSortedSignatures (HitMethods)));
        }
    }
}
#endif
