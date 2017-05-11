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

            const string MenuItemName = "File/Export FBX/WIP 14. lights with animation";

            const string FileBaseName = "example_lights_with_animation";

            /// <summary>
            /// map Unity animatable property to FbxProperty
            /// TODO: intrinsic properties, check can we find by them name?
            /// </summary>
            static Dictionary<string, string> MapUnityToFbxPropertyName = new Dictionary<string, string> ()
            {
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
                { "m_LocalPosition.x",      FbxSdk.Globals.FBXSDK_CURVENODE_COMPONENT_X },
                { "m_LocalPosition.y",      FbxSdk.Globals.FBXSDK_CURVENODE_COMPONENT_Y },
                { "m_LocalPosition.z",      FbxSdk.Globals.FBXSDK_CURVENODE_COMPONENT_Z },
                { "localEulerAnglesRaw.x",  FbxSdk.Globals.FBXSDK_CURVENODE_COMPONENT_X },
                { "localEulerAnglesRaw.y",  FbxSdk.Globals.FBXSDK_CURVENODE_COMPONENT_Y },
                { "localEulerAnglesRaw.z",  FbxSdk.Globals.FBXSDK_CURVENODE_COMPONENT_Z },
            };

#if UNI_17561
            static Dictionary<UnityEngine.LightType, FbxLight.Type> MapLightType = new Dictionary<UnityEngine.LightType, FbxLight.Type> () {
                { UnityEngine.LightType.Directional,    FbxLight.Type.eDirectional },
                { UnityEngine.LightType.Spot,           FbxLight.Type.eSpot },
                { UnityEngine.LightType.Point,          FbxLight.Type.ePoint },
            };
#endif
            static Dictionary<string, float> MapScalingFactor = new Dictionary<string, float> () {
				{ "intensity",    1.0f },
				{ "spotAngle",    1.0f },
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
            protected void ExportLight (Light unityLight, FbxScene fbxScene, FbxNode fbxNode)
            {
#if UNI_17561
                FbxLight fbxLight = FbxLight.Create (fbxScene.GetFbxManager(), MakeObjectName(unityLight.name));

                FbxLight.EType fbxLightType = null;

                // is light type supported
                if (!MapLightTypes.TryGetValue (unityLight.type, out fbxLightType))
                    return;
                
                //type The type of the light.      
                fbxLight.LightType.Set(fbxLightType);

                switch (unityLight.type) 
                {
                    case LightType.Directional : {
                        break;
                    }
                    case LightType.Spot : {
                        // Set the angle of the light's spotlight cone in degrees.
                        fbxLight.InnerAngle.Set(0.01f);
                        fbxLight.OuterAngle.Set(unityLight.spotAngle * MapScalingFactor["spotAngle"]);
                        break;
                    }
                    case LightType.Point : {
                        break;
                    }
                }

                // areaSize          The size of the area light.

                // bounceIntensity   The multiplier that defines the strength of the bounce lighting.

                // color             The color of the light.
                var unityLightColor = unityLight.color;

                fbxLight.Color.Set (unityLightColor.r, unityLightColor.g, unityLightColor.b);

                // colorTemperature  The color temperature of the light. Correlated Color Temperature (abbreviated as CCT) is multiplied with the color filter when calculating the final color of a light source.The color temperature of the electromagnetic radiation emitted from an ideal black body is defined as its surface temperature in Kelvin.White is 6500K according to the D65 standard. Candle light is 1800K.If you want to use lightsUseCCT, lightsUseLinearIntensity has to be enabled to ensure physically correct output. See Also: GraphicsSettings.lightsUseLinearIntensity, GraphicsSettings.lightsUseCCT.
                // commandBufferCount Number of command buffers set up on this light (Read Only).

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

                // cookieSize        The size of a directional light's cookie.

                // cullingMask       This is used to light certain objects in the scene selectively.
                // flare             The flare asset to use for this light.

                // Set the Intensity of a light is multiplied with the Light color.
                fbxLight.Intensity.Set (unityLight.intensity * MapScalingFactor["intensity"]);
                        
                // isBaked           Is the light contribution already stored in lightmaps and/or lightprobes (Read Only).
                // lightmapBakeType  This property describes what part of a light's contribution can be baked.

                // range             The range of the light.

                // renderMode        How to render the light.
                // shadowBias        Shadow mapping constant bias.
                // shadowCustomResolution The custom resolution of the shadow map.
                // shadowNearPlane   Near plane value to use for shadow frustums.
                // shadowNormalBias  Shadow mapping normal-based bias.
                // shadowResolution  The resolution of the shadow map.
                // shadows           How this light casts shadows
                // shadowStrength    Strength of light's shadows.

                fbxNode.SetNodeAttribute (fbxLight);
#endif
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

#if UNI_17561
                    fbxScene.GetGlobalSettings ().SetAmbientColor (FbxColor (unityColor.r, unityColor.g, unityColor.b));
#endif
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
                Light unityLight = unityGo.GetComponent<Light> ();

                if (unityLight == null)
                    return;

                // add to the list of lights
                Lights.Add(unityLight);

                // create an node and add it as a child of parent
                FbxNode fbxNode = FbxNode.Create (fbxScene,  unityGo.name);
                NumNodes++;

                ExportLight (unityLight, fbxScene, fbxNode);

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
                    int fileFormat = fbxManager.GetIOPluginRegistry ().FindWriterIDByDescription ("FBX ascii (*.fbx)");
                    bool status = fbxExporter.Initialize (LastFilePath, fileFormat, fbxManager.GetIOSettings ());
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
                if (Selection.activeTransform == null)
                    return false;

                return Selection.activeTransform.gameObject.GetComponent<Light> () != null;
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
