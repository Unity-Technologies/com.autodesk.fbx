using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{

    public class FbxIOSettingsTest : Base
    {

        protected override FbxObject CreateObject ()
        {
            return FbxIOSettings.Create (FbxManager, "");
        }

        [Test]
        public void TestCreate ()
        {
            FbxIOSettings ioSettings = FbxIOSettings.Create (FbxManager, "");

            Assert.IsNotNull (ioSettings);
            Assert.IsInstanceOf<FbxObject> (ioSettings);
        }

        [Test]
        [Ignore("Crashes because we try to delete the FbxManager twice (doesn't give ArgumentNullException)")]
        public void TestDestroyed ()
        {
            FbxIOSettings ioSettings = FbxIOSettings.Create (FbxManager, "");

            Assert.IsNotNull (ioSettings);
            Assert.IsInstanceOf<FbxObject> (ioSettings);

            FbxManager.Destroy ();

            Assert.That (() => { ioSettings.GetName (); }, Throws.Exception.TypeOf<System.ArgumentNullException>()); 
        }

        [Test]
        public void TestFVirtual ()
        {
            // Test the swig -fvirtual flag works properly: we can call virtual
            // functions defined on the base class without the function also
            // being defined in the subclass.

            FbxManager manager = FbxManager.Create ();
            FbxIOSettings ioSettings = FbxIOSettings.Create (manager, "");

            // GetSelected is a virtual method inherited from FbxObject
            Assert.IsFalse (ioSettings.GetSelected ());
            ioSettings.SetSelected (true);
            Assert.IsTrue (ioSettings.GetSelected ());

            ioSettings.Destroy ();
            manager.Destroy ();
        }
    }


}
