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
                Assert.IsInstanceOf<FbxObject> (newScene);

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
        public void TestZombie1 ()
        {
            FbxScene zombieScene;

            using (FbxScene newScene = FbxScene.Create (FbxManager, ""))
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

            using (FbxScene newScene = FbxScene.Create (FbxManager, ""))
            {
                zombieScene = newScene;
            }
            Assert.GreaterOrEqual (zombieScene.GetNodeCount (), 0);
        }
    }
}