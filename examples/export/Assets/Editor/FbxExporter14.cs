//#define UNI_17561
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

        public class FbxExporter14 : System.IDisposable
        {
            const string Title =
                "Example 14: exporting selected lights with their settings and animation.";

            const string Subject =
                 @"Example     FbxExporter14 illustrates how to:
                                            1) create and initialize an exporter
                                            2) create a scene
                                            3) create a light and set its gobo (stencil or template that is placed inside or in front of a light source)
                                            4) create animation take for the animated light settings and SRT
                                            5) set the ambient light for the scene
                                            6) export a scene to a FBX file (FBX201400 compatible, ASCII)
                                                    ";

            const string Keywords =
                 "export light node animation";

            const string Comments =
                 @"";

            const string MenuItemName = "File/Export FBX/14. lights with animation";

            const string FileBaseName = "example_lights_with_animation";

            static Dictionary<UnityEngine.LightType, FbxLight.EType> MapLightType = new Dictionary<UnityEngine.LightType, FbxLight.EType> () {
                { UnityEngine.LightType.Directional,    FbxLight.EType.eDirectional },
                { UnityEngine.LightType.Spot,           FbxLight.EType.eSpot },
                { UnityEngine.LightType.Point,          FbxLight.EType.ePoint },
                { UnityEngine.LightType.Area,           FbxLight.EType.eArea },
            };

            /// <summary>
            /// collected list of lights to export
            /// </summary>
            List<Light> Lights = new List<Light> ();

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter14 Create () { return new FbxExporter14 (); }

            /// <summary>
            /// Exports light component.
            /// Supported types: point, spot and directional
            /// Cookie => Gobo
            /// </summary>
            protected FbxLight ExportLight (GameObject unityGo, FbxScene fbxScene, FbxNode fbxNode)
            {
                Light unityLight = unityGo.GetComponent<Light> ();

                if (unityLight == null)
                    return null;

                FbxLight.EType fbxLightType;

                // Is light type supported?
                if (!MapLightType.TryGetValue (unityLight.type, out fbxLightType))
                    return null;

                // add to the list of lights
                Lights.Add(unityLight);

                FbxLight fbxLight = FbxLight.Create (fbxScene.GetFbxManager (), unityLight.name);

                // Set the type of the light.      
                fbxLight.LightType.Set(fbxLightType);

                switch (unityLight.type) 
                {
                    case LightType.Directional : {
                        break;
                    }
                    case LightType.Spot : {
                        // Set the angle of the light's spotlight cone in degrees.
                        fbxLight.InnerAngle.Set(unityLight.spotAngle);
                        fbxLight.OuterAngle.Set(unityLight.spotAngle);
                        break;
                    }
                    case LightType.Point : {
                        break;
                    }
                    case LightType.Area : {
                        // TODO: areaSize          The size of the area light by scaling the node XY
                        
                        break;
                    }
                }

                // TODO: bounceIntensity   The multiplier that defines the strength of the bounce lighting.

                // color             The color of the light.
                var unityLightColor = unityLight.color;

                fbxLight.Color.Set (new FbxDouble3(unityLightColor.r, unityLightColor.g, unityLightColor.b));

                // TODO: colorTemperature  The color temperature of the light. Correlated Color Temperature (abbreviated as CCT) is multiplied with the color filter when calculating the final color of a light source.The color temperature of the electromagnetic radiation emitted from an ideal black body is defined as its surface temperature in Kelvin.White is 6500K according to the D65 standard. Candle light is 1800K.If you want to use lightsUseCCT, lightsUseLinearIntensity has to be enabled to ensure physically correct output. See Also: GraphicsSettings.lightsUseLinearIntensity, GraphicsSettings.lightsUseCCT.
                
                // TODO: commandBufferCount Number of command buffers set up on this light (Read Only).

                // cookie            The cookie texture projected by the light.
                var unityCookieTexture = unityLight.cookie;

                if (unityCookieTexture !=null)
                {
                    // Find its filename
                    var textureSourceFullPath = AssetDatabase.GetAssetPath (unityCookieTexture);
                    if (textureSourceFullPath != "") {
                        
                        // get absolute filepath to texture
                        textureSourceFullPath = Path.GetFullPath (textureSourceFullPath);

                        fbxLight.FileName.Set (textureSourceFullPath);
                        fbxLight.DrawGroundProjection.Set (true);
                        fbxLight.DrawVolumetricLight.Set (true);
                        fbxLight.DrawFrontFacingVolumetricLight.Set (false);
                    }
                }

                // TODO: cookieSize        The size of a directional light's cookie.

                // TODO: cullingMask       This is used to light certain objects in the scene selectively.
                // TODO: flare             The flare asset to use for this light.

                // Set the Intensity of a light is multiplied with the Light color.
                fbxLight.Intensity.Set (unityLight.intensity * 100.0f /*compensate for Maya scaling by system units*/ );

                // TODO: isBaked           Is the light contribution already stored in lightmaps and/or lightprobes (Read Only).
                // TODO: lightmapBakeType  This property describes what part of a light's contribution can be baked.

                // Set the range of the light.
                // applies-to: Point & Spot
                // => FarAttenuationStart, FarAttenuationEnd
                fbxLight.FarAttenuationStart.Set (0.01f /* none zero start */);
                fbxLight.FarAttenuationEnd.Set(unityLight.range);

                // TODO: renderMode        How to render the light.

                // shadows           Set how this light casts shadows
                // applies-to: Point & Spot
                bool unityLightCastShadows = unityLight.shadows != LightShadows.None;
                fbxLight.CastShadows.Set (unityLightCastShadows);

                // TODO: shadowBias        Shadow mapping constant bias.
                // TODO: shadowCustomResolution The custom resolution of the shadow map.
                // TODO: shadowNearPlane   Near plane value to use for shadow frustums.
                // TODO: shadowNormalBias  Shadow mapping normal-based bias.
                // TODO: shadowResolution  The resolution of the shadow map.
                // TODO: shadowStrength    Strength of light's shadows.

                return fbxLight;
            }

            /// <summary>
            /// Export each animationclip as a single fbxtake
            /// </summary>
            protected void ExportAnimationClips (GameObject unityGo, FbxScene fbxScene)
            {
                // TODO: cut and paste animation code here
            }

            /// <summary>
            /// configures ambient lighting for the scene
            /// </summary>
            protected void SetAmbientLighting (FbxScene fbxScene)
            {
                // if we're using flat lighting copy the color across
                if (RenderSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Flat) {
                    Color unityColor = RenderSettings.ambientLight;

                    fbxScene.GetGlobalSettings ().SetAmbientColor (new FbxColor (unityColor.r, unityColor.g, unityColor.b));
                }
            }

            /// <summary>
            /// Exports all animation
            /// </summary>
            protected void ExportAllAnimation(FbxScene fbxScene)
            {
                foreach (Light unityLight in this.Lights) 
                {
                    ExportAnimationClips (unityLight.gameObject, fbxScene);
                }
            }

            /// <summary>
            /// Exports the game object has a light component
            /// </summary>
            protected void ExportComponents (GameObject  unityGo, FbxScene fbxScene, FbxNode fbxNodeParent)
            {
                // create an node and add it as a child of parent
                FbxNode fbxNode = FbxNode.Create (fbxScene,  unityGo.name);
                NumNodes++;

                ExportTransform (unityGo.transform, fbxNode);

                FbxLight fbxLight = ExportLight (unityGo, fbxScene, fbxNode);

                if (fbxLight != null)
                {
                    fbxNode.SetNodeAttribute (fbxLight);    
                }

                if (Verbose)
                    Debug.Log (string.Format ("exporting {0}", fbxNode.GetName ()));

                fbxNodeParent.AddChild (fbxNode);

                // now  unityGo  through our children and recurse
                foreach (Transform childT in  unityGo.transform) 
                {
                    ExportComponents (childT.gameObject, fbxScene, fbxNode);
                }

                return;
            }

            /// <summary>
            /// Export GameObject's Transform component
            /// </summary>
            protected void ExportTransform (Transform unityTransform, FbxNode fbxNode)
            {
            	// get local position of fbxNode (from Unity)
            	UnityEngine.Vector3 unityTranslate = unityTransform.localPosition;
            	UnityEngine.Vector3 unityRotate = unityTransform.localRotation.eulerAngles;
            	UnityEngine.Vector3 unityScale = unityTransform.localScale;

            	// transfer transform data from Unity to Fbx
            	var fbxTranslate = new FbxDouble3 (unityTranslate.x, unityTranslate.y, unityTranslate.z);
            	var fbxRotate = new FbxDouble3 (unityRotate.x, unityRotate.y, unityRotate.z);
            	var fbxScale = new FbxDouble3 (unityScale.x, unityScale.y, unityScale.z);

            	// set the local position of fbxNode
            	fbxNode.LclTranslation.Set (fbxTranslate);
            	fbxNode.LclRotation.Set (fbxRotate);
            	fbxNode.LclScaling.Set (fbxScale);

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

                    var fbxSettings = fbxScene.GetGlobalSettings ();
                    /// Set system units : Unity unit is meters
                    fbxSettings.SetSystemUnit(FbxSystemUnit.m); 
                    /// Set axis system : Unity Y Up, Z Forward, X Right (left-handed with odd parity)
                    /// The Maya axis system has Y up, Z forward, X Left (right handed system with odd parity).
                    /// We export right-handed for Maya because ConvertScene can't switch handedness:
                    /// https://forums.autodesk.com/t5/fbx-forum/get-confused-with-fbxaxissystem-convertscene/td-p/4265472
                    /// NOTE: models will flipped about the -X axis.
                    fbxSettings.SetAxisSystem(FbxAxisSystem.MayaYUp);

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

                    // Set the scene's ambient lighting.
                    SetAmbientLighting (fbxScene);

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
                return (Selection.activeTransform != null);
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
