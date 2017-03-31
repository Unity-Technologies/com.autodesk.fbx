using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;
using System.IO;

namespace UseCaseTests
{

    public class EmptyExportTest
    {

        [Test]
        public void EditorTest ()
        {
            // Create the FBX SDK manager
            FbxManager lSdkManager = FbxManager.Create ();

            // Create an IOSettings object.
            FbxIOSettings ios = FbxIOSettings.Create (lSdkManager, Globals.IOSROOT);
            lSdkManager.SetIOSettings (ios);

            // Create an exporter.
            FbxExporter lExporter = FbxExporter.Create (lSdkManager, "");

            // Declare the path and filename of the file to which the scene will be exported.
            // In this case, the file will be in the same directory as the executable.
            string lFilename = "test.fbx";

            // Initialize the exporter.
            bool lExportStatus = lExporter.Initialize (lFilename, -1, lSdkManager.GetIOSettings ());

            // Check that export status is True
            Assert.IsTrue (lExportStatus);

            // Create a new scene so it can be populated by the imported file.
            FbxScene lScene = FbxScene.Create (lSdkManager, "myScene");

            // Export the scene to the file.
            lExporter.Export (lScene);

            // Check if file exists
            Assert.IsTrue (File.Exists (lFilename));

            lExporter.Destroy ();

            // Import to make sure file is valid

            // Create an importer.
            FbxImporter lImporter = FbxImporter.Create (lSdkManager, "");

            // Initialize the importer.
            bool lImportStatus = lImporter.Initialize (lFilename, -1, lSdkManager.GetIOSettings ());

            Assert.IsTrue (lImportStatus);

            // Create a new scene so it can be populated by the imported file.
            FbxScene newScene = FbxScene.Create (lSdkManager, "myScene2");

            // Import the contents of the file into the scene.
            lImporter.Import (newScene);

            lImporter.Destroy ();

            // check that the scene is valid
            Assert.GreaterOrEqual (newScene.GetGenericNodeCount (), 0);

            lSdkManager.Destroy ();

            // Delete the file once the test is complete
            File.Delete (lFilename);
        }
    }
}