using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{

    public class FbxObjectTest : Base<FbxObject>
    {
        [Test]
        public void TestUTF8()
        {
            // make sure japanese survives the round-trip.
            string katakana = "片仮名";
            FbxObject obj = FbxObject.Create(Manager, katakana);
            Assert.AreEqual(katakana, obj.GetName());
        }

        [Test]
        public void TestFindClass ()
        {
            FbxClassId classId = Manager.FindClass ("FbxObject");

            Assert.AreEqual (classId.GetName (), "FbxObject");
        }

        [Test]
        public void TestFbxManager ()
        {
            using (FbxObject obj = FbxObject.Create (Manager, "")) {
                FbxManager fbxManager2 = obj.GetFbxManager();
                Assert.IsNotNull(fbxManager2);
            }
        }
    }
}
