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

namespace Autodesk.Fbx.Examples
{
    namespace Editor
    {

        public class FbxExporter11 : System.IDisposable
        {
            const string Title =
                "Example 11: exporting selected cameras with their settings and animation.";

            const string Subject =
                 @"Example FbxExporter11 illustrates how to:
                                            1) create and initialize an exporter
                                            2) create a scene
                                            3) create a camera node and export some settings
                                            4) create animation take for the animated camera settings
                                            5) set the default camera for the scene
                                            6) export a scene to a FBX file (FBX201400 compatible)
                                                    ";

            const string Keywords =
                 "export camera node animation";

            const string Comments =
                 @"We set the filmback to 35mm TV Projection.";

            const string MenuItemName = "File/Export FBX/11. camera with animation";

            const string FileBaseName = "example_camera_animation";

            /// <summary>
            /// Map a Unity property name to the corresponding FBX property and
            /// channel names for animation export.
            /// </summary>
            /// 
            static Dictionary<string, FbxPropertyChannelPair> MapUnityPropertyNameToFbx = new Dictionary<string, FbxPropertyChannelPair> ()
            {
                { "field of view",          new FbxPropertyChannelPair("FocalLength") },
                { "near clip plane",        new FbxPropertyChannelPair("NearPlane") },
                { "m_BackGroundColor.r",    new FbxPropertyChannelPair (MakeName("backgroundColor"), Globals.FBXSDK_CURVENODE_COLOR_RED) },
                { "m_BackGroundColor.g",    new FbxPropertyChannelPair (MakeName("backgroundColor"), Globals.FBXSDK_CURVENODE_COLOR_GREEN) },
                { "m_BackGroundColor.b",    new FbxPropertyChannelPair (MakeName("backgroundColor"), Globals.FBXSDK_CURVENODE_COLOR_BLUE) },
                { "m_BackGroundColor.a",    new FbxPropertyChannelPair (MakeName("backgroundColor"), "W") },
                { "clearFlags",             new FbxPropertyChannelPair (MakeName("clearFlags")) },
                { "m_LocalPosition.x",      new FbxPropertyChannelPair("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_X) },
                { "m_LocalPosition.y",      new FbxPropertyChannelPair("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_Y) },
                { "m_LocalPosition.z",      new FbxPropertyChannelPair("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_Z) },
            };

            /// <summary>
            /// name of the scene's default camera
            /// </summary>
            static string DefaultCamera = "";

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter11 Create () { return new FbxExporter11 (); }

            /// <summary>
            /// Exports camera component
            /// </summary>
            protected FbxCamera ExportCamera (Camera unityCamera, FbxScene fbxScene, FbxNode fbxNode)
            {
                FbxCamera fbxCamera = FbxCamera.Create (fbxScene.GetFbxManager(), unityCamera.name);

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

                // Export backgroundColor as a custom property
                // NOTE: export on fbxNode so that it will show up in Maya
                ExportColorProperty (fbxNode, unityCamera.backgroundColor,
                                     MakeName("backgroundColor"), 
                                     "The color with which the screen will be cleared.");

                // Export clearFlags as a custom property
                // NOTE: export on fbxNode so that it will show up in Maya
                ExportIntProperty (fbxNode, (int)unityCamera.clearFlags,
                                     MakeName("clearFlags"), 
                                     "How the camera clears the background.");


                return fbxCamera;
            }

            /// <summary>
            /// Export Component's color property
            /// </summary>
            FbxProperty ExportColorProperty (FbxObject fbxObject, Color value, string name, string label)
            {
                // create a custom property for component value
                var fbxProperty = FbxProperty.Create (fbxObject, Globals.FbxColor4DT, name, label);
                if (!fbxProperty.IsValid ()) {
                    throw new System.NullReferenceException ();
                }

                FbxColor fbxColor = new FbxColor(value.r, value.g, value.b, value.a );

                fbxProperty.Set (fbxColor);

                // Must be marked user-defined or it won't be shown in most DCCs
                fbxProperty.ModifyFlag (FbxPropertyFlags.EFlags.eUserDefined, true);
                fbxProperty.ModifyFlag (FbxPropertyFlags.EFlags.eAnimatable, true);

                return fbxProperty;
            }

            /// <summary>
            /// Export Component's int property
            /// </summary>
            FbxProperty ExportIntProperty (FbxObject fbxObject, int value, string name, string label)
            {
                // create a custom property for component value
                var fbxProperty = FbxProperty.Create (fbxObject, Globals.FbxIntDT, name, label);
                if (!fbxProperty.IsValid ()) {
                    throw new System.NullReferenceException ();
                }
                fbxProperty.Set (value);

                // Must be marked user-defined or it won't be shown in most DCCs
                fbxProperty.ModifyFlag (FbxPropertyFlags.EFlags.eUserDefined, true);
                fbxProperty.ModifyFlag (FbxPropertyFlags.EFlags.eAnimatable, true);

                return fbxProperty;
            }

            /// <summary>
            /// Export an AnimationCurve.
            ///
            /// This is not used for rotations, because we need to convert from
            /// quaternion to euler and various other stuff.
            /// </summary>
            protected void ExportAnimCurve (UnityEngine.Object unityObj,
                                            AnimationCurve unityAnimCurve,
                                            string unityPropertyName,
                                            FbxAnimLayer fbxAnimLayer)
            {
                FbxPropertyChannelPair fbxPair;

                if (!MapUnityPropertyNameToFbx.TryGetValue (unityPropertyName, out fbxPair)) {
                    Debug.LogWarning (string.Format ("no property-channel mapping found for {0}", unityPropertyName));
                    return;
                }

                GameObject unityGo = GetGameObject (unityObj);
                if (unityGo == null) {
                    Debug.LogError (string.Format ("cannot convert to GameObject from {0}", unityObj.ToString ()));
                    return;
                }

                FbxNode fbxNode;
                if (!MapUnityObjectToFbxNode.TryGetValue (unityGo, out fbxNode)) {
                    Debug.LogError (string.Format ("cannot find fbxNode for {0}", unityGo.ToString ()));
                    return;
                }

                FbxProperty fbxProperty = null;

                // try finding unity property name on node attribute
                FbxNodeAttribute fbxNodeAttribute = fbxNode.GetNodeAttribute ();
                if (fbxNodeAttribute != null) {
                    fbxProperty = fbxNodeAttribute.FindProperty (fbxPair.Property, false);
                }

                // try finding unity property on the node
                if (fbxProperty == null || !fbxProperty.IsValid ()) {
                    fbxProperty = fbxNode.FindProperty (fbxPair.Property, false);
                }

                if (!fbxProperty.IsValid ()) {
                    Debug.LogError (string.Format ("cannot find fbxProperty {0} on {1}", fbxPair.Property, fbxNode.GetName ()));
                    return;
                }

                if (Verbose) {
                    Debug.Log (string.Format ("Exporting animation for {0} ({1})",
                                              unityObj.ToString (),
                                              fbxPair.Property));
                }

                // Create the AnimCurve on the channel
                FbxAnimCurve fbxAnimCurve = (fbxPair.Channel != null)
                    ? fbxProperty.GetCurve (fbxAnimLayer, fbxPair.Channel, true)
                                 : fbxProperty.GetCurve (fbxAnimLayer, true);

                if (fbxAnimCurve == null)
                {
                    Debug.LogError (string.Format ("failed to create AnimCurve for {2}.{0}.{1}", 
                                                   fbxPair.Property, fbxPair.Channel, fbxNode.GetName ()));
                    return;                    
                }
                // copy Unity AnimCurve to FBX AnimCurve.
                fbxAnimCurve.KeyModifyBegin ();

                for (int keyIndex = 0, n = unityAnimCurve.length; keyIndex < n; ++keyIndex) {
                    var key = unityAnimCurve [keyIndex];
                    var fbxTime = FbxTime.FromSecondDouble (key.time);
                    fbxAnimCurve.KeyAdd (fbxTime);
                    fbxAnimCurve.KeySet (keyIndex, fbxTime, key.value);
                }

                fbxAnimCurve.KeyModifyEnd ();
            }

            /// <summary>
            /// Exports all animation
            /// </summary>
            private void ExportAnimationClip (AnimationClip unityAnimClip, GameObject unityRoot, FbxScene fbxScene)
            {
                if (unityAnimClip == null) return;

                // setup anim stack
                FbxAnimStack fbxAnimStack = FbxAnimStack.Create (fbxScene, unityAnimClip.name);
                fbxAnimStack.Description.Set ("Animation Take: " + unityAnimClip.name);

                // add one mandatory animation layer
                FbxAnimLayer fbxAnimLayer = FbxAnimLayer.Create (fbxScene, "Animation Base Layer");
                fbxAnimStack.AddMember (fbxAnimLayer);

                // Set up the FPS so our frame-relative math later works out
                // Custom frame rate isn't really supported in FBX SDK (there's
                // a bug), so try hard to find the nearest time mode.
                FbxTime.EMode timeMode = FbxTime.EMode.eCustom;
                double precision = 1e-6;
                while (timeMode == FbxTime.EMode.eCustom && precision < 1000) {
                    timeMode = FbxTime.ConvertFrameRateToTimeMode (unityAnimClip.frameRate, precision);
                    precision *= 10;
                }
                if (timeMode == FbxTime.EMode.eCustom) {
                    timeMode = FbxTime.EMode.eFrames30;
                }
                FbxTime.SetGlobalTimeMode (timeMode);

                // set time correctly
                var fbxStartTime = FbxTime.FromSecondDouble (0);
                var fbxStopTime = FbxTime.FromSecondDouble (unityAnimClip.length);

                fbxAnimStack.SetLocalTimeSpan (new FbxTimeSpan (fbxStartTime, fbxStopTime));

                foreach (EditorCurveBinding unityCurveBinding in AnimationUtility.GetCurveBindings (unityAnimClip)) {
                    Object unityObj = AnimationUtility.GetAnimatedObject (unityRoot, unityCurveBinding);
                    if (!unityObj) { continue; }

                    AnimationCurve unityAnimCurve = AnimationUtility.GetEditorCurve (unityAnimClip, unityCurveBinding);
                    if (unityAnimCurve == null) { continue; }

                    ExportAnimCurve (unityObj, unityAnimCurve, unityCurveBinding.propertyName, fbxAnimLayer);
                }
            }

            /// <summary>
            /// Export the Animator component on this game object
            /// </summary>
            protected void ExportAnimationClips (GameObject unityRoot, FbxScene fbxScene)
            {
                var unityAnimator = unityRoot.GetComponent<Animator> ();
                if (!unityAnimator) { return; }

                // Get the controller.
                var unityAnimController = unityAnimator.runtimeAnimatorController;
                if (!unityAnimController) { return; }

                // Only export each clip once per game object.
                var unityExportedAnimClip = new HashSet<AnimationClip> ();
                foreach (var unityAnimClip in unityAnimController.animationClips) {
                    if (unityExportedAnimClip.Add (unityAnimClip)) {
                        ExportAnimationClip (unityAnimClip, unityRoot, fbxScene);
                    }
                }
            }

            /// <summary>
            /// Exports all animation
            /// </summary>
            protected void ExportAllAnimation (FbxScene fbxScene)
            {
                // Export animations.
                foreach (var unityGo in MapUnityObjectToFbxNode.Keys) {
                    ExportAnimationClips (unityGo, fbxScene);
                }
            }

            /// <summary>
            /// configures default camera for the scene
            /// </summary>
            protected void SetDefaultCamera (FbxScene fbxScene)
            {
                if (DefaultCamera == "")
                    DefaultCamera = Globals.FBXSDK_CAMERA_PERSPECTIVE;

                fbxScene.GetGlobalSettings ().SetDefaultCamera (DefaultCamera);
            }

            ///<summary>
            ///struct use to map Unity Object to FbxProperty Channel Name
            ///</summary>
            struct FbxPropertyChannelPair
            {
                public string Property { get; private set; }
                public string Channel { get; private set; }
                public FbxPropertyChannelPair (string p, string c = null)
                {
                    Property = p;
                    Channel = c;
                }

                public bool IsValid { get { return Property.Length > 0; } }

            };

            /// <summary>
            /// keep a map between GameObject and FbxNode for quick lookup when we export
            /// animation.
            /// </summary>
            Dictionary<GameObject, FbxNode> MapUnityObjectToFbxNode = new Dictionary<GameObject, FbxNode> ();

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

                FbxNodeAttribute fbxNodeAttr = ExportCamera (unityCamera, fbxScene, fbxNode);

                if (fbxNodeAttr != null)
                {
                    fbxNode.SetNodeAttribute (fbxNodeAttr);

                    // make the last camera exported the default camera
                    DefaultCamera = fbxNode.GetName ();
                }

                if (Verbose)
                    Debug.Log (string.Format ("exporting {0}", fbxNode.GetName ()));

                fbxNodeParent.AddChild (fbxNode);

                // add mapping between fbxnode for light 
                // and unity game object for animation export
                MapUnityObjectToFbxNode [unityGo] = fbxNode;

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
                    var fbxExporter = FbxExporter.Create (fbxManager, "Exporter");

                    // Initialize the exporter.
                    int fileFormat = fbxManager.GetIOPluginRegistry ().FindWriterIDByDescription ("FBX ascii (*.fbx)");
                    bool status = fbxExporter.Initialize (LastFilePath, fileFormat, fbxManager.GetIOSettings ());
                    // Check that initialization of the fbxExporter was successful
                    if (!status)
                        return 0;

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

                    // Export animations
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
                 } else if (obj is Component) {
                      var comp = obj as Component;
                      return comp.gameObject;
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

            /// <summary>
            /// name prefix for custom properties
            /// </summary>
            const string NamePrefix = "Unity_";

            private static string MakeName (string basename)
            {
                return NamePrefix + basename;
            }

            bool Verbose { get { return true; } }

            /// <summary>
            /// manage the selection of a filename
            /// </summary>
            static string LastFilePath { get; set; }
            const string Extension = "fbx";

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
