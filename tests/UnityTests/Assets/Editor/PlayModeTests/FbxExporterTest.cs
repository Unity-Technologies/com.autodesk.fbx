using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Unity.FbxSdk;

public class FbxExporterTest {
    [Test]
    public void TestWriteEmptyFbxFile() {
        // Hack so the test can resolve UnityFbxSdkNative.dll
//        string originalPath = System.Environment.GetEnvironmentVariable("PATH");
  //      System.Environment.SetEnvironmentVariable("PATH", originalPath + @";D:\projects\FbxSharpBuild\tests\UnityTests\Assets\FbxSdk\Plugins\x64\Windows");

    //    Debug.Log(System.Environment.GetEnvironmentVariable("PATH"));
        using (var fbxManager = FbxManager.Create())
        {
            FbxIOSettings fbxIOSettings = FbxIOSettings.Create(fbxManager, Globals.IOSROOT);

            // Configure the IO settings.
            fbxManager.SetIOSettings(fbxIOSettings);

            // Create the exporter 
            var fbxExporter = FbxExporter.Create(fbxManager, "Exporter");

            // Initialize the exporter.
            int fileFormat = fbxManager.GetIOPluginRegistry().FindWriterIDByDescription("FBX ascii (*.fbx)");

            bool status = fbxExporter.Initialize("d:\\temp\\fromRuntime.fbx", fileFormat, fbxIOSettings);
            // Check that initialization of the fbxExporter was successful
            if (!status)
            {
                Debug.LogError(string.Format("failed to initialize exporter, reason:D {0}",
                                               fbxExporter.GetStatus().GetErrorString()));
                return;
            }

            // Create a scene
            var fbxScene = FbxScene.Create(fbxManager, "Scene");

            // create scene info
            FbxDocumentInfo fbxSceneInfo = FbxDocumentInfo.Create(fbxManager, "SceneInfo");

            // set some scene info values
            fbxSceneInfo.mTitle = "fromRuntime";
            fbxSceneInfo.mSubject = "Exported from a Unity runtime";
            fbxSceneInfo.mAuthor = "Unity Technologies";
            fbxSceneInfo.mRevision = "1.0";
            fbxSceneInfo.mKeywords = "export runtime";
            fbxSceneInfo.mComment = "This is to demonstrate the capability of exporting from a Unity runtime, using the FBX SDK C# bindings";

            fbxScene.SetSceneInfo(fbxSceneInfo);

            // Export the scene to the file.
            status = fbxExporter.Export(fbxScene);

            // cleanup
            fbxScene.Destroy();
            fbxExporter.Destroy();
        }

      //  System.Environment.SetEnvironmentVariable("PATH", originalPath);
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    /*
        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses() {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    */
}
