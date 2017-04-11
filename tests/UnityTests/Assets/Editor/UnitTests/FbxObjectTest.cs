using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{

    public class FbxObjectTest : Base
    {
        protected override FbxObject CreateObject ()
        {
            return FbxObject.Create (FbxManager, "");
        }

        [Test]
        [Ignore("CRASHES handling null FbxManager")]
        public void TestCreateDestroy2 ()
        {
            FbxObject obj = FbxObject.Create(null, "MyObject");
            Assert.IsNotNull (obj);
            
            // there are two destroy methods
            obj.Destroy(true);
        }

        [Test]
        public void TestCreateDestroy3 ()
        {
            FbxObject obj = FbxObject.Create(FbxManager, null);
            Assert.IsNotNull (obj);
            
            // there are two destroy methods
            obj.Destroy(true);
        }
        
        [Test]
        [Ignore("CRASHES handling zombie FbxManager")]
        public void TestCreateDestroy4 ()
        {
            FbxManager.Destroy();
            
            FbxObject obj = FbxObject.Create(FbxManager, null);
            Assert.IsNotNull (obj);
            
            // there are two destroy methods
            obj.Destroy(true);
        }

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
            FbxObject obj = FbxObject.Create(FbxManager, "MyObject");
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
            FbxObject obj = FbxObject.Create(FbxManager, katakana);
            Assert.AreEqual(katakana, obj.GetName());
        }

        [Test]
        public void TestFindClass ()
        {
            FbxClassId classId = FbxManager.FindClass ("FbxObject");

            Assert.AreEqual (classId.GetName (), "FbxObject");
        }

        [Test]
        public void TestSelected ()
        {
            FbxObject obj = FbxObject.Create (FbxManager, "MyObject");
            Assert.IsNotNull (obj);

            Assert.IsFalse( obj.GetSelected () );
            obj.SetSelected (true);
            Assert.IsTrue (obj.GetSelected ());

            obj.Destroy ();
        }

        [Test]
        public void TestFbxManager ()
        {
            using (FbxObject obj = FbxObject.Create (FbxManager, "")) {
                FbxManager fbxManager2 = obj.GetFbxManager();
                Assert.IsNotNull(fbxManager2);
            }
        }
    }
}
