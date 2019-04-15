using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Autodesk.Fbx;
using System.IO;

public class FbxExporterTest
{
    private static string s_fbxFilePath;
    
    [OneTimeSetUp]
    public static void OneTimeSetUp()
    {
        // Build the fbx scene file path 
        // (player/player_data/emptySceneFromRuntime.fbx)
        s_fbxFilePath = Path.Combine(Application.dataPath, "emptySceneFromRuntime.fbx");
    }
    
    [Test]
    public void TestWriteEmptyFbxFile() {
        /*
        Runtime test that writes an fbx scene file in the directory where the 
        player is (temp folder while running tests)
        */

        // The file should not exist. We are running the test from the Test 
        // Runner, which should always create a new player with its own fresh 
        // data directory
        FileInfo fbxFileInfo = new FileInfo(s_fbxFilePath);
        Assert.That(!fbxFileInfo.Exists, string.Format("\"{0}\" already exists but the test did not create it yet", s_fbxFilePath));

        using (var fbxManager = FbxManager.Create())
        {
            FbxIOSettings fbxIOSettings = FbxIOSettings.Create(fbxManager, Globals.IOSROOT);

            // Configure the IO settings.
            fbxManager.SetIOSettings(fbxIOSettings);

            // Create the exporter 
            var fbxExporter = FbxExporter.Create(fbxManager, "Exporter");

            // Initialize the exporter.
            int fileFormat = fbxManager.GetIOPluginRegistry().FindWriterIDByDescription("FBX ascii (*.fbx)");
            bool status = fbxExporter.Initialize(s_fbxFilePath, fileFormat, fbxIOSettings);

            Assert.That( status, string.Format("failed to initialize exporter, reason:D {0}",
                                               fbxExporter.GetStatus().GetErrorString()));
            // Create a scene
            var fbxScene = FbxScene.Create(fbxManager, "Scene");

            // create scene info
            FbxDocumentInfo fbxSceneInfo = FbxDocumentInfo.Create(fbxManager, "SceneInfo");

            // set some scene info values
            fbxSceneInfo.mTitle = "emptySceneFromRuntime";
            fbxSceneInfo.mSubject = "Exported from a Unity runtime while testing in play mode";
            fbxSceneInfo.mAuthor = "Unity Technologies";
            fbxSceneInfo.mRevision = "1.0";
            fbxSceneInfo.mKeywords = "export fbx runtime player play mode";
            fbxSceneInfo.mComment = "This is to test the capability of exporting from a Unity runtime, using the FBX SDK C# bindings";

            fbxScene.SetSceneInfo(fbxSceneInfo);

            // Export the scene to the file.
            status = fbxExporter.Export(fbxScene);
            Assert.That( status, string.Format("Failed to export scene, reason: {0}",
                                               fbxExporter.GetStatus().GetErrorString()));

            // cleanup
            fbxScene.Destroy();
            fbxExporter.Destroy();
        }

        // Test that the file exists
        fbxFileInfo = new FileInfo(s_fbxFilePath);
        Assert.That(fbxFileInfo.Exists, string.Format("\"{0}\" was not created", s_fbxFilePath));
    }

    [TearDown]
    public void TearDown()
    {
        File.Delete(s_fbxFilePath);
    }
}
