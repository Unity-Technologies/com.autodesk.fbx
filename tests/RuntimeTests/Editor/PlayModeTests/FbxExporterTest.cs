using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Autodesk.Fbx;
using System.IO;

namespace Autodesk.Fbx.PlayModeTests
{
    internal class FbxExporterTest {
        [System.Runtime.InteropServices.DllImport("/usr/lib/libSystem.dylib")]
        static extern System.IntPtr dlopen(string path, int mode);
        [System.Runtime.InteropServices.DllImport("/usr/lib/libSystem.dylib")]
        static extern System.IntPtr dlerror();

        [Test, Order(0)]
        public void Diag() {
            try { FbxManager.Create().Destroy(); Debug.Log("DIAG first call ok"); }
            catch (System.Exception e) { Debug.Log("DIAG first call failed " + e.GetBaseException().Message); }
#if UNITY_EDITOR
            foreach (var imp in UnityEditor.PluginImporter.GetAllImporters()) {
                if (imp.assetPath.Contains("UnityFbxSdkNative"))
                    Debug.Log("DIAG importer " + imp.assetPath + " native=" + imp.isNativePlugin + " editor=" + imp.GetCompatibleWithEditor() + " os=" + imp.GetEditorData("OS") + " cpu=" + imp.GetEditorData("CPU"));
            }
            foreach (var d in System.IO.Directory.GetDirectories(System.IO.Path.GetFullPath("Packages/com.autodesk.fbx/Editor/Plugins"))) Debug.Log("DIAG dir " + d);
            var exe = System.IO.Path.GetFullPath("Packages/com.autodesk.fbx/Editor/Plugins/UnityFbxSdkNative.bundle/Contents/MacOS/UnityFbxSdkNative");
            Debug.Log("DIAG exe exists=" + System.IO.File.Exists(exe) + " " + exe);
            var h = dlopen(exe, 2);
            Debug.Log("DIAG dlopen=" + h + " err=" + (h == System.IntPtr.Zero ? System.Runtime.InteropServices.Marshal.PtrToStringAnsi(dlerror()) : ""));
            Debug.Log("DIAG playing=" + Application.isPlaying + " domainReload=" + UnityEditor.EditorSettings.enterPlayModeOptionsEnabled);
#endif
        }

        [Test, Order(1)]
        public void TestWriteEmptyFbxFile() {
            /*
            Runtime test that writes an fbx scene file in the directory where the 
            player is (temp folder while running tests)
            */

            // Build the fbx scene file path 
            // (player/player_data/emptySceneFromRuntime.fbx)
            string fbxFilePath = Application.dataPath;
            fbxFilePath = Path.Combine(fbxFilePath, "emptySceneFromRuntime.fbx");

            // The file should not exist. We are running the test from the Test 
            // Runner, which should always create a new player with its own fresh 
            // data directory
            FileInfo fbxFileInfo = new FileInfo(fbxFilePath);
            Assert.That(!fbxFileInfo.Exists, string.Format("\"{0}\" already exists but the test did not create it yet", fbxFilePath));

            using (var fbxManager = FbxManager.Create())
            {
                FbxIOSettings fbxIOSettings = FbxIOSettings.Create(fbxManager, Globals.IOSROOT);

                // Configure the IO settings.
                fbxManager.SetIOSettings(fbxIOSettings);

                // Create the exporter 
                var fbxExporter = FbxExporter.Create(fbxManager, "Exporter");

                // Initialize the exporter.
                int fileFormat = fbxManager.GetIOPluginRegistry().FindWriterIDByDescription("FBX ascii (*.fbx)");
                bool status = fbxExporter.Initialize(fbxFilePath, fileFormat, fbxIOSettings);

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
            fbxFileInfo = new FileInfo(fbxFilePath);
            Assert.That(fbxFileInfo.Exists, string.Format("\"{0}\" was not created", fbxFilePath));
        }
    }
}