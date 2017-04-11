using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxSceneTest : Base
    {

        protected override FbxObject CreateObject ()
        {
            return FbxScene.Create (FbxManager, "");
        }
        
        [Test]
        public void TestCreate ()
        {
            using (FbxScene newScene = FbxScene.Create (FbxManager, ""))
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
            using (FbxScene newScene = FbxScene.Create (FbxManager, ""))
            {
                Assert.GreaterOrEqual (newScene.GetNodeCount (), 0);
            }
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroyedZombie ()
        {
            FbxScene zombieScene;

            using (FbxScene newScene = FbxScene.Create (FbxManager, ""))
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

            using (FbxScene newScene = FbxScene.Create (FbxManager, ""))
            {
                zombieScene = newScene;
            }
            // Here we get an NRE because zombieScene is disposed.
            Assert.GreaterOrEqual (zombieScene.GetNodeCount (), 0);
        }
    }
}
