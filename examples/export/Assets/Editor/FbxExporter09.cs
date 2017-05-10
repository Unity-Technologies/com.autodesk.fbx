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
                                3) create a node hierarchy 
                                4) create animation takes for each animation clip
                                5) export a scene to a FBX file (FBX201400 compatible)
                                        ";

            const string Keywords =
                 "export animation clips";

            const string Comments =
                 "";

            const string MenuItemName = "File/Export FBX/WIP - 9. Animation clips";

            const string FileBaseName = "example_animation_clips";

            static Dictionary<string, string> MapUnityToFbxPropertyName = new Dictionary<string, string> ()
            {
                { "field of view",          "FocalLength" },
                { "near clip plane",        "NearPlane" },
                { "far clip plane",         "FarPlane" },
                { "m_LocalPosition.x",      "LclTranslation" },
                { "m_LocalPosition.y",      "LclTranslation" },
                { "m_LocalPosition.z",              "LclTranslation" },
                { "localEulerAnglesRaw.x",          "LclRotation" },
                { "localEulerAnglesRaw.y",          "LclRotation" },
                { "localEulerAnglesRaw.z",          "LclRotation" },
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
            /// Create instance of example
            /// </summary>
            public static FbxExporter09 Create () { return new FbxExporter09 (); }

            /// <summary>
            /// Export an AnimationCurve
            /// </summary>
            protected void ExportAnimCurve (Object unityObj,
                                            AnimationCurve unityAnimCurve,
                                            FbxNode fbxNode,
                                            string unityPropertyName,
                                            float framesPerSecond)
            {
#if UNI_16421
                FbxProperty fbxProperty;

                if (!MapUnityToFbxProperty.Contains (unityPropertyName))
                    return;

                GameObject unityGo = GetGameObject(unityObj);

                if (!MapUnityObjectToFbxNode.Contains(unityGo))
                    return;

                fbxNode = MapUnityObjectToFbxNode[unityGo]

                // map unity property name to fbx property
                fbxProperty = fbxNode.FindProperty (MapUnityToFbxPropertyName [unityPropertyName], false);

                // ensure curve node created
                FbxAnimCurveNode fbxCurveNode = fbxProperty.GetCurveNode (true);
                if ( !fbxCurveNode )
                {
                    return;
                }

                // TODO: get default value for property
                float DefaultValue = 0.0f;

                // map unity property to fbx property channel
                string fbxChannelName = MapUnityToFbxChannelName[unityPropertyName];

                // Create the AnimCurve on the channel
                // TODO: check we can do this with FOV and NearPlane - might need to create via
                // 
                // float defaultValue = ?
                // int childId = 0;
                // fbxCurveNode.SetChannelValue( childId, defaultValue );
                // fbxCurveNode->ConnectToChannel fbxAnimCurve, childId );

                FbxAnimCurve fbxAnimCurve = fbxProperty.GetCurve (fbxAnimLayer, fbxChannelName, true);

                // copy Unity AnimCurve to FBX AnimCurve.
                fbxAnimCurve.KeyModifyBegin();

                FbxTime fbxTime;
                int keyIndex = 0;
                double timeInSeconds = 0.0;

                foreach (Keyframe key in unityAnimCurve.keys)
                {
                    FbxAnimCurveDef.EInterpolationType fbxACInterpolation = FbxAnimCurveDef.EInterpolationType.eInterpolationCubic;
                    FbxAnimCurveDef.ETangentMode fbxACTangent = FbxAnimCurveDef.eTangentAuto;

                    // get time in seconds
                    // TODO: this will not be exactly keyframe aligned
                    timeInSeconds = key.time * framesPerSecond;
                    fbxTime.SetSecondDouble(timeInSeconds);

                    keyIndex = fbxAnimCurve.KeyAdd (fbxTime);

                    var leftTangent = AnimationUtility.GetKeyLeftTangentMode (unityAnimCurve, keyIndex);
                    var rightTangent = AnimationUtility.GetKeyLeftTangentMode (unityAnimCurve, keyIndex);

                    if (leftTangent == AnimationUtility.TangentMode.Constant && 
                        rightTangent == AnimationUtility.TangentMode.Constant )
                    {
                        fbxACInterpolation = FbxAnimCurveDef.EInterpolationType.eInterpolationConstant;
                        fbxACTangent = FbxAnimCurveDef.ETangentMode.eConstantStandard;                        
                    } 
                    else if (leftTangent == AnimationUtility.TangentMode.Linear && 
                        rightTangent == AnimationUtility.TangentMode.Linear )
                    {
                        fbxACInterpolation = FbxAnimCurveDef.EInterpolationType.eInterpolationLinear;
                        fbxACTangent = FbxAnimCurveDef.ETangentMode.eTangentUser;                        
                    } 
                    else if (leftTangent == AnimationUtility.TangentMode.Linear && 
                        rightTangent == AnimationUtility.TangentMode.Linear )
                    {
                        fbxACInterpolation = FbxAnimCurveDef.EInterpolationType.eInterpolationLinear;
                        fbxACTangent = FbxAnimCurveDef.ETangentMode.eTangentUser;                        
                    } 
                    else if (leftTangent == AnimationUtility.TangentMode.Free && 
                        rightTangent == AnimationUtility.TangentMode.Free )
                    {
                        fbxACInterpolation = FbxAnimCurveDef.EInterpolationType.eInterpolationCubic;
                        fbxACTangent = FbxAnimCurveDef.ETangentMode.eTangentBreak;                        
                    } 

                    fbxAnimCurve.KeySet (keyIndex, 
                                         fbxTime, 
                                         (double)key.value, 
                                         FbxAnimCurveDef.eInterpolationCubic, 
                                         FbxAnimCurveDef.eTangentAuto);
                }

                fbxCurve.KeyModifyEnd();
#endif
            }

            /// <summary>
            /// Export an AnimationClip as a single take
            /// </summary>
            protected void ExportAnimationClip (AnimationClip unityAnimClip, GameObject unityRoot, FbxScene fbxScene)
            {
#if UNI_16421
                // setup anim stack
                FbxAnimStack fbxAnimStack = FbxAnimStack.Create (fbxScene, MakeObjectName (unityAnimClip.name));
                fbxAnimStack.Description.Set ("Animation Take: " + unityAnimClip.name);

                // add one mandatory animation layer
                FbxAnimLayer fbxAnimLayer = FbxAnimLayer.Create (fbxScene, "Animation Base Layer");
                fbxAnimStack.AddMember (fbxAnimLayer);

                // TODO: set time span of stack
                float framesPerSecond = unityAnimClip.frameRate;

                // set time correctly
                FbxTime fbxStartTime = new FbxTime();
                FbxTime fbxStopTime = new FbxTime();

                if (Mathf.Approximately (framesPerSecond, FbxTime.GetFrameRate(FbxTime.EMode.eFrames60)))
                {
                    fbxStartTime.SetGlobalTimeMode (FbxTime.EMode.eFrames60);
                    fbxStopTime.SetGlobalTimeMode (FbxTime.EMode.eFrames60);
                }
                else
                {
                    fbxStartTime.SetGlobalTimeMode(FbxTime.EMode.eCustom, framesPerSecond);
                    fbxStopTime.SetGlobalTimeMode(FbxTime.EMode.eCustom, framesPerSecond);
                }

                fbxStartTime.SetSecondDouble(0.f);
                fbxStopTime.SetSecondDouble(unityAnimClip.length);

                FbxTimeSpan fbxTimeSpan = new FbxTimeSpan();
                fbxTimeSpan.Set(fbxStartTime, fbxStopTime);

                fbxAnimStack.SetLocalTimeSpan (fbxTimeSpan);

                var mapUnityObjectToFBXNode = new Dictionary<Object, FbxNode> ();

                foreach (EditorCurveBinding unityCurveBinding in AnimationUtility.GetCurveBindings(unityAnimClip))
                {
                    Object unityObj = AnimationUtility.GetAnimatedObject (unityRoot, unityCurveBinding);

                    AnimationCurve unityAnimCurve = AnimationUtility.GetEditorCurve (unityAnimClip, unityCurveBinding);
            
                    ExportAnimCurve (unityObj, unityAnimCurve, fbxScene, unityCurveBinding.propertyName, framesPerSecond);
                }
#else
                if (Verbose)
                    Debug.Log (string.Format ("exporting animation clip {0}", unityAnimClip.name));
                
                foreach (EditorCurveBinding unityCurveBinding in AnimationUtility.GetCurveBindings (unityAnimClip)) 
                {
                    if (Verbose)
                        Debug.Log (string.Format ("exporting animated property {0}", unityCurveBinding.propertyName));
                    
                    Object unityObj = AnimationUtility.GetAnimatedObject (unityRoot, unityCurveBinding);

                    if (Verbose)
                        Debug.Log (string.Format ("exporting anim on {0} {1}", unityObj.GetType(), unityObj.name));

                    AnimationCurve unityAnimCurve = AnimationUtility.GetEditorCurve (unityAnimClip, unityCurveBinding);

                    if (Verbose)
                        Debug.Log (string.Format ("exporting animcurve keys {0}", unityAnimCurve.keys.Length));

                }
#endif
            }

            /// <summary>
            /// Export the Animation component on this game object
            /// </summary>
            protected void ExportAnimationClips (Animation unityAnimation, FbxScene fbxScene)
            {
                if (unityAnimation == null)
                    return;

                GameObject unityRoot = unityAnimation.gameObject;

                // build a unique list of animation clips for export
                var animClips = new Dictionary<string, AnimationClip> ();

                string clipName;

                if (unityAnimation.clip != null) 
                {
                    clipName = unityAnimation.clip.name;
                    animClips [clipName] = unityAnimation.clip;

                    ExportAnimationClip (animClips [clipName], unityRoot, fbxScene);
                }

                foreach (AnimationState unityAnimState in unityAnimation)
                {
                    clipName = unityAnimState.clip.name;
                        
                    if (!animClips.ContainsKey (clipName)) 
                    {
                        animClips [clipName] = unityAnimState.clip;

                        ExportAnimationClip (animClips [clipName], unityRoot, fbxScene);
                    }
                }

                return;
            }

            /// <summary>
            /// Exports all animation
            /// </summary>
            protected void ExportAllAnimation (FbxScene fbxScene)
            {
                foreach (GameObject unityGo in this.MapUnityObjectToFbxNode.Keys) {
                    ExportAnimationClips (unityGo.GetComponent<Animation> (), fbxScene);
                }
            }

            /// <summary>
            /// Export the Animation component on this game object
            /// </summary>
            protected void ExportComponents (GameObject unityGo, FbxScene fbxScene, FbxNode fbxParentNode)
            {
                // create an FbxNode and add it as a child of fbxParentNode
                FbxNode fbxNode = FbxNode.Create (fbxScene, unityGo.name);
                NumNodes++;

                if (Verbose)
                    Debug.Log (string.Format ("exporting {0}", fbxNode.GetName ()));

                fbxParentNode.AddChild (fbxNode);
                MapUnityObjectToFbxNode [unityGo] = fbxNode;

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
                Verbose = true;

                // Create the FBX manager
                using (var fbxManager = FbxManager.Create ()) 
                {
                    // Configure the IO settings.
                    fbxManager.SetIOSettings (FbxIOSettings.Create (fbxManager, Globals.IOSROOT));

                    // Create the exporter 
                    var fbxExporter = FbxExporter.Create (fbxManager, MakeObjectName ("Exporter"));

                    // Initialize the exporter.
                    int fileFormat = fbxManager.GetIOPluginRegistry ().FindWriterIDByDescription ("FBX ascii (*.fbx)");
                    bool status = fbxExporter.Initialize (LastFilePath, fileFormat, fbxManager.GetIOSettings ());
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

                    ExportAllAnimation (fbxScene);

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
            /// keep a map between GameObject and FbxNode for quick lookup when we export
            /// animation.
            /// </summary>
            Dictionary<GameObject, FbxNode> MapUnityObjectToFbxNode = new Dictionary<GameObject, FbxNode> ();

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