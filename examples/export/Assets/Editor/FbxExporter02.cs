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

        public class FbxExporter02 : System.IDisposable
        {
                fbxSceneInfo.mTitle = " Example 02: node hierarchy";
                fbxSceneInfo.mSubject = "Example of a scene with a fbxNode hierarchy and document information settings";
                fbxSceneInfo.mAuthor = "Unit Technologies";
                fbxSceneInfo.mRevision = "1.0";
                fbxSceneInfo.mKeywords = "example fbxScene with fbxNode hierarchy";
                fbxSceneInfo.mComment = "Export names and hierarchy of selected GameObjects.";
            
            const string Title =
            	"Example 02: exporting a node hierarchy";

            const string Subject =
            	@"Example FbxExporter01 illustrates how to:
                                            1) create and initialize an exporter
                                            2) create a scene
                                            3) traverse hierarchy and add nodes
                                            4) export a scene to a FBX file (ASCII mode)
                                                    ";

            const string Keywords =
            	"export scene node hierarchy";

            const string Comments =
            	@"Export hierarchy of selected GameObjects.";

            const string MenuItemName = "File/Export/Export (node hierarchy) to FBX";

            /// <summary>
            /// Number of nodes exported including siblings and decendents
            /// </summary>
            public int NumNodes { private set; get; }

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter02 Create ()
            {
            	return new FbxExporter02 ();
            }

            /// <summary>
            /// Clean up this class on garbage collection
            /// </summary>
            public void Dispose () { }

            /// <summary>
            /// Unconditionally export components on this game object
            /// </summary>
            protected void ExportComponents (GameObject uniGo, FbxScene fbxScene, FbxNode fbxNodeParent)
            {
            	// create an FbxNode and add it as a child of parent
            	FbxNode fbxNode = FbxNode.Create (fbxScene, uniGo.name);
            	NumNodes++;

            	if (Verbose)
            		Debug.Log (string.Format ("exporting {0}", fbxNode.GetName ()));

            	fbxNodeParent.AddChild (fbxNode);

            	// now uniGo through our children and recurse
            	foreach (Transform childT in uniGo.transform) {
            		ExportComponents (childT.gameObject, fbxScene, fbxNode);
            	}

            	return;
            }

            /// <summary>
            /// Export all the objects in the set.
            /// Return the number of objects in the set that we exported.
            /// </summary>
            public int ExportAll (IEnumerable<UnityEngine.Object> uniExportSet)
            {
                // Create fbx fbxManager
                var fbxManager = FbxManager.Create ();

                // Configure fbx IO settings.
                fbxManager.SetIOSettings (FbxIOSettings.Create (fbxManager, Globals.IOSROOT));

                // Create the fbxExporter 
                var fbxExporter = FbxExporter.Create (fbxManager, MakeObjectName ("fbxExporter"));

                // Initialize the fbxExporter.
                bool status = fbxExporter.Initialize (LastFilePath, -1, fbxManager.GetIOSettings ());
                // Check that initialization of the fbxExporter was successful
                if (!status)
                    return 0;
                
                // Create a fbxScene
                var fbxScene = FbxScene.Create (fbxManager, MakeObjectName ("fbxScene"));

            	// create fbxScene info
            	FbxDocumentInfo fbxSceneInfo = FbxDocumentInfo.Create (fbxManager, MakeObjectName ("SceneInfo"));

            	// set some fbxScene info values
                fbxSceneInfo.mTitle = Title;
                fbxSceneInfo.mSubject = Subject;
                fbxSceneInfo.mAuthor = "Unit Technologies";
                fbxSceneInfo.mRevision = "1.0";
                fbxSceneInfo.mKeywords = Keywords;
                fbxSceneInfo.mComment = Comments;

            	fbxScene.SetSceneInfo (fbxSceneInfo);

                FbxNode fbxRootNode = fbxScene.GetRootNode ();

                // export set of object
                foreach (var obj in uniExportSet) 
                {
                    var uniGo = GetGameObject (obj);

                    if (uniGo) 
                    {

    					this.ExportComponents (uniGo, fbxScene, fbxRootNode);
                    }
                }

                // Export the fbxScene to the file.
                status = fbxExporter.Export (fbxScene);

                // cleanup
                fbxScene.Destroy ();
                fbxExporter.Destroy ();
                fbxManager.Destroy ();

                return status==true ? NumNodes : 0; 
            }

            // 
            // Create a simple user interface (menu items)
            //
            /// <summary>
            /// create menu item in the File menu
            /// </summary>
            [MenuItem (MenuItemName, false)]
            public static void OnMenuItem ()
            {
                OnExport();
            }

            /// <summary>
            // Validate the menu item defined by the function above.
            /// </summary>
            [MenuItem (MenuItemName, true)]
            public static bool OnValidateMenuItem ()
            {
                // Return true
                return true;
            }

            /// <summary>
            /// Get the GameObject
            /// </summary>
            private GameObject GetGameObject (Object obj)
            {
            	if (obj is UnityEngine.Transform) {
            		var xform = obj as UnityEngine.Transform;
            		return xform.gameObject;
            	} else if (obj is UnityEngine.GameObject) {
            		return obj as UnityEngine.GameObject;
            	} else if (obj is MonoBehaviour) {
            		var mono = obj as MonoBehaviour;
            		return mono.gameObject;
            	}

            	return null;
            }

            /// <summary>
            /// manage the selection of a filename
            /// </summary>
            static string LastFilePath { get; set; }
            static string Basename { get { return GetActiveSceneName (); } }
            const string Extension = "fbx";

            const string NamePrefix = "";
            bool Verbose { get { return true; } }

            private static string GetActiveSceneName()
            {
                var uniScene = SceneManager.GetActiveScene();

                return string.IsNullOrEmpty(uniScene.name) ? "Untitled" : uniScene.name;    
            }

            private static string MakeObjectName (string name)
            {
            	return NamePrefix + name;
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

                using (FbxExporter02 fbxExporter = Create()) 
                {
                    
    				// ensure output directory exists
                    EnsureDirectory (filePath);

                    if (fbxExporter.ExportAll(Selection.objects) > 0)
                    {
                        string message = string.Format ("Successfully exported fbxScene: {0}", filePath);
                        UnityEngine.Debug.Log (message);
                    }
                }
            }

            private static void EnsureDirectory(string path)
            {
                //check to make sure the path exists, and if it doesn't then
                //create all the missing directories.
                FileInfo fileInfo = new FileInfo (path);

                if (!fileInfo.Exists) 
                {
                    Directory.CreateDirectory (fileInfo.Directory.FullName);
                }
            }
        }
    }
}
