using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{

    public class FbxObjectTest
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
        public void TestCreateDestroy ()
        {
            FbxObject obj = FbxObject.Create(m_fbxManager, "MyObject");
            Assert.IsNotNull (obj);
            
            // Destroy just the object.
            obj.Destroy();
        }

        [Test]
        public void TestCreateDestroyRecursive ()
        {
            FbxObject obj = FbxObject.Create(m_fbxManager, "MyObject");
            Assert.IsNotNull (obj);
            
            // Destroy object and its children (though we didn't create any).
            obj.Destroy(true);
        }

        [Test]
        [ExpectedException( typeof( System.NullReferenceException ) )]
        public void TestCreateNullManager ()
        {
            // This caused a crash at one point.
            FbxObject obj = FbxObject.Create(null, "MyObject");
            Assert.IsNotNull (obj);
            obj.Destroy();
        }

        [Test]
        public void TestCreateNullName ()
        {
            FbxObject obj = FbxObject.Create(m_fbxManager, null);
            Assert.IsNotNull (obj);
            obj.Destroy();
        }
        
        [Test]
        [ExpectedException( typeof( System.ArgumentNullException ) )]
        public void TestCreateZombieManager ()
        {
            // This caused a crash at one point: using a zombie manager.
            var manager = FbxManager.Create();
            manager.Destroy();

            FbxObject obj = FbxObject.Create(manager, null);
            Assert.IsNotNull (obj);
            obj.Destroy();
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
            FbxObject obj = FbxObject.Create(m_fbxManager, "MyObject");
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
        public void TestDispose()
        {
            using(var obj = FbxObject.Create(m_fbxManager, "")) {
            }
        }

        [Test]
        public void TestUTF8()
        {
            // make sure japanese survives the round-trip.
            string katakana = "片仮名";
            using(var obj = FbxObject.Create(m_fbxManager, katakana)) {
                Assert.AreEqual(katakana, obj.GetName());
            }
        }

        [Test]
        [ExpectedException( typeof( System.ArgumentNullException ) )]
        public void TestZombie ()
        {
            FbxObject obj = FbxObject.Create(m_fbxManager, "MyObject");
            Assert.IsNotNull (obj);

            obj.Destroy();
            obj.GetName();
        }
        
        [Test]
        public void TestFindClass ()
        {
            FbxClassId classId = m_fbxManager.FindClass ("FbxObject");

            Assert.AreEqual (classId.GetName (), "FbxObject");
        }

        [Test]
        public void TestSelected ()
        {
            FbxObject obj = FbxObject.Create (m_fbxManager, "MyObject");
            Assert.IsNotNull (obj);

            Assert.IsFalse( obj.GetSelected () );
            obj.SetSelected (true);
            Assert.IsTrue (obj.GetSelected ());

            obj.Destroy ();
        }
    }
}
