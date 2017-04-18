// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using FbxSdk;

using System.Collections.Generic;

namespace UnitTests
{
    public abstract class Base<T> where T: FbxSdk.FbxObject
    {
        static Base() {
            s_createFromMgrAndName = typeof(T).GetMethod("Create", new System.Type[] {typeof(FbxManager), typeof(string)});
            s_createFromObjAndName = typeof(T).GetMethod("Create", new System.Type[] {typeof(FbxObject), typeof(string)});

#if ENABLE_COVERAGE_TEST
            // Register the calls we make through reflection.
            // We use reflection in CreateObject(FbxManager, string) and CreateObject(FbxObject, string).
            var CreateObjectMethods = typeof(Base<T>).GetMethods(
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Public
                    );
            foreach(var com in CreateObjectMethods) {
                if (com.Name != "CreateObject") {
                    continue;
                }
                var parms = com.GetParameters();
                if (parms.Length != 2) {
                    continue;
                }
                if (parms[1].ParameterType != typeof(string)) {
                    continue;
                }
                if (parms[1].ParameterType == typeof(FbxManager)) {
                    CoverageTester.RegisterReflectionCall(com, s_createFromMgrAndName);
                }
                if (parms[1].ParameterType == typeof(FbxObject)) {
                    CoverageTester.RegisterReflectionCall(com, s_createFromObjAndName);
                }
            }
#endif
        }

        protected FbxManager Manager {
            get;
            private set;
        }

        /* Create an object with the default manager. */
        protected T CreateObject (string name = "") {
            return CreateObject(Manager, name);
        }

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage()
        {
            // We want to call all the functions of the proxy.
            var proxyMethods = typeof(T).GetMethods();

            // Our public test functions are what we can use to call that with.
            var selfMethods = new HashSet<System.Reflection.MethodInfo>();
            foreach(var method in this.GetType().GetMethods()) {
                // Check that the method is tagged [Test]
                if (method.GetCustomAttributes(typeof(TestAttribute), true).Length > 0) {
                    selfMethods.Add(method);
                }
            }

            // Don't include this function.
            selfMethods.Remove(typeof(Base<T>).GetMethod("TestCoverage", new System.Type[] {}));

            HashSet<System.Reflection.MethodInfo> hitMethods;
            HashSet<System.Reflection.MethodInfo> missedMethods;

            var coverageComplete = CoverageTester.TestCoverage(proxyMethods, selfMethods, out hitMethods, out missedMethods);

            Assert.That(
                    () => coverageComplete,
                    () => CoverageTester.MakeCoverageMessage(hitMethods, missedMethods));
        }
#endif

        /* Create an object with another manager. Default implementation uses
         * reflection to call T.Create(...); override if reflection is wrong. */
        static System.Reflection.MethodInfo s_createFromMgrAndName;
        protected virtual T CreateObject (FbxManager mgr, string name = "") {
            try {
                return (T)(s_createFromMgrAndName.Invoke(null, new object[] {mgr, name}));
            } catch(System.Reflection.TargetInvocationException xcp) {
                throw xcp.GetBaseException();
            }
        }

        /* Create an object with an object as container. Default implementation uses
         * reflection to call T.Create(...); override if reflection is wrong. */
        static System.Reflection.MethodInfo s_createFromObjAndName;
        protected virtual T CreateObject (FbxObject container, string name = "") {
            try {
                return (T)(s_createFromObjAndName.Invoke(null, new object[] {container, name}));
            } catch(System.Reflection.TargetInvocationException xcp) {
                throw xcp.GetBaseException();
            }
        }

        [SetUp]
        public virtual void Init ()
        {
            Manager = FbxManager.Create ();
        }

        [TearDown]
        public virtual void Term ()
        {
            try {
                Manager.Destroy ();
            }
            catch (System.ArgumentNullException) {
            }
        }

        [Test]
        public void TestCreate()
        {
            var obj = CreateObject("MyObject");
            Assert.IsInstanceOf<T> (obj);
        }

        [Test]
        public void TestCreateNullContainer()
        {
            Assert.That (() => { CreateObject((FbxManager)null, "MyObject"); }, Throws.Exception.TypeOf<System.NullReferenceException>());
            Assert.That (() => { CreateObject((FbxObject)null, "MyObject"); }, Throws.Exception.TypeOf<System.NullReferenceException>());
        }

        [Test]
        public void TestCreateNullName()
        {
            CreateObject((string)null);
        }

        [Test]
        public void TestCreateZombieManager()
        {
            var mgr = FbxManager.Create();
            mgr.Destroy();
            Assert.That (() => { CreateObject(mgr, "MyObject"); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        public void TestDestroySelf ()
        {
            // We can call destroy with no args, or with a 'false' argument.
            // Test both.
            var obj = CreateObject ();

            Assert.IsNotNull (obj);
            Assert.IsInstanceOf<FbxObject> (obj);
            obj.Destroy ();

            var obj2 = CreateObject ();
            obj2.Destroy (false);
        }

        [Test]
        public void TestDestroyRecursive ()
        {
            var obj = CreateObject ();

            Assert.IsNotNull (obj);
            Assert.IsInstanceOf<FbxObject> (obj);
            obj.Destroy (true);
        }

        [Test]
        public void TestUsing ()
        {
            // Test that the using statement works.
            using (var obj = CreateObject ()) {
                obj.GetName ();
            }

			// Test also that an explicit Dispose works.
			var obj2 = CreateObject();
			obj2.Dispose();
        }

        [Test]
        public void TestDestroyedZombie ()
        {
            // Test that if we try to use an object after Destroy()ing it,
            // we get an exception (not a crash).
            var obj = CreateObject();
            Assert.IsNotNull (obj);
            obj.Destroy ();
            Assert.That (() => { obj.GetName (); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        public void TestDestroyedManagerZombie ()
        {
            // Test that if we try to use an object after Destroy()ing its
            // manager, the object was destroyed as well.
            var obj = CreateObject();
            Assert.IsNotNull (obj);
            Manager.Destroy();
            Assert.That (() => { obj.GetName (); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        public void TestDisposedZombie ()
        {
            // Test that if we try to use an object after Dispose()ing it,
            // we get an exception (not a crash). This is a regression test
            // based on some wrong code:
            T zombie;
            using(var obj = CreateObject()) {
                Assert.IsNotNull (obj);
                zombie = obj;
            }
            Assert.That (() => { zombie.GetName (); }, Throws.Exception.TypeOf<System.NullReferenceException>());
        }

        [Test]
        public void TestSelected ()
        {
            var obj = CreateObject ();
            Assert.IsNotNull (obj);

            Assert.IsFalse (obj.GetSelected ());
            obj.SetSelected (true);
            Assert.IsTrue (obj.GetSelected ());
        }
    }
}
