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
        public class FbxExporter15 : System.IDisposable
        {
            const string Title =
                "Example 15: exporting a node hierarchy with custom properties";

            const string Subject =
                @"Example FbxExporter15 illustrates how to:
                    1) create and initialize an exporter
                    2) create a scene
                    3) create a hierarchy of nodes
                    4) add custom data as properties to each node
                    5) export the nodes to a FBX file (FBX201400 compatible)
                ";

            const string Keywords =
                "export node transform marker";

            const string Comments = "";

            const string MenuItemName = "File/Export FBX/15. Nodes with custom properties";

            const string FileBaseName = "example_nodes_with_custom_properties";

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter15 Create () { return new FbxExporter15(); }

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

                // add (not particularly useful) custom data: how many Unity
                // components does the unity object have?
                var numComponents = unityGo.GetComponents<UnityEngine.Component>().Length;
                var fbxProperty = FbxProperty.Create(fbxNode, Globals.FbxIntDT, "unity_component_count", "Number of Unity Components");
                if (!fbxProperty.IsValid()) {
                    throw new System.NullReferenceException();
                }
                fbxProperty.ModifyFlag(FbxPropertyFlags.EFlags.eUserDefined, true);
                fbxProperty.ModifyFlag(FbxPropertyFlags.EFlags.eAnimatable, true);
                fbxProperty.Set(numComponents);

                if (Verbose) {
                    Debug.Log (string.Format ("exporting {0} ({1} components)", fbxNode.GetName (), (int) fbxProperty.GetFloat()));
                }

                fbxParentNode.AddChild (fbxNode);

                // now go through our children and recurse
                foreach (Transform uniChildT in unityGo.transform)
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
                    var fbxExporter = FbxExporter.Create (fbxManager, "Exporter");

                    // Initialize the exporter.
                    int fileFormat = fbxManager.GetIOPluginRegistry ().FindWriterIDByDescription ("FBX ascii (*.fbx)");
                    bool status = fbxExporter.Initialize (LastFilePath, fileFormat, fbxManager.GetIOSettings ());

                    // Check that initialization of the fbxExporter was successful
                    if (!status)
                    {
                        return 0;
                    }

                    // By default, FBX exports in its most recent version. You might want to specify
                    // an older version for compatibility with other applications.
                    fbxExporter.SetFileExportVersion("FBX201400");

                    // Create a scene
                    var fbxScene = FbxScene.Create (fbxManager, "Scene");

                    // create scene info
                    FbxDocumentInfo fbxSceneInfo = FbxDocumentInfo.Create (fbxManager, "SceneInfo");

                    // set some scene info values
                    fbxSceneInfo.mTitle = Title;
                    fbxSceneInfo.mSubject = Subject;
                    fbxSceneInfo.mAuthor = "Unity Technologies";
                    fbxSceneInfo.mRevision = "1.0";
                    fbxSceneInfo.mKeywords = Keywords;
                    fbxSceneInfo.mComment = Comments;

                    fbxScene.SetSceneInfo (fbxSceneInfo);

                    var fbxSettings = fbxScene.GetGlobalSettings();
                    fbxSettings.SetSystemUnit(FbxSystemUnit.m); // Unity unit is meters

                    // The Unity axis system has Y up, Z forward, X to the right.
                    // Export as Maya axis system.
                    fbxSettings.SetAxisSystem(FbxAxisSystem.MayaYUp);

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
                                     ? MakeFileName(basename: FileBaseName, extension: Extension)
                                     : System.IO.Path.GetFileName (LastFilePath);

                var title = string.Format ("Export FBX ({0})", FileBaseName);

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
