using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{

    public class FbxIOSettingsTest
    {

        [Test]
        public void TestCreate ()
        {
            FbxManager manager = FbxManager.Create ();
            FbxIOSettings ioSettings = FbxIOSettings.Create (manager, "");

            Assert.IsNotNull (ioSettings);
            Assert.IsInstanceOf<FbxObject> (ioSettings);

            manager.Destroy ();
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroyed ()
        {
            FbxManager manager = FbxManager.Create ();
            FbxIOSettings ioSettings = FbxIOSettings.Create (manager, "");

            Assert.IsNotNull (ioSettings);
            Assert.IsInstanceOf<FbxObject> (ioSettings);

            manager.Destroy ();

            ioSettings.GetName ();
        }
    }
}