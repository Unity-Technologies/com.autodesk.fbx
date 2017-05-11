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
            bool Verbose = true;

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

            struct PropertyChannel {
                public string Property { get ; private set; }
                public string Channel { get ; private set; }
                public PropertyChannel(string p, string c) {
                    Property = p;
                    Channel = c;
                }
            }

            /// <summary>
            /// Map a Unity property name to the corresponding FBX property and
            /// channel names.
            /// </summary>
            static Dictionary<string, PropertyChannel> MapUnityPropertyNameToFbx = new Dictionary<string, PropertyChannel> ()
            {
                { "field of view",          new PropertyChannel("FocalLength", "FocalLength") },
                { "near clip plane",        new PropertyChannel("NearPlane", "NearPlane") },
                { "far clip plane",         new PropertyChannel("FarPlane", "FarPlane") },
                { "m_LocalPosition.x",      new PropertyChannel("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_X) },
                { "m_LocalPosition.y",      new PropertyChannel("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_Y) },
                { "m_LocalPosition.z",      new PropertyChannel("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_Z) },
            };

            /// <summary>
            /// Exporting rotations is more complicated. We need to convert
            /// from quaternion to euler. We use this class to help.
            /// </summary>
            class QuaternionCurve {
                public AnimationCurve x;
                public AnimationCurve y;
                public AnimationCurve z;
                public AnimationCurve w;
                Key [] m_keys;

                public struct Key {
                    public FbxTime time;
                    public FbxVector4 euler;
                }

                public QuaternionCurve() { }

                public static int GetQuaternionIndex(string unityPropertyName) {
                    if (!unityPropertyName.StartsWith("m_LocalRotation.")) { return -1; }
                    switch(unityPropertyName[unityPropertyName.Length - 1]) {
                        case 'x': return 0;
                        case 'y': return 1;
                        case 'z': return 2;
                        case 'w': return 3;
                        default: return -1;
                    }
                }

                public void SetCurve(int i, AnimationCurve curve) {
                    switch(i) {
                        case 0: x = curve; break;
                        case 1: y = curve; break;
                        case 2: z = curve; break;
                        case 3: w = curve; break;
                        default: throw new System.IndexOutOfRangeException();
                    }
                }

                static void Update(Dictionary<float, Quaternion> unityKeys, float t, int i, float v) {
                    Quaternion keyvalues;
                    unityKeys.TryGetValue(t, out keyvalues);
                    keyvalues[i] = v;
                    unityKeys[t] = keyvalues;
                }

                Key [] ComputeKeys() {
                    // Evaluate the quaternion at each key. The curves may
                    // not have all the same keys! And we may not have all
                    // four channels.  But normally we will.
                    var unityKeys = new Dictionary<float, Quaternion>();
                    if (x != null) { foreach(var key in x.keys) { Update(unityKeys, key.time, 0, key.value); } }
                    if (y != null) { foreach(var key in y.keys) { Update(unityKeys, key.time, 1, key.value); } }
                    if (z != null) { foreach(var key in z.keys) { Update(unityKeys, key.time, 2, key.value); } }
                    if (w != null) { foreach(var key in w.keys) { Update(unityKeys, key.time, 3, key.value); } }

                    // Convert to the Key type.
                    var keys = new Key[unityKeys.Count];
                    int i = 0;
                    foreach(var kvp in unityKeys) {
                        var seconds = kvp.Key;
                        var unityQuaternion = kvp.Value;
                        var fbxQuaternion = new FbxQuaternion(unityQuaternion.x, unityQuaternion.y, unityQuaternion.z, unityQuaternion.w);

                        Key key;
                        key.time = FbxTime.FromSecondDouble(seconds);
                        key.euler = fbxQuaternion.DecomposeSphericalXYZ();
                        keys[i++] = key;
                    }

                    // Sort the keys by time
                    System.Array.Sort(keys, (Key a, Key b) => a.time.CompareTo(b.time));

                    return keys;
                }

                public void Animate(FbxNode fbxNode, FbxAnimLayer fbxAnimLayer) {
                    /* find or create the three curves */
                    var x = fbxNode.LclRotation.GetCurve(fbxAnimLayer, Globals.FBXSDK_CURVENODE_COMPONENT_X, true);
                    var y = fbxNode.LclRotation.GetCurve(fbxAnimLayer, Globals.FBXSDK_CURVENODE_COMPONENT_Y, true);
                    var z = fbxNode.LclRotation.GetCurve(fbxAnimLayer, Globals.FBXSDK_CURVENODE_COMPONENT_Z, true);

                    /* set the keys */
                    x.KeyModifyBegin();
                    y.KeyModifyBegin();
                    z.KeyModifyBegin();

                    var keys = ComputeKeys();
                    for(int i = 0, n = keys.Length; i < n; ++i) {
                        var key = keys[i];
                        x.KeyAdd(key.time);
                        x.KeySet(i, key.time, (float)key.euler.X);

                        y.KeyAdd(key.time);
                        y.KeySet(i, key.time, (float)key.euler.Y);

                        z.KeyAdd(key.time);
                        z.KeySet(i, key.time, (float)key.euler.Z);
                    }

                    z.KeyModifyEnd();
                    y.KeyModifyEnd();
                    x.KeyModifyEnd();
                }
            }

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter09 Create () { return new FbxExporter09 (); }

            /// <summary>
            /// Export an AnimationCurve.
            ///
            /// This is not used for rotations, because we need to convert from
            /// quaternion to euler and various other stuff.
            /// </summary>
            protected void ExportAnimCurve (UnityEngine.Object unityObj,
                                            AnimationCurve unityAnimCurve,
                                            string unityPropertyName,
                                            FbxScene fbxScene,
                                            FbxAnimLayer fbxAnimLayer)
            {
                if (Verbose) {
                    Debug.Log("Exporting animation for " + unityObj.name + " (" + unityPropertyName + ")");
                }

                PropertyChannel fbxName;
                if (!MapUnityPropertyNameToFbx.TryGetValue(unityPropertyName, out fbxName)) {
                    return;
                }

                GameObject unityGo = GetGameObject(unityObj);
                FbxNode fbxNode;
                if (!MapUnityObjectToFbxNode.TryGetValue(unityGo, out fbxNode)) {
                    return;
                }

                // map unity property name to fbx property
                var fbxProperty = fbxNode.FindProperty (fbxName.Property, false);
                if (!fbxProperty.IsValid()) {
                    return;
                }

                // Create the AnimCurve on the channel
                // TODO: check we can do this with FOV and NearPlane - might need to create via
                //
                // float defaultValue = ?
                // int childId = 0;
                // fbxCurveNode.SetChannelValue( childId, defaultValue );
                // fbxCurveNode->ConnectToChannel fbxAnimCurve, childId );
                FbxAnimCurve fbxAnimCurve = fbxProperty.GetCurve (fbxAnimLayer, fbxName.Channel, true);

                // copy Unity AnimCurve to FBX AnimCurve.
                fbxAnimCurve.KeyModifyBegin();

                for(int keyIndex = 0, n = unityAnimCurve.length; keyIndex < n; ++keyIndex) {
                    var key = unityAnimCurve[keyIndex];
                    var fbxTime = FbxTime.FromSecondDouble(key.time);
                    fbxAnimCurve.KeyAdd (fbxTime);

                    //var leftTangent = AnimationUtility.GetKeyLeftTangentMode (unityAnimCurve, keyIndex);
                    //var rightTangent = AnimationUtility.GetKeyRightTangentMode (unityAnimCurve, keyIndex);

                    fbxAnimCurve.KeySet (keyIndex, fbxTime, key.value,
                            FbxAnimCurveDef.EInterpolationType.eInterpolationCubic,
                            FbxAnimCurveDef.ETangentMode.eTangentAuto);
                }

                fbxAnimCurve.KeyModifyEnd();
            }

            /// <summary>
            /// Export an AnimationClip as a single take
            /// </summary>
            protected void ExportAnimationClip (AnimationClip unityAnimClip, GameObject unityRoot, FbxScene fbxScene)
            {
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
                    timeMode = FbxTime.ConvertFrameRateToTimeMode(unityAnimClip.frameRate, precision);
                    precision *= 10;
                }
                if (timeMode == FbxTime.EMode.eCustom) {
                    timeMode = FbxTime.EMode.eFrames30;
                }
                FbxTime.SetGlobalTimeMode (timeMode);

                // set time correctly
                var fbxStartTime = FbxTime.FromSecondDouble(0);
                var fbxStopTime = FbxTime.FromSecondDouble(unityAnimClip.length);

                fbxAnimStack.SetLocalTimeSpan (new FbxTimeSpan(fbxStartTime, fbxStopTime));

                /* The major difficulty: Unity uses quaternions for rotation
                 * (which is how it should be) but FBX uses euler angles. So we
                 * need to gather up the list of transform curves per object. */
                var quaternions = new Dictionary<UnityEngine.Object, QuaternionCurve>();

                foreach (EditorCurveBinding unityCurveBinding in AnimationUtility.GetCurveBindings(unityAnimClip))
                {
                    Object unityObj = AnimationUtility.GetAnimatedObject (unityRoot, unityCurveBinding);
                    if (!unityObj) { continue; }

                    AnimationCurve unityAnimCurve = AnimationUtility.GetEditorCurve (unityAnimClip, unityCurveBinding);
                    if (unityAnimCurve == null) { continue; }

                    int index = QuaternionCurve.GetQuaternionIndex(unityCurveBinding.propertyName);
                    if (index == -1) {
                        /* Some normal property (translation, field of
                         * view, etc); export it right away. */
                        ExportAnimCurve (unityObj, unityAnimCurve, unityCurveBinding.propertyName,
                                fbxScene, fbxAnimLayer);
                    } else {
                        /* Quaternion property; save it to calculate later. */
                        QuaternionCurve quat;
                        if (!quaternions.TryGetValue(unityObj, out quat)) {
                            quat = new QuaternionCurve();
                            quaternions.Add(unityObj, quat);
                        }
                        quat.SetCurve(index, unityAnimCurve);
                    }
                }

                /* now export all the quaternion curves */
                foreach(var kvp in quaternions) {
                    var unityObj = kvp.Key;
                    var quat = kvp.Value;

                    FbxNode fbxNode;
                    if (!MapUnityObjectToFbxNode.TryGetValue(GetGameObject(unityObj), out fbxNode)) {
                        continue;
                    }
                    quat.Animate(fbxNode, fbxAnimLayer);
                }
            }

            /// <summary>
            /// Export the Animator component on this game object
            /// </summary>
            protected void ExportAnimationClips (GameObject unityRoot, FbxScene fbxScene)
            {
                var animator = unityRoot.GetComponent<Animator>();
                if (!animator) { return; }

                // Get the controller.
                var controller = animator.runtimeAnimatorController;
                if (!controller) { return; }

                // Only export each clip once.
                var exported = new HashSet<AnimationClip>();
                foreach (var clip in controller.animationClips) {
                    if (exported.Add(clip)) {
                        ExportAnimationClip(clip, unityRoot, fbxScene);
                    }
                }
            }

            /// <summary>
            /// Exports all animation
            /// </summary>
            protected void ExportAllAnimation (FbxScene fbxScene)
            {
                foreach (GameObject unityGo in this.MapUnityObjectToFbxNode.Keys) {
                    ExportAnimationClips (unityGo, fbxScene);
                }
            }

            /// <summary>
            /// Create the node hierarchy and store the mapping from Unity
            /// object to FBX node.
            /// </summary>
            protected void CreateHierarchy (GameObject unityGo, FbxScene fbxScene, FbxNode fbxParentNode)
            {
                if (Verbose) {
                    Debug.Log ("exporting " + unityGo.name);
                }

                FbxNode fbxNode = FbxNode.Create (fbxParentNode, unityGo.name);

                MapUnityObjectToFbxNode [unityGo] = fbxNode;

                foreach (Transform unityChild in unityGo.transform)
                {
                    CreateHierarchy (unityChild.gameObject, fbxScene, fbxNode);
                }
            }

            /// <summary>
            /// Export all the objects in the set.
            /// Return the number of objects in the set that we exported.
            /// </summary>
            public int ExportAll (IEnumerable<UnityEngine.Object> unityExportSet)
            {
                // Create the FBX manager; it cleans everything up at the end of the using block.
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

                    // Set compatibility to 2014
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

                    var fbxSettings = fbxScene.GetGlobalSettings ();
                    fbxSettings.SetSystemUnit(FbxSystemUnit.m); // Unity unit is meters

                    // While Unity is Y-up, Z-forward, left-handed, the FBX SDK can't convert to
                    // right-handed. So we just lie and say that we're right-handed like Maya.
                    fbxSettings.SetAxisSystem(FbxAxisSystem.MayaYUp);

                    FbxNode fbxRootNode = fbxScene.GetRootNode ();

                    // export set of objects
                    foreach (var obj in unityExportSet)
                    {
                        var unityGo = GetGameObject (obj);

                        if ( unityGo )
                        {
                            CreateHierarchy (unityGo, fbxScene, fbxRootNode);
                        }
                    }

                    ExportAllAnimation (fbxScene);

                    // Export the scene to the file.
                    status = fbxExporter.Export (fbxScene);

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
            public int NumNodes { get { return MapUnityObjectToFbxNode.Count; } }

            /// <summary>
            /// Clean up this class on garbage collection
            /// </summary>
            public void Dispose () { }

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
