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

        //
        // The exporter02 example illustrates how to:
        //
        //        1) create and initialize an exporter
        //        2) create a scene
        //        3) export the selected node and scene graph to a .FBX file (ASCII mode)
        //

        public class FbxExporter02 : System.IDisposable
        {
            bool Verbose { get { return true; } }

            public void Dispose () { }

            public void ExportComponents(FbxScene scene, FbxNode parent, IEnumerable<GameObject> exportSet)
            {
                foreach (GameObject go in exportSet) {
                    // create an FbxNode and add it as a child of parent
                    FbxNode node = FbxNode.Create (scene, go.name);
                    if (Verbose) 
                        Debug.Log (string.Format("exporting {0}",node.GetName()));
                    parent.AddChild (node);

                    List<GameObject> childSet = new List<GameObject>();
                    // now go through our children and recurse
                    foreach (Transform childT in go.transform) {
                        childSet.Add (childT.gameObject);
                    }

                    ExportComponents (scene, node, childSet);
                }

            }

            /// <summary>
            /// Export all the objects in the set.
            /// Return the number of objects in the set that we exported.
            /// </summary>
            public int ExportAll (IEnumerable<UnityEngine.Object> exportSet)
            {
                // Create fbx manager
                var manager = FbxManager.Create ();

                // Configure fbx IO settings.
                manager.SetIOSettings (FbxIOSettings.Create (manager, Globals.IOSROOT));

                // Create the exporter 
                var exporter = FbxExporter.Create (manager, MakeObjectName ("Exporter"));

                // Initialize the exporter.
                bool status = exporter.Initialize (LastFilePath, -1, manager.GetIOSettings ());
                // Check that initialization of the exporter was successful
                if (!status)
                    return 0;
                
                // Create a scene
                var scene = FbxScene.Create (manager, MakeObjectName ("Scene"));

            	// create scene info
            	FbxDocumentInfo sceneInfo = FbxDocumentInfo.Create (manager, MakeObjectName ("SceneInfo"));

            	// set some scene info values
            	sceneInfo.mTitle = " Example 02: node and scene graph";
            	sceneInfo.mSubject = "Example of a scene with a node hierarchy and document information settings";
            	sceneInfo.mAuthor = "Unit Technologies";
            	sceneInfo.mRevision = "1.0";
            	sceneInfo.mKeywords = "example scene with node hierarchy";
            	sceneInfo.mComment = "Export names and hierarchy of selected GameObjects.";

            	scene.SetSceneInfo (sceneInfo);

                // add the Unity scene hierarchy to the scene for export
                // only interested in GameObjects for now
                List<GameObject> gos = new List<GameObject> ();
                foreach (UnityEngine.Object obj in exportSet) {
                    GameObject go;
                    if (obj is UnityEngine.Transform) {
                        var xform = obj as UnityEngine.Transform;
                        go = xform.gameObject;
                    } else if (obj is UnityEngine.GameObject) {
                        go = obj as UnityEngine.GameObject;
                    } else if (obj is MonoBehaviour) {
                        var mono = obj as MonoBehaviour;
                        go = mono.gameObject;
                    } else {
                        if (Verbose) 
                            Debug.Log ("skipping {0}", obj);
                        continue;
                    }
                     gos.Add (go);
                }
                FbxNode root = scene.GetRootNode ();
                ExportComponents (scene, root, gos);

                // Export the scene to the file.
                status = exporter.Export (scene);

                // cleanup
                scene.Destroy ();
                exporter.Destroy ();
                manager.Destroy ();

                return status==true ? 1 : 0; 
            }

            // 
            // Create a simple user interface (menu items)
            //
            /// <summary>
            /// create menu item in the File menu
            /// </summary>
            [MenuItem ("File/Export/Export (Node hierarchy) to FBX", false)]
            public static void OnMenuItem ()
            {
                OnExport();
            }

            /// <summary>
            // Validate the menu item defined by the function above.
            /// </summary>
            [MenuItem ("File/Export/Export (Node hierarchy) to FBX", true)]
            public static bool OnValidateMenuItem ()
            {
                // Return true
                return true;
            }

            //
            // manage the selection of a filename
            //
            static string m_LastFilePath = "";
            static string LastFilePath { get { return m_LastFilePath; } set { m_LastFilePath = value; } }
            static string Basename { get { return GetActiveSceneName (); } }
            static string Extension { get { return "fbx"; } }

            private static string GetActiveSceneName()
            {
                var scene = SceneManager.GetActiveScene();

                return string.IsNullOrEmpty(scene.name) ? "Untitled" : scene.name;    
            }

            private static string MakeFileName(string basename = "test", string extension = "fbx")
            {
                return basename + "." + extension;
            }

            // use the SaveFile panel to allow user to enter a file name
            private static void OnExport()
            {
                // Now that we know we have stuff to export, get the user-desired path.
                var directory = string.IsNullOrEmpty (LastFilePath) 
                                      ? Application.dataPath 
                                      : System.IO.Path.GetDirectoryName (LastFilePath);
                
                var filename = string.IsNullOrEmpty (LastFilePath) 
                                     ? MakeFileName(basename: Basename, extension: Extension) 
                                     : System.IO.Path.GetFileName (LastFilePath);
                
                var title = string.Format ("Export FBX ({0})", Basename);

                var filePath = EditorUtility.SaveFilePanel (title, directory, filename, "");

                if (string.IsNullOrEmpty (filePath)) {
                    return;
                }

                LastFilePath = filePath;

                using (FbxExporter02 exporter = new FbxExporter02()) {
                    
    				// ensure output directory exists
                    EnsureDirectory (filePath);

                    if (exporter.ExportAll(Selection.objects) > 0)
                    {
                        string message = string.Format ("Successfully exported scene: {0}", filePath);
                        UnityEngine.Debug.Log (message);
                    }
                }
            }

            private static void EnsureDirectory(string path)
            {
                //check to make sure the path exists, and if it doesn't then
                //create all the missing directories.
                FileInfo fileInfo = new FileInfo (path);

                if (!fileInfo.Exists) {
                    Directory.CreateDirectory (fileInfo.Directory.FullName);
                }
            }

             private static string MakeObjectName (string name)
            {
                return "_fbxexporter02_" + name;
            }
        }
    }
}
