//#define UNI_16810
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

        public class FbxExporter11 : System.IDisposable
        {
            const string Title =
                 "Example 02: exporting a basic node hierarchy";

            const string Subject =
                 @"Example FbxExporter11 illustrates how to:
                                            1) create and initialize an exporter
                                            2) create a scene
                                            3) create a camera node and export some settings
                                            4) create animation take for the animated camera settings and SRT
                                            5) set the default camera for the scene
                                            6) export a scene to a FBX file (FBX201400 compatible)
                                                    ";

            const string Keywords =
                 "export camera node animation";

            const string Comments =
                 @"";

            const string MenuItemName = "File/Export FBX/11. camera with animation";

            const string FileBaseName = "example_camera_animation";

            /// <summary>
            /// map Unity animatable property to FbxProperty
            /// TODO: intrinsic properties, check can we find by them name?
            /// </summary>
            static Dictionary<string, string> MapUnityToFbxPropertyName = new Dictionary<string, string> ()
            {
                { "field of view",          "FocalLength" },
                { "near clip plane",        "NearPlane" },
                { "far clip plane",         "FarPlane" },
                { "m_LocalPosition.x",      "LclTranslation" },
                { "m_LocalPosition.y",      "LclTranslation" },
                { "m_LocalPosition.z",      "LclTranslation" },
                { "localEulerAnglesRaw.x",  "LclRotation" },
                { "localEulerAnglesRaw.y",  "LclRotation" },
                { "localEulerAnglesRaw.z",  "LclRotation" },
            };

            /// <summary>
            /// map Unity animatable property to FbxProperty
            /// </summary>
            static Dictionary<string, string> MapUnityToFbxChannelName = new Dictionary<string, string> ()
            {
                { "field of view",          "FocalLength" },
                { "near clip plane",        "NearPlane" },
                { "far clip plane",         "FarPlane" },
#if UNI_16421
				{ "m_LocalPosition.x",      fbxsdk.Globals.FBXSDK_CURVENODE_COMPONENT_X },
				{ "m_LocalPosition.y",      fbxsdk.Globals.FBXSDK_CURVENODE_COMPONENT_Y },
				{ "m_LocalPosition.z",      fbxsdk.Globals.FBXSDK_CURVENODE_COMPONENT_Z },
				{ "localEulerAnglesRaw.x",  fbxsdk.Globals.FBXSDK_CURVENODE_COMPONENT_X },
				{ "localEulerAnglesRaw.y",  fbxsdk.Globals.FBXSDK_CURVENODE_COMPONENT_Y },
				{ "localEulerAnglesRaw.z",  fbxsdk.Globals.FBXSDK_CURVENODE_COMPONENT_Z },
#endif
            };

            /// <summary>
            /// name of the scene's default camera
            /// </summary>
            static string DefaultCamera = "";

            /// <summary>
            /// collected list of cameras to export
            /// </summary>
            List<Camera> Cameras;

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter11 Create () { return new FbxExporter11 (); }

            /// <summary>
            /// Exports camera component
            /// </summary>
            protected void ExportCamera (Camera unityCamera, FbxScene fbxScene, FbxNode fbxNode)
            {
#if UNI_16810
                FbxCamera fbxCamera = FbxCamera.Create (fbxScene.GetFbxManager(), MakeObjectName(unityCamera.name));

                bool perspective = unityCamera.orthographic!=true;
                float aspectRatio = unityCamera.aspect;

                // Configure FilmBack settings: 35mm TV Projection (0.816 x 0.612)
                float apertureHeightInInches = 0.612f;
                float apertureWidthInInches = aspectRatio * apertureHeightInInches;

                FbxCamera.EProjectionType projectionType =
                    perspective ? FbxCamera.EProjectionType.ePerspective : FbxCamera.EProjectionType.eOrthogonal;
                
                fbxCamera.ProjectionType.Set(projectionType);
                fbxCamera.SetAspect (FbxCamera.EAspectRatioMode.eFixedRatio, aspectRatio, 1.0f);
                fbxCamera.FilmAspectRatio.Set(aspectRatio);
                fbxCamera.SetApertureWidth (apertureWidthInInches);
                fbxCamera.SetApertureHeight (apertureHeightInInches);
                fbxCamera.SetApertureMode (FbxCamera.EApertureMode.eFocalLength);

                // FOV / Focal Length
                fbxCamera.FocalLength.Set(fbxCamera.ComputeFocalLength (unityCamera.fieldOfView));

                // NearPlane
                fbxCamera.SetNearPlane (unityCamera.nearClipPlane);

                // FarPlane
                fbxCamera.SetFarPlane (unityCamera.farClipPlane);

                fbxNode.SetNodeAttribute (fbxCamera);
#endif
                // make the last camera exported the default camera
                DefaultCamera = fbxNode.GetName ();
            }

            /// <summary>
            /// Export camera animation as a single take
            /// </summary>
            protected void ExportCameraAnimation (Camera unityCamera, FbxScene fbxScene)
            {
#if UNI_16810
                ExportAnimationClips (unityCamera.gameObject.GetComponent<Animation> (), fbxScene);
#endif
            }

            /// <summary>
            /// configures default camera for the scene
            /// </summary>
            protected void SetDefaultCamera (FbxScene fbxScene)
            {
#if UNI_16810
            	if (DefaultCamera == "")
            		DefaultCamera = FbxSdk.Globals.FBXSDK_CAMERA_PERSPECTIVE;

            	fbxScene.GetGlobalSettings ().SetDefaultCamera (this.DefaultCamera);
#endif
            }

            /// <summary>
            /// Exports all animation
            /// </summary>
            protected void ExportAllAnimation(FbxScene fbxScene)
            {
                foreach (Camera unityCamera in this.Cameras) 
                {
                    ExportCameraAnimation (unityCamera, fbxScene);
                }
            }

            /// <summary>
            /// Exports the game object has a camera component
            /// </summary>
            protected void ExportComponents (GameObject  unityGo, FbxScene fbxScene, FbxNode fbxNodeParent)
            {
                Camera unityCamera = unityGo.GetComponent<Camera> ();

                if (unityCamera == null)
                    return;
                
                // create an node and add it as a child of parent
                FbxNode fbxNode = FbxNode.Create (fbxScene,  unityGo.name);
                NumNodes++;

                ExportCamera (unityCamera, fbxScene, fbxNode);

                if (Verbose)
                    Debug.Log (string.Format ("exporting {0}", fbxNode.GetName ()));

                fbxNodeParent.AddChild (fbxNode);

                // now  unityGo  through our children and recurse
                foreach (Transform childT in  unityGo.transform) {
                    ExportComponents (childT.gameObject, fbxScene, fbxNode);
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
                    // Configure the IO settings.
                    fbxManager.SetIOSettings (FbxIOSettings.Create (fbxManager, Globals.IOSROOT));

                    // Create the exporter 
                    var fbxExporter = FbxExporter.Create (fbxManager, MakeObjectName ("fbxExporter"));

                    // Initialize the exporter.
                    bool status = fbxExporter.Initialize (LastFilePath, -1, fbxManager.GetIOSettings ());
                    // Check that initialization of the fbxExporter was successful
                    if (!status)
                        return 0;

                    // By default, FBX exports in its most recent version. You might want to specify
                    // an older version for compatibility with other applications.
                    fbxExporter.SetFileExportVersion("FBX201400");

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

                    // export set of object
                    foreach (var obj in unityExportSet) 
                    {
                        var unityGo = GetGameObject (obj);

                        if (unityGo) 
                        {
                            this.ExportComponents (unityGo, fbxScene, fbxRootNode);
                        }
                    }

                    ExportAllAnimation (fbxScene);

                    // Set the scene's default camera.
                    SetDefaultCamera (fbxScene);

                    // Export the scene to the file.
                    status = fbxExporter.Export (fbxScene);

                    // cleanup
                    fbxScene.Destroy ();
                    fbxExporter.Destroy ();

                    if (!status)
                        return 0;
                }
                return NumNodes;
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
                if (Selection.activeTransform == null)
                    return false;

                return Selection.activeTransform.gameObject.GetComponent<Camera> () != null;
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
            /// Number of nodes exported including siblings and decendents
            /// </summary>
            public int NumNodes { private set; get; }

            /// <summary>
            /// Clean up this class on garbage collection
            /// </summary>
            public void Dispose () { }

            const string NamePrefix = "";
            bool Verbose { get { return true; } }

            /// <summary>
            /// manage the selection of a filename
            /// </summary>
            static string LastFilePath { get; set; }
            const string Extension = "fbx";

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

                if (string.IsNullOrEmpty (filePath)) {
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
