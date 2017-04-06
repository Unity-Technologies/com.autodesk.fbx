using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;
using System.IO;

namespace UnitTests
{

    public class FbxExporterTest
    {
        FbxManager manager;
        FbxExporter exporter;

        string testFolder;

        [SetUp]
        public void InitBeforeTest()
        {
            manager = FbxManager.Create ();
            exporter = FbxExporter.Create (manager, "exporter");

            Assert.IsNotNull (exporter);

            DirectoryInfo tempDir = Directory.CreateDirectory("to_delete");

            testFolder = tempDir.FullName;
        }

        [TearDown]
        public void CleanupAfterTest()
        {
            try{
                exporter.Destroy();
                manager.Destroy();
            }
            catch(System.ArgumentNullException){
                // already destroyed in test
            }

            // delete all files that were created
            Directory.Delete(testFolder, true);
        }

        [Test]
        public void TestExportEmptyFbxDocument ()
        {
            FbxDocument emptyDoc = FbxDocument.Create (manager, "empty");

            string filename = Path.Combine(testFolder, "TestExportEmptyFbxDocument.fbx");

            // Initialize the exporter.
            bool exportStatus = exporter.Initialize (filename, -1, manager.GetIOSettings());

            Assert.IsTrue (exportStatus);

            bool status = exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }


        [Test]
        public void TestExportNull ()
        {
            string filename = Path.Combine(testFolder, "TestExportNull.fbx");

            // Initialize the exporter.
            bool exportStatus = exporter.Initialize (filename, -1, manager.GetIOSettings());

            Assert.IsTrue (exportStatus);

            bool status = exporter.Export (null);

            Assert.IsFalse (status);

            // FbxSdk creates an empty file even though the export status was false
            Assert.IsTrue (File.Exists (filename));
        }
            
        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroy ()
        {
            exporter.Destroy ();
            exporter.GetName ();
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroyManager ()
        {
            manager.Destroy ();
            exporter.GetName ();
        }

        [Test]
        public void TestInitializeInvalidFilenameOnly()
        {
            FbxDocument emptyDoc = FbxDocument.Create (manager, "empty");

            string filename = Path.Combine(testFolder, "TestInitializeInvalidFilenameOnly.foo");

            // Initialize the exporter.
            bool exportStatus = exporter.Initialize (filename);

            Assert.IsTrue (exportStatus);

            bool status = exporter.Export (emptyDoc);

            Assert.IsTrue (status);

            // FbxSdk doesn't create a file in this situation
            Assert.IsFalse (File.Exists (filename));
        }

        [Test]
        public void TestInitializeValidFilenameOnly()
        {
            FbxDocument emptyDoc = FbxDocument.Create (manager, "empty");

            string filename = Path.Combine(testFolder, "TestInitializeValidFilenameOnly.fbx");

            // Initialize the exporter.
            bool exportStatus = exporter.Initialize (filename);

            Assert.IsTrue (exportStatus);

            bool status = exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeInvalidFileFormat()
        {
            FbxDocument emptyDoc = FbxDocument.Create (manager, "empty");

            string filename = Path.Combine(testFolder, "TestInitializeInvalidFileFormat.fbx");

            // Initialize the exporter.
            bool exportStatus = exporter.Initialize (filename, int.MinValue);

            Assert.IsTrue (exportStatus);

            bool status = exporter.Export (emptyDoc);

            // looks like anything less than 0 is treated the same as -1
            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeInvalidFileFormat2()
        {
            FbxDocument emptyDoc = FbxDocument.Create (manager, "empty");

            string filename = Path.Combine(testFolder, "TestInitializeInvalidFileFormat2.fbx");

            // Initialize the exporter.
            bool exportStatus = exporter.Initialize (filename, int.MaxValue);

            Assert.IsTrue (exportStatus);

            bool status = exporter.Export (emptyDoc);

            Assert.IsFalse (status);
            Assert.IsFalse (File.Exists (filename));
        }

        [Test]
        public void TestInitializeValidFileFormat()
        {
            FbxDocument emptyDoc = FbxDocument.Create (manager, "empty");

            string filename = Path.Combine(testFolder, "TestInitializeValidFileFormat.fbx");

            // Initialize the exporter.
            bool exportStatus = exporter.Initialize (filename, 1);

            Assert.IsTrue (exportStatus);

            bool status = exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeNullIOSettings()
        {
            FbxDocument emptyDoc = FbxDocument.Create (manager, "empty");

            string filename = Path.Combine(testFolder, "TestInitializeNullIOSettings.fbx");

            // Initialize the exporter.
            bool exportStatus = exporter.Initialize (filename, -1, null);

            Assert.IsTrue (exportStatus);

            bool status = exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        [Ignore("Crashes Unity")]
        public void TestInitializeInvalidIOSettings()
        {
            FbxDocument emptyDoc = FbxDocument.Create (manager, "empty");

            string filename = Path.Combine(testFolder, "TestInitializeInvalidIOSettings.fbx");

            // Initialize the exporter.
            bool exportStatus = exporter.Initialize (filename, -1, FbxIOSettings.Create(null, ""));

            Assert.IsTrue (exportStatus);

            bool status = exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }
    }
}