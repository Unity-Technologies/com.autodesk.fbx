using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{

    public class FbxIOSettingsTest : FbxSdkTestBase
    {

        [Test]
        public void TestCreate ()
        {
            FbxIOSettings ioSettings = FbxIOSettings.Create (FbxManager, "");

            Assert.IsNotNull (ioSettings);
            Assert.IsInstanceOf<FbxObject> (ioSettings);
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        [Ignore("Crashes because we try to delete the FbxManager twice (doesn't give ArgumentNullException)")]
        public void TestDestroyed ()
        {
            FbxIOSettings ioSettings = FbxIOSettings.Create (FbxManager, "");

            Assert.IsNotNull (ioSettings);
            Assert.IsInstanceOf<FbxObject> (ioSettings);

            FbxManager.Destroy ();

            ioSettings.GetName ();
        }

        [Test]
        public void TestFVirtual ()
        {
            FbxIOSettings ioSettings = FbxIOSettings.Create (FbxManager, "");

            // GetSelected is a virtual method inherited from FbxObject
            Assert.IsFalse( ioSettings.GetSelected () );
            ioSettings.SetSelected (true);
            Assert.IsTrue (ioSettings.GetSelected ());

            ioSettings.Destroy ();
        }
    }
    
}