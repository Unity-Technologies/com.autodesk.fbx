// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using Autodesk.Fbx;
using System.Collections.Generic;
using System.Reflection;

namespace Autodesk.Fbx.UnitTests
{
    public class GlobalsTest
    {
        const string kPINVOKE = "NativeMethods";
        static System.Type s_PINVOKEtype;
        static ConstructorInfo s_PINVOKEctor;
        static List<MethodInfo> s_UpcastFunctions = new List<MethodInfo>();

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() {
            /* Search the current assembly for unit tests. */
            var alltypes = GetType().Assembly.GetTypes();
            var unitTestMethods = new List<MethodBase>();
            foreach(var t in alltypes) {
                CoverageTester.CollectTestMethods(t, unitTestMethods);
            }

            /* Search the assembly that Autodesk.Fbx.Globals is in to find classes in
             * the FbxSdk namespace to test. */
            alltypes = typeof(Autodesk.Fbx.Globals).Assembly.GetTypes();
            var methodsToCover = new List<MethodBase>();
            foreach(var t in alltypes) {
                if (t.Namespace != "Autodesk.Fbx") {
                    continue;
                }

                /* don't take in delegates; we can't properly track coverage,
                   so just avoid the false negative */
                if (t.IsSubclassOf(typeof(System.Delegate))) {
                    continue;
                }

                /* take in the PINVOKE class but skip its helper classes */
                bool skip = false;
                for(var u = t.DeclaringType ; u != null; u = u.DeclaringType) {
                    if (u.TypeHandle.Value == s_PINVOKEtype.TypeHandle.Value) {
                        skip = true;
                        break;
                    }
                }
                if (skip) { continue; }

                CoverageTester.CollectMethodsToCover(t, methodsToCover);
            }

            List<MethodBase> hitMethods = new List<MethodBase>();
            List<MethodBase> missedMethods = new List<MethodBase>();
            var ok = CoverageTester.TestCoverage(methodsToCover, unitTestMethods, out hitMethods, out missedMethods);
            NUnit.Framework.Assert.That(
                    () => ok,
                    () => CoverageTester.MakeCoverageMessage(hitMethods, missedMethods));
        }
#endif

        static GlobalsTest()
        {
            /* We test the PINVOKE class by reflection since it's private to
             * its assembly. */
            var alltypes = typeof(Autodesk.Fbx.Globals).Assembly.GetTypes();
            foreach(var t in alltypes) {
                if (t.Namespace == "Autodesk.Fbx" && t.Name == kPINVOKE) {
                    s_PINVOKEtype = t;
                    break;
                }
            }
            Assert.IsNotNull(s_PINVOKEtype);

            s_PINVOKEctor = s_PINVOKEtype.GetConstructor(new System.Type[] {});

            foreach(var m in s_PINVOKEtype.GetMethods()) {
                if (m.Name.EndsWith("SWIGUpcast")) {
                    s_UpcastFunctions.Add(m);
                }
            }

#if ENABLE_COVERAGE_TEST
            var basicTests = typeof(GlobalsTest).GetMethod("BasicTests");
            if (s_PINVOKEctor != null) {
                CoverageTester.RegisterReflectionCall(basicTests, s_PINVOKEctor);
            }

            foreach(var m in s_UpcastFunctions) {
                CoverageTester.RegisterReflectionCall(basicTests, m);
            }
#endif
        }

        bool ProgressCallback(float a, string b) { return true; }

        [Test]
        public void BasicTests ()
        {
            /* Try to create the Globals, which isn't
             * static, so the coverage tests want us to create them. */
            new Globals();

            /* Create the NativeMethods, which isn't static.
             * But it is protected, so we can't create it normally,
             * which is why we use reflection. */
            s_PINVOKEctor.Invoke(null);

            /* Don't actually invoke the SWIGUpcast functions. They're a
             * feature to handle multiple inheritance. But FBX SDK doesn't use
             * multiple inheritance anyway. */
        }

    }
}
