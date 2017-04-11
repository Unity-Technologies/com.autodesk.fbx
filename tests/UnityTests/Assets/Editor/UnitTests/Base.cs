using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;
using System;

namespace UnitTests
{
    public abstract class Base
    {

        private FbxManager m_fbxManager;

        protected FbxManager FbxManager {
            get {
                return m_fbxManager;
            }
        }

        private FbxObject m_testObject;

        protected abstract FbxObject CreateObject ();


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
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroyedManagerZombie ()
        {
            // Test that if we try to use an object after Destroy()ing its
            // manager, the object was destroyed as well.
            var obj = CreateObject();
            Assert.IsNotNull (obj);
            m_fbxManager.Destroy();
            obj.GetName ();
        }

        [Test]
        [ExpectedException (typeof(System.NullReferenceException))]
        public void TestDisposedZombie ()
        {
            // Test that if we try to use an object after Dispose()ing it,
            // we get an exception (not a crash). This is a regression test
            // based on some wrong code:
            FbxObject zombie;
            using(var obj = CreateObject()) {
                Assert.IsNotNull (obj);
                zombie = obj;
            }
            zombie.GetName ();
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
