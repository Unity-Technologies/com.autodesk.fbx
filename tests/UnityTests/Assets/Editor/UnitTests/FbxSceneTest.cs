using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxSceneTest
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
            using (FbxScene newScene = FbxScene.Create (m_fbxManager, ""))
            {
                Assert.IsNotNull (newScene);
                Assert.IsInstanceOf<FbxObject> (newScene);

                newScene.Destroy();
            }
        }

        [Test]
        public void TestNodeCount ()
        {
            using (FbxScene newScene = FbxScene.Create (m_fbxManager, ""))
            {
                Assert.GreaterOrEqual (newScene.GetNodeCount (), 0);
            }
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroyedZombie ()
        {
            FbxScene zombieScene;

            using (FbxScene newScene = FbxScene.Create (m_fbxManager, ""))
            {
                zombieScene = newScene;

                newScene.Destroy();

                // Here we get an ANE because zombieScene is destroyed.
                Assert.GreaterOrEqual (zombieScene.GetNodeCount (), 0);
            }
        }

        [Test]
        [ExpectedException (typeof(System.NullReferenceException))]
        public void TestDisposedZombie ()
        {
            FbxScene zombieScene;

            using (FbxScene newScene = FbxScene.Create (m_fbxManager, ""))
            {
                zombieScene = newScene;
            }
            // Here we get an NRE because zombieScene is disposed.
            Assert.GreaterOrEqual (zombieScene.GetNodeCount (), 0);
        }
    }
}
