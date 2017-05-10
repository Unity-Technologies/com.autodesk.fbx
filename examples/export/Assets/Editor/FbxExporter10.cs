// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using FbxSdk;

namespace FbxSdk.Examples
{
    namespace Editor {

        public class FbxProgressBarExample
        {
            const string MenuItemName = "File/Export FBX/10. Progress Bar Example";

            const string FileBaseName = "example_empty_scene_with_progress";

            const string ProgressBarTitle = "Fbx Export";

            const int NumNulls = 5000;

            /// <summary>
            /// This example is a source code example for how to show a progress bar during
            /// export.
            ///
            /// To show the progress bar in action, we create a menu option,
            /// create a scene with 5,000 nulls, and export it.
            /// </summary>
            [MenuItem (MenuItemName)]
            public static void OnExport ()
            {
                try {
                    // Create the FBX manager
                    using (var fbxManager = FbxManager.Create ())
                    {
                        var fbxScene = BuildScene(fbxManager);
                        if (fbxScene == null) { return; }

                        // Create the exporter
                        var fbxExporter = FbxExporter.Create (fbxManager, "Exporter");

                        // Find a random temp file
                        var path = "";
                        var attempts = 0;
                        do {
                            path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName() + ".fbx");
                            attempts++;
                        } while (System.IO.File.Exists(path) && attempts < 100);
                        if (attempts >= 100) { return; }

                        // Initialize the exporter.
                        if (!fbxExporter.Initialize (path)) { return; }

                        // Set the progress callback.
                        fbxExporter.SetProgressCallback(ExportProgressCallback);

                        // Export the scene to the file.
                        if (!fbxExporter.Export (fbxScene)) {
                            if (fbxExporter.GetStatus().Error()) {
                                Debug.LogWarning("FBX Export failed: " + fbxExporter.GetStatus());
                            } else {
                                Debug.Log("FBX Export canceled by user.");
                            }
                        }
                    }
                } finally {
                    // You must clear the progress bar when you're done,
                    // otherwise it never goes away and many actions in Unity
                    // are blocked (e.g. you can't quit).
                    EditorUtility.ClearProgressBar();
                }
            }

            static bool ExportProgressCallback(float percentage, string status) {
                // Convert from percentage to [0,1].
                // Then convert from that to [0.5,1] because the first half of
                // the progress bar was for creating the scene.
                var progress01 = 0.5f * (1f + (percentage / 100.0f));

                // Unity says "true" for "cancel"; FBX wants "true" for "continue"
                return !EditorUtility.DisplayCancelableProgressBar(ProgressBarTitle, status, progress01);
            }

            static FbxScene BuildScene(FbxManager fbxManager) {
                // Create a scene
                var fbxScene = FbxScene.Create (fbxManager, "Scene");

                // Create 5000 nulls. Show progress on this task from [0,0.5]
                EditorUtility.DisplayProgressBar(ProgressBarTitle, "Creating the scene", 0);
                int percent = 1 + NumNulls / 100;
                for(int i = 0; i < NumNulls; ++i) {
                    FbxNode.Create(fbxScene, "node_" + i);
                    if (i % percent == 0) {
                        if (EditorUtility.DisplayCancelableProgressBar(ProgressBarTitle, "Creating the scene", 0.5f * (float)i / (float)NumNulls)) {
                            // cancel silently
                            return null;
                        }
                    }
                }
                return fbxScene;
            }
        }
    }
}
