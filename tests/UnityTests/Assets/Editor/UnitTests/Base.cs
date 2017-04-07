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
                // TODO: crashes instead of going to the catch block if fbx manager already destroyed
                m_fbxManager.Destroy ();
            } catch (System.ArgumentNullException) {
            }
        }

        [Test]
        public void TestCreateDestroy ()
        {
            var obj = CreateObject ();

            Assert.IsNotNull (obj);
            Assert.IsInstanceOf<FbxObject> (obj);

            // there are two destroy methods
            obj.Destroy (true);
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestZombie ()
        {
            m_testObject.Destroy ();
            m_testObject.GetName ();
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