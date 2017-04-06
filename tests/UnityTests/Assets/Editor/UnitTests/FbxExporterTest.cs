using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;
using System.IO;

namespace UnitTests
{

    public class FbxExporterTest
    {


        public void ExportHelper (FbxManager manager, FbxDocument doc)
        {
            FbxExporter exporter = FbxExporter.Create (manager, "exporter");

            Assert.IsNotNull (exporter);

            string filename = "test.fbx";

            // Initialize the exporter.
            bool exportStatus = exporter.Initialize (filename, -1, manager.GetIOSettings ());

            Assert.IsTrue (exportStatus);

            exporter.Export (doc);

            Assert.IsTrue (File.Exists (filename));

            exporter.Destroy ();

            // Delete the file once the test is complete
            File.Delete (filename);
        }

        [Test]
        public void TestExportEmptyFbxDocument ()
        {
            FbxManager manager = FbxManager.Create ();
            FbxDocument emptyDoc = FbxDocument.Create (manager, "empty");

            ExportHelper (manager, emptyDoc);

            manager.Destroy ();
        }

        [Test]
        public void TestExportNull ()
        {
            FbxManager manager = FbxManager.Create ();

            ExportHelper (manager, null);

            manager.Destroy ();
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroy ()
        {
            FbxManager manager = FbxManager.Create ();
            FbxExporter exporter = FbxExporter.Create (manager, "exporter");

            exporter.Destroy ();

            exporter.GetName ();

            manager.Destroy ();
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroyManager ()
        {
            FbxManager manager = FbxManager.Create ();
            FbxExporter exporter = FbxExporter.Create (manager, "exporter");

            manager.Destroy ();
            exporter.GetName ();
        }
    }
}