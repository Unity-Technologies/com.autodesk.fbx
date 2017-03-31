using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{

    public class FbxManagerTest
    {

        FbxManager m_fbxManager;

        [SetUp]
        public void Init ()
        {
            m_fbxManager = FbxManager.Create ();
        }

        [TearDown]
        public void End ()
        {
            m_fbxManager.Destroy ();
        }

        [Test]
        public void TestVersion ()
        {
            string version = FbxManager.GetVersion (true);

            Assert.IsNotEmpty (version);

            string versionShort = FbxManager.GetVersion (false);

            Assert.IsNotEmpty (versionShort);
        }

        [Test]
        public void TestGetFileFormatVersion ()
        {
            int major = -1, minor = -1, revision = -1;

            FbxManager.GetFileFormatVersion (out major, out minor, out revision);

            Assert.GreaterOrEqual (major, 0);
            Assert.GreaterOrEqual (minor, 0);
            Assert.GreaterOrEqual (revision, 0);

        }

        [Test]
        public void TestFindClass ()
        {
            FbxClassId classId = m_fbxManager.FindClass ("FbxObject");

            Assert.AreEqual (classId.GetName (), "FbxObject");
        }
    }
}