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
            string version = FbxManager.GetVersion ();
            Assert.IsNotEmpty (version);
            
            string versionLong = FbxManager.GetVersion (true);
            Assert.IsNotEmpty (versionLong);

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

        [Test]
        public void TestIOSettings ()
        {
            FbxIOSettings ioSettings = m_fbxManager.GetIOSettings ();
            Assert.IsNull(ioSettings);

            using (FbxIOSettings ioSettings1 = FbxIOSettings.Create (m_fbxManager, "")) {
                m_fbxManager.SetIOSettings (ioSettings1);

                FbxIOSettings ioSettings2 = m_fbxManager.GetIOSettings ();
                Assert.IsNotNull (ioSettings2);
            }
        }

        [Test]
        public void TestIdentity ()
        {
            using (FbxObject obj = FbxObject.Create (m_fbxManager, "")) {
                FbxManager fbxManager2 = obj.GetFbxManager();
                
                Assert.AreEqual (m_fbxManager, fbxManager2);
            }
        }

        [Test]
        public void TestUsing ()
        {
            // Test that the using statement works, and destroys the manager.
            FbxObject obj;
            using (var mgr = FbxManager.Create ()) {
                obj = FbxObject.Create(mgr, "asdf");
            }
            Assert.That(() => { obj.GetName (); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }
    }
}
