using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{

    public class FbxObjectTest : Base<FbxObject>
    {
        [Test]
        public void TestNames ()
        {
            /*
             * We use this also for testing that string handling works.
             * Make sure we can pass const char*, FbxString, and const
             * FbxString&.
             * Make sure we can return those too (though I'm not actually
             * seeing a return of a const-ref anywhere).
             */

            // Test a function that takes const char*.
            FbxObject obj = FbxObject.Create(Manager, "MyObject");
            Assert.IsNotNull (obj);

            // Test a function that returns const char*.
            Assert.AreEqual ("MyObject", obj.GetName ());

            // Test a function that takes an FbxString with an accent in it.
            obj.SetNameSpace("Accentué");

            // Test a function that returns FbxString.
            Assert.AreEqual ("MyObject", obj.GetNameWithoutNameSpacePrefix ());

            // Test a function that returns FbxString with an accent in it.
            Assert.AreEqual ("Accentué", obj.GetNameSpaceOnly());

            // Test a function that takes a const char* and returns an FbxString.
            // We don't want to convert the other StripPrefix functions, which
            // modify their argument in-place.
            Assert.AreEqual("MyObject", FbxObject.StripPrefix("NameSpace::MyObject"));

            obj.Destroy();
        }

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
