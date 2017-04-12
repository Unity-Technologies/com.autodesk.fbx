// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public abstract class Base<T> where T: FbxSdk.FbxObject
    {
        private FbxManager m_fbxManager;

        protected FbxManager FbxManager {
            get {
                return m_fbxManager;
            }
        }

        /* Create an object with the default manager. */
        protected T CreateObject (string name = "") {
            return CreateObject(m_fbxManager, name);
        }

        /* Create an object with another manager. Default implementation uses
         * reflection to call T.Create(...); override if reflection is wrong. */
        protected virtual T CreateObject (FbxManager mgr, string name = "") {
            try {
                return (T)(typeof(T).GetMethod("Create").Invoke(null, new object[] {mgr, name}));
            } catch(System.Reflection.TargetInvocationException xcp) {
				throw xcp.GetBaseException();
            }
        }

        [SetUp]
        public virtual void Init ()
        {
            m_fbxManager = FbxManager.Create ();
        }

        [TearDown]
        public virtual void Term ()
        {
            try {
                m_fbxManager.Destroy ();
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
        public void TestCreateNullManager()
        {
            Assert.That (() => { CreateObject(null, "MyObject"); }, Throws.Exception.TypeOf<System.NullReferenceException>());
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
            m_fbxManager.Destroy();
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
