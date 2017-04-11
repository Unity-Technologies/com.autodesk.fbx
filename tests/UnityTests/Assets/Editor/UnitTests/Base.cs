using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;
using System;

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

        private T m_testObject;

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
        public virtual void InitTest ()
        {
            m_fbxManager = FbxManager.Create ();
            m_testObject = CreateObject ();
        }

        [TearDown]
        public virtual void DestroyTest ()
        {
            try {
                m_testObject.Destroy ();
            } catch (System.ArgumentNullException) {
            }

            try {
                m_fbxManager.Destroy ();
            } catch (System.ArgumentNullException) {
            }
        }

        [Test]
        public void TestCreate()
        {
            var obj = CreateObject("MyObject");
            Assert.IsInstanceOf<T> (obj);
        }

        [Test]
        [ExpectedException (typeof(System.NullReferenceException))]
        public void TestCreateNullManager()
        {
            var obj = CreateObject(null, "MyObject");
        }

        [Test]
        public void TestCreateNullName()
        {
            var obj = CreateObject((string)null);
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestCreateZombieManager()
        {
            var mgr = FbxManager.Create();
            mgr.Destroy();
            var obj = CreateObject(mgr, "MyObject");
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
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroyedZombie ()
        {
            // Test that if we try to use an object after Destroy()ing it,
            // we get an exception (not a crash).
            var obj = CreateObject();
            Assert.IsNotNull (obj);
            obj.Destroy ();
            obj.GetName ();
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
            Assert.That (() => { zombie.GetName (); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        public void TestSelected ()
        {
            Assert.IsNotNull (m_testObject);

            Assert.IsFalse (m_testObject.GetSelected ());
            m_testObject.SetSelected (true);
            Assert.IsTrue (m_testObject.GetSelected ());
        }
    }
}
