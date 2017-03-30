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
            SWIGTYPE_p_int major = cpp.new_intp ();
            SWIGTYPE_p_int minor = cpp.new_intp ();
            SWIGTYPE_p_int revision = cpp.new_intp ();

            // Assume that the revision will always be a positive number
            cpp.intp_assign (major, -1);
            cpp.intp_assign (minor, -1);
            cpp.intp_assign (revision, -1);

            Assert.AreEqual (cpp.intp_value (major), -1);
            Assert.AreEqual (cpp.intp_value (minor), -1);
            Assert.AreEqual (cpp.intp_value (revision), -1);

            FbxManager.GetFileFormatVersion (major, minor, revision);

            Assert.GreaterOrEqual (cpp.intp_value (major), 0);
            Assert.GreaterOrEqual (cpp.intp_value (minor), 0);
            Assert.GreaterOrEqual (cpp.intp_value (revision), 0);

            cpp.delete_intp (major);
            cpp.delete_intp (minor);
            cpp.delete_intp (revision);
        }

        [Test]
        public void TestFindClass ()
        {
            SWIGTYPE_p_FbxClassId classId = m_fbxManager.FindClass ("FbxObject");

        }
    }
}