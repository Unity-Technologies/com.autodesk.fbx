using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxSceneTest : Base<FbxScene>
    {
        [Test]
        public void TestNodeCount ()
        {
            using (FbxScene newScene = FbxScene.Create (FbxManager, ""))
            {
                Assert.GreaterOrEqual (newScene.GetNodeCount (), 0);
            }
        }
    }
}
