using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using FbxSdk;

namespace UnitTests
{
    public class FbxIOPluginRegistryTest
    {
        #if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage ()
        {
            CoverageTester.TestCoverage (typeof(FbxIOPluginRegistry), this.GetType ());
        }
        #endif

        [Test]
        public void TestBasics ()
        {
            using (FbxManager manager = FbxManager.Create ()) {
                int fileFormat = manager.GetIOPluginRegistry ().FindWriterIDByDescription ("FBX ascii (*.fbx)");
                Assert.GreaterOrEqual (fileFormat, 0); // just check that it is something other than -1

                // test an invalid format
                fileFormat = manager.GetIOPluginRegistry ().FindWriterIDByDescription ("invalid format");
                Assert.AreEqual (-1, fileFormat);

                // test null
                Assert.That (() => { manager.GetIOPluginRegistry ().FindWriterIDByDescription (null); }, Throws.Exception.TypeOf<System.NullReferenceException>());

                // test dispose
                // TODO: Dispose doesn't really seem useful here, should we do anything about it?
                manager.GetIOPluginRegistry ().Dispose ();
                fileFormat = manager.GetIOPluginRegistry ().FindWriterIDByDescription ("invalid format");
                Assert.AreEqual (-1, fileFormat);
            }
        }
    }
}