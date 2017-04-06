using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;
using System.IO;

namespace UnitTests
{

    public class FbxExporterTest
    {
        FbxManager m_fbxManager;
        FbxExporter m_exporter;

        string m_testFolder;

        [SetUp]
        public void InitBeforeTest()
        {
            m_fbxManager = FbxManager.Create ();
            m_exporter = FbxExporter.Create (m_fbxManager, "exporter");

            Assert.IsNotNull (m_exporter);

            DirectoryInfo tempDir = Directory.CreateDirectory("to_delete");

            m_testFolder = tempDir.FullName;
        }

        [TearDown]
        public void CleanupAfterTest()
        {
            try{
                m_exporter.Destroy();
                m_fbxManager.Destroy();
            }
            catch(System.ArgumentNullException){
                // already destroyed in test
            }

            // delete all files that were created
            Directory.Delete(m_testFolder, true);
        }

        [Test]
        public void TestExportEmptyFbxDocument ()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = Path.Combine(m_testFolder, "TestExportEmptyFbxDocument.fbx");

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, -1, m_fbxManager.GetIOSettings());

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }


        [Test]
        public void TestExportNull ()
        {
            string filename = Path.Combine(m_testFolder, "TestExportNull.fbx");

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, -1, m_fbxManager.GetIOSettings());

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (null);

            Assert.IsFalse (status);

            // FbxSdk creates an empty file even though the export status was false
            Assert.IsTrue (File.Exists (filename));
        }
            
        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroy ()
        {
            m_exporter.Destroy ();
            m_exporter.GetName ();
        }

        [Test]
        [ExpectedException (typeof(System.ArgumentNullException))]
        public void TestDestroyManager ()
        {
            m_fbxManager.Destroy ();
            m_exporter.GetName ();
        }

        [Test]
        public void TestInitializeInvalidFilenameOnly()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = Path.Combine(m_testFolder, "TestInitializeInvalidFilenameOnly.foo");

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);

            // FbxSdk doesn't create a file in this situation
            Assert.IsFalse (File.Exists (filename));
        }

        [Test]
        public void TestInitializeValidFilenameOnly()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = Path.Combine(m_testFolder, "TestInitializeValidFilenameOnly.fbx");

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeInvalidFileFormat()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = Path.Combine(m_testFolder, "TestInitializeInvalidFileFormat.fbx");

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, int.MinValue);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            // looks like anything less than 0 is treated the same as -1
            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeInvalidFileFormat2()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = Path.Combine(m_testFolder, "TestInitializeInvalidFileFormat2.fbx");

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, int.MaxValue);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsFalse (status);
            Assert.IsFalse (File.Exists (filename));
        }

        [Test]
        public void TestInitializeValidFileFormat()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = Path.Combine(m_testFolder, "TestInitializeValidFileFormat.fbx");

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, 1);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeNullIOSettings()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = Path.Combine(m_testFolder, "TestInitializeNullIOSettings.fbx");

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, -1, null);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        [Ignore("Crashes Unity when passed null FbxManager to FbxIOSettings")]
        public void TestInitializeInvalidIOSettings()
        {
            FbxDocument emptyDoc = FbxDocument.Create (m_fbxManager, "empty");

            string filename = Path.Combine(m_testFolder, "TestInitializeInvalidIOSettings.fbx");

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, -1, FbxIOSettings.Create(null, ""));

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }
    }
}