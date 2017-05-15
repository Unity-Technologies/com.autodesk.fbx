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

                Key [] ComputeKeys(FbxNode node) {
                    // Get the source pivot pre-rotation if any, so we can
                    // remove it from the animation we get from Unity.
                    var fbxPreRotationEuler = node.GetRotationActive() ? node.GetPreRotation(FbxNode.EPivotSet.eSourcePivot)
                        : new FbxVector4();
                    var fbxPreRotationInverse = new FbxQuaternion();
                    fbxPreRotationInverse.ComposeSphericalXYZ(fbxPreRotationEuler);
                    fbxPreRotationInverse.Inverse();

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

                        // The final animation, including the effect of pre-rotation:
                        var fbxFinalAnimation = new FbxQuaternion(unityQuaternion.x, unityQuaternion.y, unityQuaternion.z, unityQuaternion.w);

                        // Cancel out the pre-rotation. Order matters. FBX reads left-to-right.
                        // When we run animation we will apply:
                        //      pre-rotation
                        //      then pre-rotation inverse
                        //      then animation.
                        var fbxAnimation = fbxPreRotationInverse * fbxFinalAnimation;

                        // Store the key so we can sort them later.
                        Key key;
                        key.time = FbxTime.FromSecondDouble(seconds);
                        key.euler = fbxAnimation.DecomposeSphericalXYZ();
                        keys[i++] = key;
                    }

                    // Sort the keys by time
                    System.Array.Sort(keys, (Key a, Key b) => a.time.CompareTo(b.time));

                    return keys;
                }

                public void Animate(FbxNode fbxNode, FbxAnimLayer fbxAnimLayer, bool Verbose) {
                    /* Find or create the three curves. */
                    var x = fbxNode.LclRotation.GetCurve(fbxAnimLayer, Globals.FBXSDK_CURVENODE_COMPONENT_X, true);
                    var y = fbxNode.LclRotation.GetCurve(fbxAnimLayer, Globals.FBXSDK_CURVENODE_COMPONENT_Y, true);
                    var z = fbxNode.LclRotation.GetCurve(fbxAnimLayer, Globals.FBXSDK_CURVENODE_COMPONENT_Z, true);

                    /* set the keys */
                    x.KeyModifyBegin();
                    y.KeyModifyBegin();
                    z.KeyModifyBegin();

                    var keys = ComputeKeys(fbxNode);
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

                    if (Verbose) {
                        Debug.Log("Exported rotation animation for " + fbxNode.GetName());
                    }
                }
            }

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter09 Create () { return new FbxExporter09 (); }

            /// <summary>
            /// Export bones of skinned mesh, if this is a skinned mesh with
            /// bones and bind poses.
            /// </summary>
            void ExportSkeleton (GameObject unityGo, FbxScene fbxScene)
            {
                var unitySkinnedMeshRenderer = unityGo.GetComponent<SkinnedMeshRenderer>();
                if (!unitySkinnedMeshRenderer) { return; }
                var bones = unitySkinnedMeshRenderer.bones;
                if (bones == null || bones.Length == 0) { return; }
                var mesh = unitySkinnedMeshRenderer.sharedMesh;
                if (!mesh) { return; }
                var bindPoses = mesh.bindposes;
                if (bindPoses == null || bindPoses.Length != bones.Length) { return; }

                // Three steps:
                // 0. Set up the map from bone to index.
                // 1. Create the bones, in arbitrary order.
                // 2. Connect up the hierarchy.
                // 3. Set the transforms.
                // Step 0 supports step 1 (finding which is the root bone) and step 3
                // (setting up transforms; the complication is the use of pivots).

                // Step 0: map transform to index so we can look up index by bone.
                Dictionary<Transform, int> index = new Dictionary<Transform, int>();
                for (int boneIndex = 0, n = bones.Length; boneIndex < n; boneIndex++) {
                    Transform unityBoneTransform = bones [boneIndex];
                    index[unityBoneTransform] = boneIndex;
                }

                // Step 1: create the bones.
                for (int boneIndex = 0, n = bones.Length; boneIndex < n; boneIndex++) {
                    Transform unityBoneTransform = bones [boneIndex];

                    // Create the bone node if we haven't already. Parent it to
                    // its corresponding parent, or to the scene if there is none.
                    FbxNode fbxBoneNode;
                    if (!MapUnityObjectToFbxNode.TryGetValue(unityBoneTransform.gameObject, out fbxBoneNode)) {
                        var unityParent = unityBoneTransform.parent;
                        FbxNode fbxParent;
                        if (MapUnityObjectToFbxNode.TryGetValue(unityParent.gameObject, out fbxParent)) {
                            fbxBoneNode = FbxNode.Create (fbxParent, unityBoneTransform.name);
                        } else {
                            fbxBoneNode = FbxNode.Create (fbxScene, unityBoneTransform.name);
                        }
                        MapUnityObjectToFbxNode.Add(unityBoneTransform.gameObject, fbxBoneNode);
                    }

                    // Set it up as a skeleton node if we haven't already.
                    if (fbxBoneNode.GetSkeleton() == null) {
                        FbxSkeleton fbxSkeleton = FbxSkeleton.Create (fbxScene, unityBoneTransform.name + "_Skel");
                        var fbxSkeletonType = index.ContainsKey(unityBoneTransform.parent)
                            ? FbxSkeleton.EType.eLimbNode : FbxSkeleton.EType.eRoot;
                        fbxSkeleton.SetSkeletonType (fbxSkeletonType);
                        fbxSkeleton.Size.Set (1.0f);
                        fbxBoneNode.SetNodeAttribute (fbxSkeleton);
                        if (Verbose) { Debug.Log("Converted " + unityBoneTransform.name + " to a " + fbxSkeletonType + " bone"); }
                    }
                }

                // Step 2: connect up the hierarchy.
                foreach (var unityBone in bones) {
                    var fbxBone = MapUnityObjectToFbxNode[unityBone.gameObject];
                    var fbxParent = MapUnityObjectToFbxNode[unityBone.parent.gameObject];
                    fbxParent.AddChild(fbxBone);
                }

                // Step 3: set up the transforms.
                for (int boneIndex = 0, n = bones.Length; boneIndex < n; boneIndex++) {
                    var unityBone = bones[boneIndex];
                    var fbxBone = MapUnityObjectToFbxNode[unityBone.gameObject];

                    Matrix4x4 pose;
                    if (fbxBone.GetSkeleton().GetSkeletonType() == FbxSkeleton.EType.eRoot) {
                        // bind pose is local -> root. We want root -> local, so invert.
                        pose = bindPoses[boneIndex].inverse; // assuming parent is identity matrix
                    } else {
                        // Bind pose is local -> parent -> ... -> root.
                        // We want parent -> local.
                        // Invert our bind pose to get root -> local.
                        // The apply parent -> root to leave just parent -> local.
                        pose = bindPoses[index[unityBone.parent]] * bindPoses[boneIndex].inverse;
                    }

                    // FBX is transposed relative to Unity: transpose as we convert.
                    FbxMatrix matrix = new FbxMatrix ();
                    matrix.SetColumn (0, new FbxVector4 (pose.GetRow (0).x, pose.GetRow (0).y, pose.GetRow (0).z, pose.GetRow (0).w));
                    matrix.SetColumn (1, new FbxVector4 (pose.GetRow (1).x, pose.GetRow (1).y, pose.GetRow (1).z, pose.GetRow (1).w));
                    matrix.SetColumn (2, new FbxVector4 (pose.GetRow (2).x, pose.GetRow (2).y, pose.GetRow (2).z, pose.GetRow (2).w));
                    matrix.SetColumn (3, new FbxVector4 (pose.GetRow (3).x, pose.GetRow (3).y, pose.GetRow (3).z, pose.GetRow (3).w));

                    // FBX wants translation, rotation (in euler angles) and scale.
                    // We assume there's no real shear, just rounding error.
                    FbxVector4 translation, rotation, shear, scale;
                    double sign;
                    matrix.GetElements (out translation, out rotation, out shear, out scale, out sign);

                    // Bones should have zero rotation, and use a pivot instead.
                    fbxBone.LclTranslation.Set (new FbxDouble3(translation.X, translation.Y, translation.Z));
                    fbxBone.LclRotation.Set (new FbxDouble3(0,0,0));
                    fbxBone.LclScaling.Set (new FbxDouble3 (scale.X, scale.Y, scale.Z));

                    fbxBone.SetRotationActive (true);
                    fbxBone.SetPivotState (FbxNode.EPivotSet.eSourcePivot, FbxNode.EPivotState.ePivotReference);
                    fbxBone.SetPreRotation (FbxNode.EPivotSet.eSourcePivot, new FbxVector4 (rotation.X, rotation.Y, rotation.Z));
                }
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
                FbxAnimCurve fbxAnimCurve = fbxProperty.GetCurve (fbxAnimLayer, fbxName.Channel, true);

                // copy Unity AnimCurve to FBX AnimCurve.
                fbxAnimCurve.KeyModifyBegin();

                for(int keyIndex = 0, n = unityAnimCurve.length; keyIndex < n; ++keyIndex) {
                    var key = unityAnimCurve[keyIndex];
                    var fbxTime = FbxTime.FromSecondDouble(key.time);
                    fbxAnimCurve.KeyAdd (fbxTime);
                    fbxAnimCurve.KeySet (keyIndex, fbxTime, key.value);
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
                        /* Some normal property (e.g. translation), export right away */
                        ExportAnimCurve (unityObj, unityAnimCurve, unityCurveBinding.propertyName,
                                fbxScene, fbxAnimLayer);
                    } else {
                        /* Rotation property; save it to convert quaternion -> euler later. */
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
                    quat.Animate(fbxNode, fbxAnimLayer, Verbose);
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

                // Only export each clip once per game object.
                var exported = new HashSet<AnimationClip>();
                foreach (var clip in controller.animationClips) {
                    if (exported.Add(clip)) {
                        ExportAnimationClip(clip, unityRoot, fbxScene);
                    }
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
                        if (!unityGo) { continue; }

                        // Create the node itself, plus the hierarchy below it.
                        // This creates MapUnityObjectToFbxNode.
                        CreateHierarchy (unityGo, fbxScene, fbxRootNode);
                    }

                    // Export skeletons.
                    foreach (var unityGo in MapUnityObjectToFbxNode.Keys) {
                        ExportSkeleton(unityGo, fbxScene);
                    }

                    // Export animations.
                    foreach (var unityGo in MapUnityObjectToFbxNode.Keys) {
                        ExportAnimationClips(unityGo, fbxScene);
                    }

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
