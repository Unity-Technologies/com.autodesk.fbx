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
            /// Map a Unity property name to the corresponding FBX property and
            /// channel names.
            /// </summary>
            /// 
            static Dictionary<string, FbxPropertyChannelPair> MapUnityPropertyNameToFbx = new Dictionary<string, FbxPropertyChannelPair> ()
            {
                { "m_Color.r",              new FbxPropertyChannelPair (FbxNodeAttribute.sColor, Globals.FBXSDK_CURVENODE_COLOR_RED) },
                { "m_Color.g",              new FbxPropertyChannelPair (FbxNodeAttribute.sColor, Globals.FBXSDK_CURVENODE_COLOR_GREEN) },
                { "m_Color.b",              new FbxPropertyChannelPair (FbxNodeAttribute.sColor, Globals.FBXSDK_CURVENODE_COLOR_BLUE) },
                // ignore m_Color.a; there's no mapping to the FbxLight.Color; which is a Double3
                { "m_Intensity",            new FbxPropertyChannelPair ("Intensity") },
                { "m_SpotAngle",            new FbxPropertyChannelPair ("InnerAngle") },

                // mapped these to custom properties e.g. Unity_ColorTemperature
                { "m_ColorTemperature",     new FbxPropertyChannelPair(MakeName("colorTemperature")) }, 
                { "m_CookieSize",           new FbxPropertyChannelPair(MakeName("cookieSize")) },

                { "m_LocalPosition.x",      new FbxPropertyChannelPair("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_X) },
                { "m_LocalPosition.y",      new FbxPropertyChannelPair("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_Y) },
                { "m_LocalPosition.z",      new FbxPropertyChannelPair("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_Z) },
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

                // Export bounceIntensity as custom property
                // NOTE: export on fbxNode so that it will show up in Maya
                ExportFloatProperty (fbxNode, unityLight.bounceIntensity, 
                                     MakeName("bounceIntensity"), 
                                     "The multiplier that defines the strength of the bounce lighting.");

                // color             The color of the light.
                var unityLightColor = unityLight.color;

                fbxLight.Color.Set (new FbxDouble3(unityLightColor.r, unityLightColor.g, unityLightColor.b));

                // Export colorTemperature as custom property
                ExportFloatProperty (fbxNode, unityLight.colorTemperature,
                                     MakeName("colorTemperature"),
                                     "The color temperature of the light. Correlated Color Temperature (abbreviated as CCT) is multiplied with the color filter when calculating the final color of a light source.The color temperature of the electromagnetic radiation emitted from an ideal black body is defined as its surface temperature in Kelvin.White is 6500K according to the D65 standard. Candle light is 1800K.If you want to use lightsUseCCT, lightsUseLinearIntensity has to be enabled to ensure physically correct output. See Also: GraphicsSettings.lightsUseLinearIntensity, GraphicsSettings.lightsUseCCT.");

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

                // Export cookieSize as custom property
                ExportFloatProperty (fbxNode, unityLight.cookieSize,
                                     MakeName("cookieSize"),
                                     "The size of a directional light's cookie.");

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
            /// Export Unity Property as a Float Property
            /// </summary>
            FbxProperty ExportFloatProperty (FbxObject fbxObject, float value, string name, string label )
            {
            	// add (not particularly useful) custom data: how many Unity
            	// components does the unity object have?
            	var fbxProperty = FbxProperty.Create (fbxObject, Globals.FbxDoubleDT, name, label);
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
                if (unityGo==null)
                {
                    Debug.LogError(string.Format("cannot convert to GameObject from {0}",unityObj.ToString()));
                    return;
                }

                FbxNode fbxNode;
                if (!MapUnityObjectToFbxNode.TryGetValue(unityGo, out fbxNode)) {
                    Debug.LogError(string.Format ("cannot find fbxNode for {0}", unityGo.ToString()));
                    return;
                }

                FbxProperty fbxProperty = null;

                // try finding unity property name on node attribute
                FbxNodeAttribute fbxNodeAttribute = fbxNode.GetNodeAttribute ();
                if (fbxNodeAttribute != null) {
                    fbxProperty = fbxNodeAttribute.FindProperty (fbxPair.Property, false);
                }

                // try finding unity property on the node
                if (fbxProperty==null || !fbxProperty.IsValid()) {
                    fbxProperty = fbxNode.FindProperty(fbxPair.Property, false);
                }

                if (fbxProperty==null || !fbxProperty.IsValid ()) {
                    Debug.LogError (string.Format ("cannot find fbxProperty {0} on {1}", fbxPair.Property, fbxNode.GetName ()));
                    return;
                }

                if (Verbose) {
                    Debug.Log ( string.Format("Exporting animation for {0} ({1})", 
                                              unityObj.ToString(), 
                                              fbxPair.Property));
                }

                // Create the AnimCurve on the channel
                FbxAnimCurve fbxAnimCurve = (fbxPair.Channel != null)
                    ? fbxProperty.GetCurve (fbxAnimLayer, fbxPair.Channel, true)
                                 : fbxProperty.GetCurve (fbxAnimLayer, true);
                
                // copy Unity AnimCurve to FBX AnimCurve.
                fbxAnimCurve.KeyModifyBegin ();

                for (int keyIndex = 0, n = unityAnimCurve.length; keyIndex < n; ++keyIndex) 
                {
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

                foreach (EditorCurveBinding unityCurveBinding in AnimationUtility.GetCurveBindings(unityAnimClip))
                {
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
                foreach (var unityAnimClip in unityAnimController.animationClips) 
                {
                    if (unityExportedAnimClip.Add (unityAnimClip)) 
                    {
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
                foreach (var unityGo in MapUnityObjectToFbxNode.Keys) 
                {
                    ExportAnimationClips (unityGo, fbxScene);
                }
            }

            /// <summary>
            /// configures ambient lighting for the scene
            /// </summary>
            protected void SetAmbientLighting (FbxScene fbxScene)
            {
                Color unityColor = RenderSettings.ambientLight;

                fbxScene.GetGlobalSettings ().SetAmbientColor (new FbxColor (unityColor.r, unityColor.g, unityColor.b));
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

                public bool IsValid { get { return Property.Length>0; } }

            };
            
            /// <summary>
            /// keep a map between GameObject and FbxNode for quick lookup when we export
            /// animation.
            /// </summary>
            Dictionary<GameObject, FbxNode> MapUnityObjectToFbxNode = new Dictionary<GameObject, FbxNode> ();

            ///<summary>
            ///Information about the mesh that is important for exporting.
            ///</summary>
            public struct MeshInfo
            {
                /// <summary>
                /// The transform of the mesh.
                /// </summary>
                public Matrix4x4 xform;
                public Mesh mesh;

                /// <summary>
                /// The gameobject in the scene to which this mesh is attached.
                /// This can be null: don't rely on it existing!
                /// </summary>
                public GameObject unityObject;

                /// <summary>
                /// Return true if there's a valid mesh information
                /// </summary>
                /// <value>The vertex count.</value>
                public bool IsValid { get { return mesh != null; } }

                /// <summary>
                /// Gets the vertex count.
                /// </summary>
                /// <value>The vertex count.</value>
                public int VertexCount { get { return mesh.vertexCount; } }

                /// <summary>
                /// Gets the triangles. Each triangle is represented as 3 indices from the vertices array.
                /// Ex: if triangles = [3,4,2], then we have one triangle with vertices vertices[3], vertices[4], and vertices[2]
                /// </summary>
                /// <value>The triangles.</value>
                public int [] Triangles { get { return mesh.triangles; } }

                /// <summary>
                /// Gets the vertices, represented in local coordinates.
                /// </summary>
                /// <value>The vertices.</value>
                public Vector3 [] Vertices { get { return mesh.vertices; } }

                /// <summary>
                /// Gets the normals for the vertices.
                /// </summary>
                /// <value>The normals.</value>
                public Vector3 [] Normals { get { return mesh.normals; } }

                /// <summary>
                /// TODO: Gets the binormals for the vertices.
                /// </summary>
                /// <value>The normals.</value>
                private Vector3 [] m_Binormals;
                public Vector3 [] Binormals {
                    get {
                        /// NOTE: LINQ
                        ///    return mesh.normals.Zip (mesh.tangents, (first, second)
                        ///    => Math.cross (normal, tangent.xyz) * tangent.w
                        if (m_Binormals.Length == 0) {
                            m_Binormals = new Vector3 [mesh.normals.Length];

                            for (int i = 0; i < mesh.normals.Length; i++)
                                m_Binormals [i] = Vector3.Cross (mesh.normals [i],
                                                                 mesh.tangents [i])
                                                         * mesh.tangents [i].w;

                        }
                        return m_Binormals;
                    }
                }

                /// <summary>
                /// TODO: Gets the tangents for the vertices.
                /// </summary>
                /// <value>The tangents.</value>
                public Vector4 [] Tangents { get { return mesh.tangents; } }

                /// <summary>
                /// TODO: Gets the tangents for the vertices.
                /// </summary>
                /// <value>The tangents.</value>
                public Color [] VertexColors { get { return mesh.colors; } }

                /// <summary>
                /// Gets the uvs.
                /// </summary>
                /// <value>The uv.</value>
                public Vector2 [] UV { get { return mesh.uv; } }

                /// <summary>
                /// The material used, if any; otherwise null.
                /// We don't support multiple materials on one gameobject.
                /// </summary>
                public Material Material {
                    get {
                        if (!unityObject) { return null; }
                        var renderer = unityObject.GetComponent<Renderer> ();
                        if (!renderer) { return null; }
                        // .material instantiates a new material, which is bad
                        // most of the time.
                        return renderer.sharedMaterial;
                    }
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="MeshInfo"/> struct.
                /// </summary>
                /// <param name="mesh">A mesh we want to export</param>
                public MeshInfo (Mesh mesh)
                {
                    this.mesh = mesh;
                    this.xform = Matrix4x4.identity;
                    this.unityObject = null;
                    this.m_Binormals = null;
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="MeshInfo"/> struct.
                /// </summary>
                /// <param name="gameObject">The GameObject the mesh is attached to.</param>
                /// <param name="mesh">A mesh we want to export</param>
                public MeshInfo (GameObject gameObject, Mesh mesh)
                {
                    this.mesh = mesh;
                    this.xform = gameObject.transform.localToWorldMatrix;
                    this.unityObject = gameObject;
                    this.m_Binormals = null;
                }
            }

            /// <summary>
            /// Get a mesh renderer's mesh.
            /// </summary>
            private MeshInfo GetMeshInfo (GameObject gameObject, bool requireRenderer = true)
            {
                // Two possibilities: it's a skinned mesh, or we have a mesh filter.
                Mesh mesh;
                var meshFilter = gameObject.GetComponent<MeshFilter> ();
                if (meshFilter) {
                    mesh = meshFilter.sharedMesh;
                } else {
                    var renderer = gameObject.GetComponent<SkinnedMeshRenderer> ();
                    if (!renderer) {
                        mesh = null;
                    } else {
                        mesh = new Mesh ();
                        renderer.BakeMesh (mesh);
                    }
                }
                if (!mesh) {
                    return new MeshInfo ();
                }
                return new MeshInfo (gameObject, mesh);
            }

            /// <summary>
            /// Unconditionally export this mesh object to the file.
            /// We have decided; this mesh is definitely getting exported.
            /// </summary>
            public FbxMesh ExportMesh (GameObject unityGo, FbxScene fbxScene)
            {
                var meshInfo = GetMeshInfo (unityGo);

                if (!meshInfo.IsValid)
                    return null;

                // create the mesh structure.
                FbxMesh fbxMesh = FbxMesh.Create (fbxScene, "Scene");

                // Create control points.
                int NumControlPoints = meshInfo.VertexCount;

                fbxMesh.InitControlPoints (NumControlPoints);

                // copy control point data from Unity to FBX
                for (int v = 0; v < NumControlPoints; v++) {
                    fbxMesh.SetControlPointAt (new FbxVector4 (meshInfo.Vertices [v].x, meshInfo.Vertices [v].y, meshInfo.Vertices [v].z), v);
                }

                for (int f = 0; f < meshInfo.Triangles.Length / 3; f++) {
                    fbxMesh.BeginPolygon ();
                    fbxMesh.AddPolygon (meshInfo.Triangles [3 * f]);
                    fbxMesh.AddPolygon (meshInfo.Triangles [3 * f + 1]);
                    fbxMesh.AddPolygon (meshInfo.Triangles [3 * f + 2]);
                    fbxMesh.EndPolygon ();
                }

                return fbxMesh;
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

                var fbxMesh = ExportMesh (unityGo, fbxScene);

                if (fbxMesh!=null)
                {
                    // set the fbxNode containing the mesh
                    fbxNode.SetNodeAttribute (fbxMesh);
                    fbxNode.SetShadingMode (FbxNode.EShadingMode.eWireFrame);
                }

                FbxLight fbxLight = ExportLight (unityGo, fbxScene, fbxNode);

                if (fbxLight != null)
                {
                    fbxNode.SetNodeAttribute (fbxLight);    
                }

                if (Verbose)
                    Debug.Log (string.Format ("exporting {0}", fbxNode.GetName ()));

                fbxNodeParent.AddChild (fbxNode);

                // add mapping between fbxnode for light 
                // and unity game object for animation export
                MapUnityObjectToFbxNode [unityGo] = fbxNode;

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

                    // Export animations
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
                } else if (obj is UnityEngine.Component) {
                    var component = obj as Component;
                    return component.gameObject;
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
            /// define prefix used for new properties
            /// </summary>
            const string NamePrefix = "Unity_";

            bool Verbose { get { return true; } }

            /// <summary>
            /// manage the selection of a filename
            /// </summary>
            static string LastFilePath { get; set; }
            const string Extension = "fbx";

            private static string MakeName (string basename)
            {
            	return NamePrefix + basename;
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
