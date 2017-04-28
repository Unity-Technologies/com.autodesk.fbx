//#define UNI_16421
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

        public class FbxExporter09 : System.IDisposable
        {
            const string Title =
                "Example 09: exporting animation clips";

            const string Subject =
                 @"Example FbxExporter09 illustrates how to:
                                1) create and initialize an exporter
                                2) create a scene
                                3) export a scene to a FBX file (ASCII mode)
                                        ";

            const string Keywords =
                 "export animation clips";

            const string Comments =
                 "";

            const string MenuItemName = "File/Export FBX/WIP - 9. Animation clips";

            const string FileBaseName = "example_animation_clips";

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter09 Create () { return new FbxExporter09 (); }

            /// <summary>
            /// Export an AnimationClip as a single take
            /// </summary>
            protected void ExportAnimationClip (AnimationClip unityAnimClip, FbxScene fbxScene)
            {
#if UNI_16421
                // setup anim stack
                FbxAnimStack fbxAnimStack = FbxAnimStack.Create (fbxScene, MakeObjectName (unityAnimClip.name + " Take"));
                fbxAnimStack.Description.Set ("Animation Take for scene.");

                // add one mandatory animation layer
                FbxAnimLayer fbxAnimLayer = FbxAnimLayer.Create (fbxScene, "Animation Base Layer");
                fbxAnimStack.AddMember (fbxAnimLayer);

                // TODO: set time span of stack

                // TODO: collect bones

                // TODO: export curves on bone
#endif                
            }

            /// <summary>
            /// Export the Animation component on this game object
            /// </summary>
            protected void ExportAnimationClips (Animation unityAnimation, FbxScene fbxScene)
            {
                // build a unique list of animation clips for export
                var animClips = new Dictionary<string, AnimationClip> ();

                if (unityAnimation.clip != null) 
                {
                    animClips [unityAnimation.clip.name] = unityAnimation.clip;
                }

                foreach (AnimationState unityAnimState in unityAnimation)
                {
                    var unityAnimClip = unityAnimation.clip;
                    animClips [unityAnimClip.name] = unityAnimClip;
                }

                // export that list
                foreach (string clipName in animClips.Keys)
                {
                    ExportAnimationClip (animClips [clipName], fbxScene);
                }

            	return;
            }

            /// <summary>
            /// Export the Animation component on this game object
            /// </summary>
            protected void ExportComponents (GameObject unityGo, FbxScene fbxScene, FbxNode fbxParentNode)
            {
                Animation unityAnimation = unityGo.GetComponent<Animation> ();

                if (unityAnimation == null)
                    return;

                ExportAnimationClips (unityAnimation, fbxScene);

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
                    // Configure the IO settings.
                    fbxManager.SetIOSettings (FbxIOSettings.Create (fbxManager, Globals.IOSROOT));

                    // Create the exporter 
                    var fbxExporter = FbxExporter.Create (fbxManager, MakeObjectName ("Exporter"));

                    // Initialize the exporter.
                    bool status = fbxExporter.Initialize (LastFilePath, -1, fbxManager.GetIOSettings ());
                    // Check that initialization of the fbxExporter was successful
                    if (!status)
                        return 0;

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

                    var fbxSettings = fbxScene.GetGlobalSettings ();
                    fbxSettings.SetSystemUnit(FbxSystemUnit.m); // Unity unit is meters

                    // The Unity axis system has Y up, Z forward, X to the right:
                    var fbxAxisSystem = new FbxAxisSystem (FbxAxisSystem.EUpVector.eYAxis, 
                                                           FbxAxisSystem.EFrontVector.eParityOdd, 
                                                           FbxAxisSystem.ECoordSystem.eLeftHanded);
                    fbxSettings.SetAxisSystem(fbxAxisSystem);

                    FbxNode fbxRootNode = fbxScene.GetRootNode ();

                    // export set of objects
                    foreach (var obj in unityExportSet) 
                    {
                        var unityGo = GetGameObject (obj);

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


            const string NamePrefix = "";
            public bool Verbose { private set; get; }

            /// <summary>
            /// manage the selection of a filename
            /// </summary>
            static string LastFilePath { get; set; }
            const string Extension = "fbx";

            /// <summary>
            /// Get the GameObject
            /// </summary>
            private static GameObject GetGameObject (Object obj)
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