using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxImporterTest
    {
        FbxManager m_fbxManager;

        [SetUp]
        public void Init ()
        {
            m_fbxManager = FbxManager.Create ();
        }

        [TearDown]
        public void Term ()
        {
            m_fbxManager.Destroy ();
        }

        [Test]
        public void TestCreate ()
        {
            using (FbxImporter newImporter = FbxImporter.Create (m_fbxManager, ""))
            {
                Assert.IsNotNull (newImporter);
                Assert.IsInstanceOf<FbxImporter> (newImporter);
                Assert.IsInstanceOf<FbxObject> (newImporter);
                Assert.IsInstanceOf<FbxEmitter> (newImporter);

                newImporter.Destroy();
            }
        }

        [Test]
        public void TestImport1 ()
        {
            using (FbxImporter newImporter = FbxImporter.Create (m_fbxManager, "MyImporter"))
            {
                Assert.IsFalse (newImporter.Import (null));
            }
        }

        [Test]
        public void TestImport2 ()
        {
            using (FbxImporter newImporter = FbxImporter.Create (m_fbxManager, "MyImporter"))
            {
                Assert.IsFalse (newImporter.Import (null, false));
                
                // don't ask
                Assert.IsTrue (newImporter.Import (null, true));
            }
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestZombie1 ()
        {
            FbxImporter zombieScene;

            using (FbxImporter newImporter = FbxImporter.Create (m_fbxManager, ""))
            {
                zombieScene = newImporter;

                newImporter.Destroy();

                Assert.AreEqual (zombieScene.GetName (), "");
            }
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        [Ignore("CRASHES accessing zombie")]
        public void TestZombie2 ()
        {
            FbxImporter zombieScene;

            using (FbxImporter newImporter = FbxImporter.Create (m_fbxManager, ""))
            {
                zombieScene = newImporter;
            }
            Assert.AreEqual (zombieScene.GetName (), "");
        }
    }
}
