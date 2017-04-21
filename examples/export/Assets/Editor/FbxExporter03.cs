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
    namespace Editor
    {
        public class FbxExporter03 : System.IDisposable
        {
            const string Title = 
                "Example 03: exporting a node hierarchy with transforms";
            
            const string Subject = 
                @"Example FbxExporter03 illustrates how to:
                    1) create and initialize an exporter        
                    2) create a scene                           
                    3) create a hierarchy of nodes              
                    4) add transform data to each node          
                    5) export the nodes to a FBX file (ASCII mode)
                ";
            
            const string Keywords = 
                "export node transform";
            
            const string Comments = 
                @"We are exporting rotations using the Euler angles from Unity.";

            const string MenuItemName = "File/Export/Export (nodes with transforms) to FBX";

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter03 Create () { return new FbxExporter03(); }

            /// <summary>
            /// Export GameObject's Transform component
            /// </summary>
            protected void ExportTransform (Transform unityTransform, FbxNode fbxNode)
            {
                // get local position of fbxNode (from Unity)
                UnityEngine.Vector3 unityTranslate  =  unityTransform.localPosition;
                UnityEngine.Vector3 unityRotate     =  unityTransform.localRotation.eulerAngles;
                UnityEngine.Vector3 unityScale      =  unityTransform.localScale;

                // transfer transform data from Unity to Fbx
                var fbxTranslate = new FbxDouble3 (unityTranslate.x, unityTranslate.y, unityTranslate.z);
                var fbxRotate = new FbxDouble3 (unityRotate.x, unityRotate.y, unityRotate.z);
                var fbxScale = new FbxDouble3 (unityScale.x, unityScale.y, unityScale.z);

                // set the local position of fbxNode
                fbxNode.LclTranslation.Set(fbxTranslate);
                fbxNode.LclRotation.Set(fbxRotate);
                fbxNode.LclScaling.Set(fbxScale);

                return;
            }

            /// <summary>
            /// Unconditionally export components on this game object
            /// </summary>
            protected void ExportComponents (GameObject  unityGo, FbxScene fbxScene, FbxNode fbxParentNode)
            {
                // create an FbxNode and add it as a child of fbxParentNode
                FbxNode fbxNode = FbxNode.Create (fbxScene,  unityGo.name);
                NumNodes++;

                ExportTransform ( unityGo.transform, fbxNode);

                if (Verbose)
                    Debug.Log (string.Format ("exporting {0}", fbxNode.GetName ()));

                fbxParentNode.AddChild (fbxNode);

                // now  unityGo  through our children and recurse
                foreach (Transform uniChildT in  unityGo.transform) 
                {
                    ExportComponents (uniChildT.gameObject, fbxScene, fbxNode);
                }

                return;
            }

            /// <summary>
            /// Export all the objects in the set.
            /// Return the number of objects in the set that we exported.
            /// </summary>
            public int ExportAll (IEnumerable<UnityEngine.Object> unityExportSet)
            {
                // Create the FBX manager
                using (var fbxManager = FbxManager.Create ()) 
                {
                    // Configure IO settings.
                    fbxManager.SetIOSettings (FbxIOSettings.Create (fbxManager, Globals.IOSROOT));

                    // Create the exporter 
                    var fbxExporter = FbxExporter.Create (fbxManager, MakeObjectName ("fbxExporter"));

                    // Initialize the exporter.
                    bool status = fbxExporter.Initialize (LastFilePath, -1, fbxManager.GetIOSettings ());

                    // Check that initialization of the fbxExporter was successful
                    if (!status) 
                    {
                        return 0;
                    }

                    // Create a scene
                    var fbxScene = FbxScene.Create (fbxManager, MakeObjectName ("Scene"));

                    // create scene info
                    FbxDocumentInfo fbxSceneInfo = FbxDocumentInfo.Create (fbxManager, MakeObjectName ("SceneInfo"));

                    // set some scene info values
                    fbxSceneInfo.mTitle = Title;
                    fbxSceneInfo.mSubject = Subject;
                    fbxSceneInfo.mAuthor = "Unity Technologies";
                    fbxSceneInfo.mRevision = "1.0";
                    fbxSceneInfo.mKeywords = Keywords;
                    fbxSceneInfo.mComment = Comments;

                    fbxScene.SetSceneInfo (fbxSceneInfo);

                    FbxNode fbxRootNode = fbxScene.GetRootNode ();

                    // export set of objects
                    foreach (var obj in unityExportSet) 
                    {
                        var  unityGo  = GetGameObject (obj);

                        if ( unityGo ) 
                        {
                            this.ExportComponents ( unityGo, fbxScene, fbxRootNode);
                        }
                    }

                    // Export the scene to the file.
                    status = fbxExporter.Export (fbxScene);

                    // cleanup
                    fbxScene.Destroy ();
                    fbxExporter.Destroy ();

                    return status == true ? NumNodes : 0;
                }
            }

            /// <summary>
            /// create menu item in the File menu
            /// </summary>
            [MenuItem (MenuItemName, false)]
            public static void OnMenuItem () 
            {
                OnExport();
            }

            /// <summary>
            /// Validate the menu item defined by the function above.
            /// Return false if no transform is selected.
            /// </summary>
            [MenuItem (MenuItemName, true)]
            public static bool OnValidateMenuItem ()
            {
                return Selection.activeTransform != null;
            }

            /// <summary>
            /// Number of nodes exported including siblings and decendents
            /// </summary>
            public int NumNodes { private set; get; }

            /// <summary>
            /// Clean up this class on garbage collection
            /// </summary>
            public void Dispose () { }

            static bool Verbose { get { return true; } }
            const string NamePrefix = "";

            /// <summary>
            /// manage the selection of a filename
            /// </summary>
            static string   LastFilePath { get; set; }
            static string   Basename { get { return GetActiveSceneName (); } }
            const string    Extension = "fbx";

            /// <summary>
            /// Get the GameObject
            /// </summary>
            private static GameObject GetGameObject (Object obj)
            {
                if (obj is UnityEngine.Transform) 
                {
                    var xform = obj as UnityEngine.Transform;
                    return xform.gameObject;
                } 
                else if (obj is UnityEngine.GameObject) 
                {
                    return obj as UnityEngine.GameObject;
                } 
                else if (obj is MonoBehaviour) 
                {
                    var mono = obj as MonoBehaviour;
                    return mono.gameObject;
                }

                return null;
            }

            private static string GetActiveSceneName()
            {
                var unityScene = SceneManager.GetActiveScene();

                return string.IsNullOrEmpty(unityScene.name) ? "Untitled" : unityScene.name;    
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

                if (string.IsNullOrEmpty (filePath)) 
                {
                    return;
                }

                LastFilePath = filePath;

                using (var fbxExporter = Create()) 
                {
                    // ensure output directory exists
                    EnsureDirectory (filePath);

                    if (fbxExporter.ExportAll(Selection.objects) > 0)
                    {
                        string message = string.Format ("Successfully exported: {0}", filePath);
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
