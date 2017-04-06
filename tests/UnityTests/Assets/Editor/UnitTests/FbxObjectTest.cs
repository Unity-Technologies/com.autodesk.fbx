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
            
            // there are two destroy methods
            obj.Destroy(true);
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
            FbxObject obj = FbxObject.Create(m_fbxManager, null);
            Assert.IsNotNull (obj);
            
            // there are two destroy methods
            obj.Destroy(true);
        }
        
        [Test]
        [Ignore("CRASHES handling zombie FbxManager")]
        public void TestCreateDestroy4 ()
        {
            m_fbxManager.Destroy();
            
            FbxObject obj = FbxObject.Create(m_fbxManager, null);
            Assert.IsNotNull (obj);
            
            // there are two destroy methods
            obj.Destroy(true);
        }

        [Test]
        public void TestGetName ()
        {
            FbxObject obj = FbxObject.Create(m_fbxManager, "MyObject");
            Assert.IsNotNull (obj);
        
            Assert.AreEqual (obj.GetName (), "MyObject");
            
            obj.Destroy();
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
    }
}