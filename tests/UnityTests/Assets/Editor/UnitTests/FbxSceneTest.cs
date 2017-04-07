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
                Assert.IsInstanceOf<FbxScene> (newScene);
                Assert.IsInstanceOf<FbxDocument> (newScene);
                Assert.IsInstanceOf<FbxObject> (newScene);
                Assert.IsInstanceOf<FbxEmitter> (newScene);

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
        public void TestZombie1 ()
        {
            FbxScene zombieScene;

            using (FbxScene newScene = FbxScene.Create (m_fbxManager, ""))
            {
                zombieScene = newScene;

                newScene.Destroy();

                Assert.GreaterOrEqual (zombieScene.GetNodeCount (), 0);
            }
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        [Ignore("CRASHES accessing zombie")]
        public void TestZombie2 ()
        {
            FbxScene zombieScene;

            using (FbxScene newScene = FbxScene.Create (m_fbxManager, ""))
            {
                zombieScene = newScene;
            }
            Assert.GreaterOrEqual (zombieScene.GetNodeCount (), 0);
        }
    }
}
