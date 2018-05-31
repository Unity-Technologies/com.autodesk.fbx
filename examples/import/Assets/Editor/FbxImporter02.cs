//#define UNI_18844
// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Autodesk.Fbx;

namespace Autodesk.Fbx.Examples
{
    namespace Editor
    {

        public class FbxImporter02 : System.IDisposable
        {
            const string Title =
                 "Example 02: import node hierarchy";

            const string Subject =
                 @"Example FbxImporter02 illustrates how to:
                                1) create and initialize the importer
                                2) import the scene
                                3) visit each node in hierarchy
                                4) construct local translation, rotation and scaling taking pre/post rotations, pivots and offsets into effect
                                4) create a Unity game object with same name, parent and, transformation matrix
                    ";

            const string Keywords =
                 "import scene node hierarchy";

            const string Comments =
                 @"";

            const string MenuItemName = "File/Import FBX/02. Import Node Hierachy";

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxImporter02 Create () { return new FbxImporter02 (); }

            int lScalingM;

            /// <summary>
            /// Process fbx scene by doing nothing
            /// </summary>
            public void ProcessNode (FbxNode fbxNode, GameObject unityParentObj = null, bool constructTransform = false)
            {
                string name = fbxNode.GetName ();

                GameObject unityGo = new GameObject (name);
                NumNodes++;

                if (unityParentObj != null) {
                    unityGo.transform.parent = unityParentObj.transform;
                }

                FbxAMatrix fbxTransform = null;

                if (constructTransform) {
#if UNI_18844
                    // Construct rotation matrices
                    FbxVector4 fbxRotation = new FbxVector4 (fbxNode.LclRotation.Get ());
                    FbxAMatrix fbxRotationM = new FbxAMatrix ();
                    fbxRotationM.SetR(fbxRotation);

                    FbxVector4 fbxPreRotation = new FbxVector4 (fbxNode.PreRotation.Get ());
                    FbxAMatrix fbxPreRotationM = new FbxAMatrix ();
                    fbxPreRotationM.SetR(fbxPreRotation);

                    FbxVector4 fbxPostRotation = new FbxVector4 (fbxNode.PostRotation.Get ());
                    FbxAMatrix fbxPostRotationM = new FbxAMatrix ();
                    fbxPostRotationM.SetR(fbxPostRotation);

                    // Construct translation matrix
                    FbxAMatrix fbxTranslationM = new FbxAMatrix ();
                    FbxVector4 fbxTranslation = new FbxVector4 (fbxNode.LclTranslation.Get ());
                    fbxTranslationM.SetT(fbxTranslation);

                    // Construct scaling matrix
                    FbxAMatrix fbxScalingM = new FbxAMatrix ();
                    FbxVector4 fbxScaling = new FbxVector4 (fbxNode.LclScaling.Get ());
                    fbxScalingM.SetS(fbxScaling);

                    // Construct offset and pivot matrices
                    FbxAMatrix fbxRotationOffsetM = new FbxAMatrix ();
                    FbxVector4 fbxRotationOffset = fbxNode.RotationOffset.Get();
                    fbxRotationOffsetM.SetT(fbxRotationOffset);

                    FbxAMatrix fbxRotationPivotM = new FbxAMatrix ();
                    FbxVector4 fbxRotationPivot = fbxNode.RotationPivot.Get();
                    fbxRotationPivotM.SetT(fbxRotationPivot);

                    FbxAMatrix fbxScalingOffsetM = new FbxAMatrix ();
                    FbxVector4 fbxScalingOffset = fbxNode.ScalingOffset.Get ();
                    fbxScalingOffsetM.SetT(fbxScalingOffset);

                    FbxAMatrix fbxScalingPivotM = new FbxAMatrix ();
                    FbxVector4 fbxScalingPivot = fbxNode.ScalingPivot.Get ();
                    fbxScalingPivotM.SetT(fbxScalingPivot);

                    fbxTransform = 
                        fbxTranslationM * 
                        fbxRotationOffsetM * 
                        fbxRotationPivotM * 
                        fbxPreRotationM * 
                        fbxRotationM * 
                        fbxPostRotationM *
                        fbxRotationPivotM.Inverse () *
                        fbxScalingOffsetM * 
                        fbxScalingPivotM * 
                        fbxScalingM *
                        fbxScalingPivotM.Inverse ();
                    
                    lclTrs = fbxTransform.GetT ();
                    lclRot = fbxTransform.GetQ ();
                    lclScl = fbxTransform.GetS ();
#endif
                } else {
                    fbxTransform = fbxNode.EvaluateLocalTransform ();
                }

                if (fbxTransform == null)
                {
                    Debug.LogError (string.Format ("failed to retrieve transform for node : {0}", fbxNode.GetName()));
                    return;
                }

                FbxVector4 lclTrs = fbxTransform.GetT ();
                FbxQuaternion lclRot = fbxTransform.GetQ ();
                FbxVector4 lclScl = fbxTransform.GetS ();

                Debug.Log (string.Format ("processing {3} Lcl : T({0}) R({1}) S({2})",
                                         lclTrs.ToString (), 
                                         lclRot.ToString (),
                                         lclScl.ToString (),
                                         fbxNode.GetName()));

                // check we can handle translation value
                Debug.Assert (lclTrs.X <= float.MaxValue && lclTrs.X >= float.MinValue);
                Debug.Assert (lclTrs.Y <= float.MaxValue && lclTrs.Y >= float.MinValue);
                Debug.Assert (lclTrs.Z <= float.MaxValue && lclTrs.Z >= float.MinValue);

                unityGo.transform.localPosition = new Vector3 ((float)lclTrs.X, (float)lclTrs.Y, (float)lclTrs.Z);
                unityGo.transform.localRotation = new Quaternion((float)lclRot[0], (float)lclRot[1], (float)lclRot[2], (float)lclRot[3]);
                unityGo.transform.localScale = new Vector3 ((float)lclScl.X, (float)lclScl.Y, (float)lclScl.Z);

                for (int i = 0; i<fbxNode.GetChildCount (); ++i) 
                {
                    ProcessNode(fbxNode.GetChild (i), unityGo);
                }
            }

            /// <summary>
            /// Convert scene's system units but leave scaling unchanged
            /// </summary>
            public void ConvertScene (FbxScene fbxScene, FbxSystemUnit toUnits)
            {
                // Get scale factor.
                float scaleFactor = 1.0f;

                scaleFactor = (float)fbxScene.GetGlobalSettings ().GetSystemUnit ().GetConversionFactorTo (toUnits);

                if (scaleFactor.Equals (1.0f))
                    return;

                // Get root node.
                FbxNode fbxRootNode = fbxScene.GetRootNode ();

                // For all the nodes to convert the translations
                Queue<FbxNode> fbxNodes = new Queue<FbxNode> ();

                fbxNodes.Enqueue (fbxRootNode);

                while (fbxNodes.Count > 0) {
                    FbxNode fbxNode = fbxNodes.Dequeue ();

                    // Convert node's translation.
                    FbxDouble3 lclTrs = fbxNode.LclTranslation.Get ();

#if UNI_18844
                    lclTrs *= scaleFactor;
                    lclTrs *= scaleFactor;
                    lclTrs *= scaleFactor;
#endif
                    fbxNode.LclTranslation.Set (lclTrs);

                    for (int i = 0; i < fbxNode.GetChildCount (); ++i) {
                        fbxNodes.Enqueue (fbxNode.GetChild (i));
                    }
                }
            }

            /// <summary>
            /// Process fbx scene by doing nothing
            /// </summary>
            public void ProcessScene (FbxScene fbxScene)
            {
                Debug.Log (string.Format ("Scene name: {0}", fbxScene.GetName()));

                var fbxSettings = fbxScene.GetGlobalSettings ();
                FbxSystemUnit fbxSystemUnit = fbxSettings.GetSystemUnit ();

                if (fbxSystemUnit != UnitySystemUnit) 
                {
                    Debug.Log (string.Format ("converting system unit to match Unity. Expected {0}, found {1}.",
                                             UnitySystemUnit, fbxSystemUnit));

                    ConvertScene (fbxScene, UnitySystemUnit);
                } 
                else 
                {
                    Debug.Log (string.Format("file system units {0}", fbxSystemUnit));
                }

                // The Unity axis system has Y up, Z forward, X to the right.
                FbxAxisSystem fbxAxisSystem = fbxSettings.GetAxisSystem ();

                if (fbxAxisSystem != UnityAxisSystem) {
                    Debug.LogWarning (string.Format ("file axis system do not match Unity, Expected {0} found {1}",
                                                     AxisSystemToString (UnityAxisSystem),
                                                     AxisSystemToString (fbxAxisSystem)));
                }

                ProcessNode (fbxScene.GetRootNode ());

                return;
            }


            /// <summary>
            /// Import all from scene.
            /// Return the number of objects we imported.
            /// </summary>
            public int ImportAll (IEnumerable<Object> unitySelectionSet)
            {
                // Create the FBX manager
                using (var fbxManager = FbxManager.Create ()) 
                {
                    FbxIOSettings fbxIOSettings = FbxIOSettings.Create (fbxManager, Globals.IOSROOT);

                    // Configure the IO settings.
                    fbxManager.SetIOSettings (fbxIOSettings);

                    // Get the version number of the FBX files generated by the
                    // version of FBX SDK that you are using.
                    int sdkMajor = -1, sdkMinor = -1, sdkRevision = -1;

                    FbxManager.GetFileFormatVersion (out sdkMajor, out sdkMinor, out sdkRevision);

                    // Create the importer 
                    var fbxImporter = FbxImporter.Create (fbxManager, "Importer");

                    // Initialize the importer.
                    int fileFormat = -1;

                    bool status = fbxImporter.Initialize (LastFilePath, fileFormat, fbxIOSettings);
                    FbxStatus fbxStatus = fbxImporter.GetStatus ();

                    // Get the version number of the FBX file format.
                    int fileMajor = -1, fileMinor = -1, fileRevision = -1;
                    fbxImporter.GetFileVersion (out fileMajor, out fileMinor, out fileRevision);

                    // Check that initialization of the fbxImporter was successful
                    if (!status) 
                    {
                        Debug.LogError (string.Format ("failed to initialize FbxImporter, error returned {0}", 
                                                       fbxStatus.GetErrorString ()));

                        if (fbxStatus.GetCode () == FbxStatus.EStatusCode.eInvalidFileVersion) 
                        {
                            Debug.LogError (string.Format ("Invalid file version detected\nSDK version: {0}.{1}.{2}\nFile version: {3}.{4}.{5}",
                                                           sdkMajor, sdkMinor, sdkRevision, 
                                                           fileMajor, fileMinor, fileRevision));

                        }

                        return 0;
                    }

                    // Import options. Determine what kind of data is to be imported.
                    // The default is true, but here we set the options explictly.
                    fbxIOSettings.SetBoolProp(Globals.IMP_FBX_MATERIAL,         false);
                    fbxIOSettings.SetBoolProp(Globals.IMP_FBX_TEXTURE,          false);
                    fbxIOSettings.SetBoolProp(Globals.IMP_FBX_ANIMATION,        false);
                    fbxIOSettings.SetBoolProp(Globals.IMP_FBX_GLOBAL_SETTINGS,  true);

                    // Create a scene
                    var fbxScene = FbxScene.Create (fbxManager, "Scene");

                    // Import the scene to the file.
                    status = fbxImporter.Import (fbxScene);

                    if (status == false) 
                    {
                        Debug.LogError (string.Format ("failed to import file ({0})", 
                                                       fbxImporter.GetStatus ().GetErrorString ()));
                    } 
                    else 
                    {
                        // import data into scene 
                        ProcessScene (fbxScene);
                    }

                    // cleanup
                    fbxScene.Destroy ();
                    fbxImporter.Destroy ();

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
                OnImport();
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
            /// Number of nodes imported including siblings and decendents
            /// </summary>
            public int NumNodes { private set; get; }

            /// <summary>
            /// Clean up this class on garbage collection
            /// </summary>
            public void Dispose () { }

            public bool Verbose { private set; get; }

            /// <summary>
            /// manage the selection of a filename
            /// </summary>
            static string LastFilePath { get; set; }
            const string kExtension = "fbx";
            const string kBorderLine = "--------------------\n";
            const string kNewLine = "\n";
            const string kPadding = "    ";

            private static string MakeFileName(string basename = "test", string extension = "fbx")
            {
                return basename + "." + extension;
            }

            private FbxSystemUnit UnitySystemUnit { get { return FbxSystemUnit.m; } }

            private FbxAxisSystem UnityAxisSystem { 
                get { return new FbxAxisSystem (FbxAxisSystem.EUpVector.eYAxis, 
                                                FbxAxisSystem.EFrontVector.eParityOdd, 
                                                FbxAxisSystem.ECoordSystem.eLeftHanded); }
            }

            private static string AxisSystemToString(FbxAxisSystem fbxAxisSystem) {
                return string.Format ("[{0}, {1}, {2}]",
                                      fbxAxisSystem.GetUpVector ().ToString (),
                                      fbxAxisSystem.GetFrontVector ().ToString (),
                                      fbxAxisSystem.GetCoorSystem ().ToString ());
            }

            // use the SaveFile panel to allow user to enter a file name
            private static void OnImport()
            {
                // Now that we know we have stuff to import, get the user-desired path.
                var directory = string.IsNullOrEmpty (LastFilePath) 
                                      ? Application.dataPath 
                                      : Path.GetDirectoryName (LastFilePath);
                
                var title = "Import FBX";

                var filePath = EditorUtility.OpenFilePanel(title, directory, kExtension);

                if (string.IsNullOrEmpty (filePath)) 
                {
                    return;
                }

                LastFilePath = filePath;

                using (var fbxImporter = Create()) 
                {
                    if (fbxImporter.ImportAll(Selection.objects) > 0)
                    {
                        Debug.Log (string.Format ("Successfully imported: {0}", filePath));
                    }
                }
            }
        }
    }
}
