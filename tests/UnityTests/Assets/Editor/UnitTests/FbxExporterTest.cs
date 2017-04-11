using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;
using System.IO;

namespace UnitTests
{

    public class FbxExporterTest : Base<FbxExporter>
    {
        FbxExporter m_exporter;

        string m_testFolderPrefix = "to_delete_";
        string m_testFolder;

        private string GetRandomDirectory()
        {
            string randomDir = Path.Combine(Path.GetTempPath(), m_testFolderPrefix);

            string temp;
            do {
                // check that the m_fbxManageres not already exist
                temp = randomDir + Path.GetRandomFileName ();
            } while(Directory.Exists (temp));

            return temp;
        }

        private string GetRandomFilename(string path, bool fbxExtension = true)
        {
            string temp;
            do {
                // check that the directory does not already exist
                temp = Path.Combine (path, Path.GetRandomFileName ());

                if(fbxExtension){
                    temp = Path.ChangeExtension(temp, ".fbx");
                }

            } while(File.Exists (temp));

            return temp;
        }

        public override void InitTest()
        {
            base.InitTest ();

            m_exporter = FbxExporter.Create (FbxManager, "exporter");

            Assert.IsNotNull (m_exporter);

            var testDirectories = Directory.GetDirectories(Path.GetTempPath(), m_testFolderPrefix + "*");

            foreach (var directory in testDirectories)
            {
                Directory.Delete(directory, true);
            }

            m_testFolder = GetRandomDirectory ();
            Directory.CreateDirectory (m_testFolder);
        }

        public override void DestroyTest()
        {
            try{
                m_exporter.Destroy();
            }
            catch(System.ArgumentNullException){
                // already destroyed in test
            }

            base.DestroyTest ();

            // delete all files that were created
            Directory.Delete(m_testFolder, true);
        }

        [Test]
        public void TestExportEmptyFbxDocument ()
        {
            FbxDocument emptyDoc = FbxDocument.Create (FbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, -1, FbxManager.GetIOSettings());

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }


        [Test]
        public void TestExportNull ()
        {
            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter.
            bool exportStatus = m_exporter.Initialize (filename, -1, FbxManager.GetIOSettings());

            Assert.IsTrue (exportStatus);

            // Export a null document. This is documented to fail.
            bool status = m_exporter.Export (null);

            Assert.IsFalse (status);

            // FbxSdk creates an empty file even though the export status was false
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeInvalidFilenameOnly()
        {
            FbxDocument emptyDoc = FbxDocument.Create (FbxManager, "empty");

            // Build the filename without the extension.
            string filename = GetRandomFilename (m_testFolder, false);

            // Initialize the exporter. Use default file type and IO settings.
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
            FbxDocument emptyDoc = FbxDocument.Create (FbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter. Use default file type and IO settings.
            bool exportStatus = m_exporter.Initialize (filename);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeFileFormatNegative()
        {
            FbxDocument emptyDoc = FbxDocument.Create (FbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter. Pass it a negative file format different than -1.
            bool exportStatus = m_exporter.Initialize (filename, int.MinValue);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeFileFormatInvalid()
        {
            FbxDocument emptyDoc = FbxDocument.Create (FbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter. Pass it a file format that's not valid.
            bool exportStatus = m_exporter.Initialize (filename, int.MaxValue);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsFalse (status);
            Assert.IsFalse (File.Exists (filename));
        }

        [Test]
        public void TestInitializeValidFileFormat()
        {
            FbxDocument emptyDoc = FbxDocument.Create (FbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter. Use a valid non-default file format.
            bool exportStatus = m_exporter.Initialize (filename, 1);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        public void TestInitializeNullIOSettings()
        {
            FbxDocument emptyDoc = FbxDocument.Create (FbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter with explicit null IO settings (which is
            // also the default).
            bool exportStatus = m_exporter.Initialize (filename, -1, null);

            Assert.IsTrue (exportStatus);

            bool status = m_exporter.Export (emptyDoc);

            Assert.IsTrue (status);
            Assert.IsTrue (File.Exists (filename));
        }

        [Test]
        [ExpectedException( typeof( System.ArgumentNullException ) )]
        public void TestInitializeInvalidIOSettings()
        {
            FbxDocument emptyDoc = FbxDocument.Create (FbxManager, "empty");

            string filename = GetRandomFilename (m_testFolder);

            // Initialize the exporter. Pass it zombie IO settings.
            var ioSettings = FbxIOSettings.Create(FbxManager, "");
            ioSettings.Destroy();
            m_exporter.Initialize (filename, -1, ioSettings);
        }
    }
}
