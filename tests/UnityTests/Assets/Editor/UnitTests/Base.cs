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
        // T.Create(FbxManager, string)
        static System.Reflection.MethodInfo s_createFromMgrAndName;

        // T.Create(FbxObject, string)
        static System.Reflection.MethodInfo s_createFromObjAndName;

        // T.Equals(T), T.Equals(base(T), ...
        static List<System.Reflection.MethodInfo> s_Equals = new List<System.Reflection.MethodInfo>();

        // operator== (T, T), operator== (base(T), base(T), ...
        static List<System.Reflection.MethodInfo> s_op_Equality = new List<System.Reflection.MethodInfo>();

        // operator!= (T, T), operator== (base(T), base(T), ...
        static List<System.Reflection.MethodInfo> s_op_Inequality = new List<System.Reflection.MethodInfo>();

        static Base() {
            s_createFromMgrAndName = typeof(T).GetMethod("Create", new System.Type[] {typeof(FbxManager), typeof(string)});
            s_createFromObjAndName = typeof(T).GetMethod("Create", new System.Type[] {typeof(FbxObject), typeof(string)});

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

            // We use reflection in CreateObject(FbxManager, string) and CreateObject(FbxObject, string).
            if (s_createFromMgrAndName != null) {
                var createFromMgrAndName = typeof(Base<T>).GetMethod("CreateObject", new System.Type[] {typeof(FbxManager), typeof(string)});
                CoverageTester.RegisterReflectionCall(createFromMgrAndName, s_createFromMgrAndName);
            }
            if (s_createFromObjAndName != null) {
                var createFromObjAndName = typeof(Base<T>).GetMethod("CreateObject", new System.Type[] {typeof(FbxObject), typeof(string)});
                CoverageTester.RegisterReflectionCall(createFromObjAndName, s_createFromObjAndName);
            }

            // We use reflection in TestEquality
            var testEquality = typeof(Base<T>).GetMethod("TestEquality");
            foreach(var equals in s_Equals) { CoverageTester.RegisterReflectionCall(testEquality, equals); }
            foreach(var equals in s_op_Equality) { CoverageTester.RegisterReflectionCall(testEquality, equals); }
            foreach(var equals in s_op_Inequality) { CoverageTester.RegisterReflectionCall(testEquality, equals); }
#endif
        }

        static U Invoke<U>(System.Reflection.MethodInfo method, object instance, params object [] args) {
            try {
                return (U)(method.Invoke(instance, args));
            } catch(System.Reflection.TargetInvocationException xcp) {
                throw xcp.GetBaseException();
            }
        }
        static U InvokeStatic<U>(System.Reflection.MethodInfo method, params object [] args) {
            try {
                return (U)(method.Invoke(null, args));
            } catch(System.Reflection.TargetInvocationException xcp) {
                throw xcp.GetBaseException();
            }
        }

        protected FbxManager Manager {
            get;
            private set;
        }

        /* Create an object with the default manager. */
        public T CreateObject (string name = "") {
            return CreateObject(Manager, name);
        }

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(T), this.GetType()); }
#endif

        /* Test all the equality functions we can find. */
        [Test]
        public virtual void TestEquality() {
            var a = CreateObject("a");
            var b = CreateObject("b");

            // Test all the Equals functions on a.
            // a.Equals(a) is true
            // a.Equals(b) is false
            // a.Equals(null) is false and doesn't throw an exception
            foreach(var equals in s_Equals) {
                Assert.IsTrue(Invoke<bool>(equals, a, a));
                Assert.IsFalse(Invoke<bool>(equals, a, b));
                Assert.IsFalse(Invoke<bool>(equals, a, (object)null)); // passing null passes a null params
            }

            // test operator== in various cases including null handling
            foreach(var equals in s_op_Equality) {
                Assert.IsTrue(InvokeStatic<bool>(equals, a, a ));
                Assert.IsFalse(InvokeStatic<bool>(equals, a, b ));
                Assert.IsFalse(InvokeStatic<bool>(equals, a, null ));
                Assert.IsFalse(InvokeStatic<bool>(equals, null, b ));
                Assert.IsTrue(InvokeStatic<bool>(equals, null, null ));
            }

            // test operator!= in the same cases; should always return ! the answer
            foreach(var equals in s_op_Inequality) {
                Assert.IsTrue(!InvokeStatic<bool>(equals, a, a ));
                Assert.IsFalse(!InvokeStatic<bool>(equals, a, b ));
                Assert.IsFalse(!InvokeStatic<bool>(equals, a, null ));
                Assert.IsFalse(!InvokeStatic<bool>(equals, null, b ));
                Assert.IsTrue(!InvokeStatic<bool>(equals, null, null ));
            }
        }

        /* Create an object with another manager. Default implementation uses
         * reflection to call T.Create(...); override if reflection is wrong. */
        public virtual T CreateObject (FbxManager mgr, string name = "") {
            return InvokeStatic<T>(s_createFromMgrAndName, mgr, name);
        }

        /* Create an object with an object as container. Default implementation uses
         * reflection to call T.Create(...); override if reflection is wrong. */
        public virtual T CreateObject (FbxObject container, string name = "") {
            return InvokeStatic<T>(s_createFromObjAndName, container, name);
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
            Assert.AreEqual(Manager, obj.GetFbxManager());
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

        [Test]
        public void TestNames ()
        {
            /*
             * We use this also for testing that string handling works.
             * Make sure we can pass const char*, FbxString, and const
             * FbxString&.
             * Make sure we can return those too (though I'm not actually
             * seeing a return of a const-ref anywhere).
             */

            // Test a function that takes const char*.
            FbxObject obj = FbxObject.Create(Manager, "MyObject");
            Assert.IsNotNull (obj);

            // Test a function that returns const char*.
            Assert.AreEqual ("MyObject", obj.GetName ());

            // Test a function that takes an FbxString with an accent in it.
            obj.SetNameSpace("Accentué");

            // Test a function that returns FbxString.
            Assert.AreEqual ("MyObject", obj.GetNameWithoutNameSpacePrefix ());

            // Test a function that returns FbxString with an accent in it.
            Assert.AreEqual ("Accentué", obj.GetNameSpaceOnly());

            // Test a function that takes a const char* and returns an FbxString.
            // We don't want to convert the other StripPrefix functions, which
            // modify their argument in-place.
            Assert.AreEqual("MyObject", FbxObject.StripPrefix("NameSpace::MyObject"));

            obj.Destroy();
        }
    }
}
