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

        static System.IntPtr DiagDllImportResolver(string name, System.Reflection.Assembly assembly, System.Runtime.InteropServices.DllImportSearchPath? searchPath) {
            Debug.Log("DIAG dllimport resolver " + name);
            return System.IntPtr.Zero;
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        static void DiagEarly() {
            Debug.Log("DIAG early init t=" + System.DateTime.Now.ToString("HH:mm:ss.fff"));
            System.AppDomain.CurrentDomain.FirstChanceException += (sender, args) => {
                var ex = args.Exception;
                if (ex is System.DllNotFoundException || (ex is System.TypeInitializationException && ex.Message.Contains("Autodesk")))
                    Debug.Log("DIAG firstchance t=" + System.DateTime.Now.ToString("HH:mm:ss.fff") + " thread=" + System.Threading.Thread.CurrentThread.ManagedThreadId + " " + ex.GetType().Name + ": " + ex.Message.Split('\n')[0] + "\n" + System.Environment.StackTrace);
            };
            try {
                var asm = typeof(FbxManager).Assembly;
                var alcT = typeof(object).Assembly.GetType("System.Runtime.Loader.AssemblyLoadContext");
                var alc = alcT.GetMethod("GetLoadContext").Invoke(null, new object[] { asm });
                System.Func<System.Reflection.Assembly, string, System.IntPtr> hookEvent = (a, n) => { Debug.Log("DIAG early event raised " + n + "\n" + System.Environment.StackTrace); return System.IntPtr.Zero; };
                alcT.GetEvent("ResolvingUnmanagedDll").AddEventHandler(alc, hookEvent);
                Debug.Log("DIAG early hooked alc=" + alc);
            }
            catch (System.Exception e) { Debug.Log("DIAG early hook failed " + e); }
        }
#endif

        [Test, Order(0)]
        public void Diag() {
            try {
                const System.Reflection.BindingFlags all0 = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                var asm0 = typeof(FbxManager).Assembly;
                var alc0 = typeof(object).Assembly.GetType("System.Runtime.Loader.AssemblyLoadContext").GetMethod("GetLoadContext").Invoke(null, new object[] { asm0 });
                var wrapper0 = alc0.GetType().GetProperty("UnityAssemblyLoadContextWrapper", all0).GetValue(alc0);
                var resolver0 = wrapper0.GetType().GetProperty("NativeLibraryResolver", all0).GetValue(wrapper0);
                var resolve0 = resolver0.GetType().GetMethod("Resolve", all0);
                for (int i = 0; i < 3; i++)
                    Debug.Log("DIAG pre-resolve " + i + " t=" + System.DateTime.Now.ToString("HH:mm:ss.fff") + " thread=" + System.Threading.Thread.CurrentThread.ManagedThreadId + " -> " + (resolve0.Invoke(resolver0, new object[] { asm0, "UnityFbxSdkNative" }) ?? "<null>"));
            }
            catch (System.Exception e) { Debug.Log("DIAG pre-resolve failed " + e); }
            try {
                var asm1 = typeof(FbxManager).Assembly;
                var alcT = typeof(object).Assembly.GetType("System.Runtime.Loader.AssemblyLoadContext");
                var alc1 = alcT.GetMethod("GetLoadContext").Invoke(null, new object[] { asm1 });
                var fld = alcT.GetField("_resolvingUnmanagedDll", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var del = fld?.GetValue(alc1) as System.Delegate;
                Debug.Log("DIAG event field=" + (fld != null) + " handlers=" + (del == null ? "0" : string.Join(",", System.Array.ConvertAll(del.GetInvocationList(), x => x.Method.DeclaringType + "." + x.Method.Name + " target=" + x.Target))));
                System.Func<System.Reflection.Assembly, string, System.IntPtr> hookEvent = (a, n) => { Debug.Log("DIAG event raised " + n + " for " + a.GetName().Name); return System.IntPtr.Zero; };
                alcT.GetEvent("ResolvingUnmanagedDll").AddEventHandler(alc1, hookEvent);
                var nl = typeof(object).Assembly.GetType("System.Runtime.InteropServices.NativeLibrary");
                var drT = typeof(object).Assembly.GetType("System.Runtime.InteropServices.DllImportResolver");
                var rd = System.Delegate.CreateDelegate(drT, typeof(FbxExporterTest).GetMethod("DiagDllImportResolver", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
                try { nl.GetMethod("SetDllImportResolver").Invoke(null, new object[] { asm1, rd }); Debug.Log("DIAG SetDllImportResolver ok"); }
                catch (System.Reflection.TargetInvocationException e) { Debug.Log("DIAG SetDllImportResolver threw " + e.InnerException.Message); }
            }
            catch (System.Exception e) { Debug.Log("DIAG hooks failed " + e); }
            try { FbxManager.Create().Destroy(); Debug.Log("DIAG first call ok"); }
            catch (System.Exception e) { Debug.Log("DIAG first call failed " + e.GetType().Name + " / " + e.GetBaseException().Message.Split('\n')[0]); }
            try {
                const System.Reflection.BindingFlags all = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                var asm = typeof(FbxManager).Assembly;
                var alcType = typeof(object).Assembly.GetType("System.Runtime.Loader.AssemblyLoadContext");
                var alc = alcType.GetMethod("GetLoadContext").Invoke(null, new object[] { asm });
                Debug.Log("DIAG alc=" + alc.GetType().FullName + " " + alc);
                var wrapper = alc.GetType().GetProperty("UnityAssemblyLoadContextWrapper", all)?.GetValue(alc);
                if (wrapper != null) {
                    foreach (var prop in wrapper.GetType().GetProperties(all))
                        if (prop.GetIndexParameters().Length == 0) { try { Debug.Log("DIAG wrapper." + prop.Name + "=" + prop.GetValue(wrapper)); } catch (System.Exception) {} }
                    var resolver = wrapper.GetType().GetProperty("NativeLibraryResolver", all).GetValue(wrapper);
                    var resolved = resolver.GetType().GetMethod("Resolve", all).Invoke(resolver, new object[] { asm, "UnityFbxSdkNative" });
                    Debug.Log("DIAG resolver=" + resolver.GetType().FullName + " resolved=" + (resolved ?? "<null>"));
                }
            }
            catch (System.Exception e) { Debug.Log("DIAG reflection failed " + e); }
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