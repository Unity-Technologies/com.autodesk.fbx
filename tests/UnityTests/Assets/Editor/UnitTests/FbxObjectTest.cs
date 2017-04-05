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
            
            obj.Destroy();
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
        public void TestCallDestroyed ()
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