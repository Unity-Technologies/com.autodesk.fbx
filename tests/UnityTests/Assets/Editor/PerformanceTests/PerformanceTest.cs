using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using FbxSdk;
using System.Diagnostics;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System;

namespace PerformanceTests
{

    public class PerformanceTest
    {

        [System.Serializable]
        public class ResultJsonList
        {
            public List<ResultJson> tests;
        }

        [System.Serializable]
        public class ResultJson
        {
            public string testName;
            public double result;
            public bool success;
            public string error = "";
        }

        private ResultJson RunCppTest (string testName)
        {
            // run native C++ tests here + get results to compare against
            // In Windows, the exe has to be in the same folder as the fbxsdk library in order to run
            string path = Path.Combine(Application.dataPath, "Plugins/fbxsdk");
            Process cpp = new Process ();
            cpp.StartInfo.FileName = Path.Combine(path, "PerformanceBenchmarks.exe");
            cpp.StartInfo.Arguments = testName;
            cpp.StartInfo.RedirectStandardOutput = true;
            cpp.StartInfo.UseShellExecute = false;
            cpp.Start ();

            StringBuilder output = new StringBuilder ();
            while (!cpp.HasExited) {
                output.Append (cpp.StandardOutput.ReadToEnd ());
            }

            try {
                ResultJsonList cppJson = JsonUtility.FromJson<ResultJsonList> (output.ToString ());

                if(cppJson == null){
                    UnityEngine.Debug.LogError("CppError [" + testName + "]:" + output);
                    return null;
                }

                if (cppJson.tests.Count <= 0) {
                    UnityEngine.Debug.LogError ("Error: No json test results received");
                    return null;
                }

                ResultJson cppResult = cppJson.tests [0];

                Assert.IsTrue (cppResult.success);

                if (!String.IsNullOrEmpty (cppResult.error)) {
                    UnityEngine.Debug.LogError ("CppError [" + testName + "]: " + cppResult.error);
                }

                return cppResult;
            } catch (System.ArgumentException) {
                UnityEngine.Debug.LogError ("Error [" + testName + "]: Malformed json string: " + output);
                return null;
            }
        }

        [Test]
        public void FbxObjectCreateTest ()
        {
            var stopwatch = new Stopwatch ();

            FbxManager fbxManager = FbxManager.Create ();

            long total = 0;
            int N = 5000;

            stopwatch.Reset ();
            stopwatch.Start ();
            for (int i = 0; i < N; i++) {
                // ... run code to measure time for
                FbxObject.Create (fbxManager, "");
            }
            stopwatch.Stop ();

            total = stopwatch.ElapsedMilliseconds;

            // should destroy all objects allocated by the FbxManager
            fbxManager.Destroy ();

            // Check against Native C++ tests
            ResultJson cppResult = RunCppTest ("FbxObjectCreate:" + N);

            Assert.IsNotNull (cppResult);

            // Ex: test that the unity test is no more than 4 times slower
            Assert.LessOrEqual (total, 4 * cppResult.result);
        }

        [Test]
        public void EmptyExportImportTest ()
        {
            var stopwatch = new Stopwatch ();

            FbxManager fbxManager = FbxManager.Create ();

            int N = 10;
            long total = 0;

            for (int i = 0; i < N; i++) {
                stopwatch.Reset ();
                stopwatch.Start ();

                FbxIOSettings ioSettings = FbxIOSettings.Create (fbxManager, Globals.IOSROOT);
                fbxManager.SetIOSettings (ioSettings);

                FbxExporter exporter = FbxExporter.Create (fbxManager, "");

                string filename = "test.fbx";

                bool exportStatus = exporter.Initialize (filename, -1, fbxManager.GetIOSettings ());

                // Check that export status is True
                Assert.IsTrue (exportStatus);

                // Create an empty scene to export
                FbxScene scene = FbxScene.Create (fbxManager, "myScene");

                // Export the scene to the file.
                exporter.Export (scene);

                exporter.Destroy ();

                // Import to make sure file is valid

                FbxImporter importer = FbxImporter.Create (fbxManager, "");

                bool importStatus = importer.Initialize (filename, -1, fbxManager.GetIOSettings ());

                Assert.IsTrue (importStatus);

                // Create a new scene so it can be populated
                FbxScene newScene = FbxScene.Create (fbxManager, "myScene2");

                importer.Import (newScene);

                importer.Destroy ();

                stopwatch.Stop ();

                total += stopwatch.ElapsedMilliseconds;

                // Delete the file once the test is complete
                File.Delete (filename);
            }

            fbxManager.Destroy ();

            ResultJson cppResult = RunCppTest ("EmptyExportImport:" + N);

            Assert.IsNotNull (cppResult);

            // Ex: test that the unity test is no more than 4 times slower
            Assert.LessOrEqual (total / (float)N, 4 * cppResult.result);
        }
    }
}