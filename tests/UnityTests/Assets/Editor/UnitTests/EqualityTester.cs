
using NUnit.Framework;
using System.Collections.Generic;

namespace UnitTests
{
    public static class EqualityTester<T> where T: class
    {
        // T.Equals(T), T.Equals(base(T), ...
        static List<System.Reflection.MethodInfo> s_Equals = new List<System.Reflection.MethodInfo>();

        // operator== (T, T), operator== (base(T), base(T), ...
        static List<System.Reflection.MethodInfo> s_op_Equality = new List<System.Reflection.MethodInfo>();

        // operator!= (T, T), operator== (base(T), base(T), ...
        static List<System.Reflection.MethodInfo> s_op_Inequality = new List<System.Reflection.MethodInfo>();

        static EqualityTester() {
            // For T and its base classes B1, B2, ...
            // get the following functions so we can test equality:
            // bool Equals(U)
            // static bool operator == (U, U)
            // static bool operator != (U, U)
            var U = typeof(T);
            do {
                // Get all the methods, look for Equals(U), op_Equality(U,U), and op_Inequality(U,U)
                var methods = U.GetMethods();
                foreach(var method in methods) {
                    if (method.Name == "Equals") {
                        var parms = method.GetParameters();
                        if (parms.Length == 1 && parms[0].ParameterType == U) {
                            s_Equals.Add(method);
                        }
                    } else if (method.Name == "op_Equality") {
                        var parms = method.GetParameters();
                        if (parms.Length == 2 && parms[0].ParameterType == U && parms[1].ParameterType == U) {
                            s_op_Equality.Add(method);
                        }
                    } else if (method.Name == "op_Inequality") {
                        var parms = method.GetParameters();
                        if (parms.Length == 2 && parms[0].ParameterType == U && parms[1].ParameterType == U) {
                            s_op_Inequality.Add(method);
                        }
                    }
                }

                // Repeat on the base type, if there is one.
                U = U.BaseType;
            } while (U != null);

#if ENABLE_COVERAGE_TEST
            // Register the calls we make through reflection.
            var testEquality = typeof(EqualityTester<T>).GetMethod("TestEquality");
            foreach(var equals in s_Equals) { CoverageTester.RegisterReflectionCall(testEquality, equals); }
            foreach(var equals in s_op_Equality) { CoverageTester.RegisterReflectionCall(testEquality, equals); }
            foreach(var equals in s_op_Inequality) { CoverageTester.RegisterReflectionCall(testEquality, equals); }
#endif
        }

        /*
         * Register the reflection calls with the coverage tester immediately.
         *
         * If you test coverage before you test equality, the coverage test
         * might not notice the equality test. Calling this function before any
         * testing will fix that problem.
         */
        public static void RegisterCoverage() { /* This call forced the static init to get invoked. */ }

        public static void TestEquality(T a, T b) {
            // Test all the Equals functions on a.
            // a.Equals(a) is true
            // a.Equals(b) is false
            // a.Equals(null) is false and doesn't throw an exception
            foreach(var equals in s_Equals) {
                Assert.IsTrue(Invoker.Invoke<bool>(equals, a, a));
                Assert.IsFalse(Invoker.Invoke<bool>(equals, a, b));
                Assert.IsFalse(Invoker.Invoke<bool>(equals, a, null));
            }

            // test operator== in various cases including null handling
            foreach(var equals in s_op_Equality) {
                Assert.IsTrue(Invoker.InvokeStatic<bool>(equals, a, a ));
                Assert.IsFalse(Invoker.InvokeStatic<bool>(equals, a, b ));
                Assert.IsFalse(Invoker.InvokeStatic<bool>(equals, a, null ));
                Assert.IsFalse(Invoker.InvokeStatic<bool>(equals, null, b ));
                Assert.IsTrue(Invoker.InvokeStatic<bool>(equals, null, null ));
            }

            // test operator!= in the same cases; should always return ! the answer
            foreach(var equals in s_op_Inequality) {
                Assert.IsTrue(!Invoker.InvokeStatic<bool>(equals, a, a ));
                Assert.IsFalse(!Invoker.InvokeStatic<bool>(equals, a, b ));
                Assert.IsFalse(!Invoker.InvokeStatic<bool>(equals, a, null ));
                Assert.IsFalse(!Invoker.InvokeStatic<bool>(equals, null, b ));
                Assert.IsTrue(!Invoker.InvokeStatic<bool>(equals, null, null ));
            }
        }
    }
}
