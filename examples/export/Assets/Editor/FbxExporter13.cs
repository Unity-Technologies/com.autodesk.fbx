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
    namespace Editor {

        public class FbxExporter13 : System.IDisposable
        {
            const string Title =
                 "Example 13: exporting a scene with instances";

            const string Subject =
                 @"Example FbxExporter13 illustrates how to:
                                1) create and initialize an exporter
                                2) create a scene
                                3) export node hierarchy with transforms
                                4) export shared mesh and material
                                5) export a scene to a FBX file (FBX201400 compatible, ASCII)
                                        ";

            const string Keywords =
                 "export scene instances mesh material texture uvs";

            const string Comments =
                 @"";

            const string MenuItemName = "File/Export FBX/13. Scene with mesh instances";
            const string MenuItemName1 = "File/Export FBX/Create Example Scene/Scene with lots of mesh instances (select model prefab)";

            const string FileBaseName = "example_mesh_instances";

            const int InstanceCount = 1000;
            const float ScaleFactor = 2.0f;
                                        
            static Dictionary<Object, FbxMesh> SharedMeshes = new Dictionary<Object, FbxMesh>();

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter13 Create () { return new FbxExporter13 (); }

            /// <summary>
            /// if this game object is a model prefab then export with shared components
            /// </summary>
            protected void ExportInstance (GameObject unityGo, FbxNode fbxNode, FbxScene fbxScene)
            {
                PrefabType unityPrefabType = PrefabUtility.GetPrefabType(unityGo);

                if (unityPrefabType != PrefabType.PrefabInstance) return;

                Object unityPrefabParent = PrefabUtility.GetPrefabParent (unityGo);

                if (Verbose)
                    Debug.Log (string.Format ("exporting instance {0}({1})", unityGo.name, unityPrefabParent.name));
                
                FbxMesh fbxMesh = null;

                if (SharedMeshes.TryGetValue (unityPrefabParent, out fbxMesh))
                {
                    fbxMesh = ExportMesh (GetMeshInfo( unityGo ), fbxNode, fbxScene);
                    SharedMeshes [unityPrefabParent] = fbxMesh;
                }
                    
                if (fbxMesh == null) return;

                // set the fbxNode containing the mesh
                fbxNode.SetNodeAttribute (fbxMesh);
                fbxNode.SetShadingMode (FbxNode.EShadingMode.eWireFrame);

                return;
            }

            /// <summary>
            /// Export components on this game object
            /// </summary>
            protected void ExportComponents (GameObject unityGo, FbxScene fbxScene, FbxNode fbxNodeParent)
            {
                if (Verbose)
                    Debug.Log (string.Format ("exporting components {0}", unityGo.name));
                
                // create an node and add it as a child of parent
                FbxNode fbxNode = FbxNode.Create (fbxScene, unityGo.name);
                NumNodes++;

                ExportTransform (unityGo.transform, fbxNode);
                ExportInstance (unityGo, fbxNode, fbxScene);
                    
                fbxNodeParent.AddChild (fbxNode);

                // now  unityGo  through our children and recurse
                foreach (Transform childT in unityGo.transform) 
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
            /// return base layer for mesh
            /// </summary>
            /// 
            private FbxLayer GetBaseLayer(FbxMesh fbxMesh)
            {
                FbxLayer fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
                if (fbxLayer == null) {
                    fbxMesh.CreateLayer ();
                    fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
                }
                return fbxLayer;
            }

            /// <summary>
            /// Export the mesh's normals, binormals and tangents using 
            /// layer 0.
            /// </summary>
            /// 
            public void ExportNormalsEtc (MeshInfo mesh, FbxMesh fbxMesh)
            {
                /// Set the Normals on Base Layer
                using (var fbxLayerElement = FbxLayerElementNormal.Create (fbxMesh, MakeObjectName ("Normals"))) 
                {
                    fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);

                    // TODO: normals for each triangle vertex instead of averaged per control point
                    //fbxNormalLayer.SetMappingMode (FbxLayerElement.eByPolygonVertex);

                    fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);

                    // Add one normal per each vertex face index (3 per triangle)
                    FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                    for (int n = 0; n < mesh.Normals.Length; n++) 
                    {
                        fbxElementArray.Add (new FbxVector4 (mesh.Normals [n] [0], 
                                                             mesh.Normals [n] [1], 
                                                             mesh.Normals [n] [2]));
                    }
                      GetBaseLayer(fbxMesh).SetNormals (fbxLayerElement);
                }

                /// Set the binormals on Layer 0. 
                using (var fbxLayerElement = FbxLayerElementBinormal.Create (fbxMesh, MakeObjectName ("Binormals"))) 
                {
                    fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);

                    // TODO: normals for each triangle vertex instead of averaged per control point
                    //fbxBinormalLayer.SetMappingMode (FbxLayerElement.eByPolygonVertex);

                    fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);

                    // Add one normal per each vertex face index (3 per triangle)
                    FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                    for (int n = 0; n < mesh.Binormals.Length; n++) {
                        fbxElementArray.Add (new FbxVector4 (mesh.Binormals [n] [0], 
                                                             mesh.Binormals [n] [1], 
                                                             mesh.Binormals [n] [2]));
                    }
                    GetBaseLayer(fbxMesh).SetBinormals (fbxLayerElement);
                }

                /// Set the tangents on Layer 0.
                using (var fbxLayerElement = FbxLayerElementTangent.Create (fbxMesh, MakeObjectName ("Tangents"))) 
                {
                    fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);

                    // TODO: normals for each triangle vertex instead of averaged per control point
                    //fbxBinormalLayer.SetMappingMode (FbxLayerElement.eByPolygonVertex);

                    fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);

                    // Add one normal per each vertex face index (3 per triangle)
                    FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                    for (int n = 0; n<mesh.Normals.Length; n++) 
                    {
                        fbxElementArray.Add (new FbxVector4 (mesh.Tangents [n] [0],
                                                             mesh.Tangents [n] [1],
                                                             mesh.Tangents [n] [2]));
                    }
                    GetBaseLayer(fbxMesh).SetTangents (fbxLayerElement);
                }
            }

            /// <summary>
            /// Export the mesh's UVs using layer 0.
            /// </summary>
            /// 
            public void ExportUVs (MeshInfo mesh, FbxMesh fbxMesh)
            {
                using (var fbxLayerElement = FbxLayerElementUV.Create (fbxMesh, MakeObjectName ("UVSet")))
                {
                    fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByPolygonVertex);
                    fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eIndexToDirect);

                    // set texture coordinates per vertex
                    FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                    for (int n = 0; n < mesh.UV.Length; n++) {
                    fbxElementArray.Add (new FbxVector2 (mesh.UV [n] [0],
                                                      mesh.UV [n] [1]));
                    }

                    // For each face index, point to a texture uv
                    FbxLayerElementArray fbxIndexArray = fbxLayerElement.GetIndexArray ();
                    fbxIndexArray.SetCount (mesh.Indices.Length);

                    for (int vertIndex = 0; vertIndex < mesh.Indices.Length; vertIndex++)
                    {
                        fbxIndexArray.SetAt (vertIndex, mesh.Indices [vertIndex]);
                    }
                    GetBaseLayer(fbxMesh).SetUVs (fbxLayerElement, FbxLayerElement.EType.eTextureDiffuse);
                }
            }

            /// <summary>
            /// Map Unity material name to FBX material object
            /// </summary>
            Dictionary<string, FbxSurfaceMaterial> MaterialMap = new Dictionary<string, FbxSurfaceMaterial> ();

            /// <summary>
            /// Map texture filename name to FBX texture object
            /// </summary>
            Dictionary<string, FbxTexture> TextureMap = new Dictionary<string, FbxTexture> ();

            /// <summary>
            /// Export an Unity Texture
            /// </summary>
            public void ExportTexture (Material unityMaterial, string unityPropName,
                FbxSurfaceMaterial fbxMaterial, string fbxPropName)
            {
                if (!unityMaterial) { return; }

                // Get the texture on this property, if any.
                if (!unityMaterial.HasProperty (unityPropName)) { return; }
                var unityTexture = unityMaterial.GetTexture (unityPropName);
                if (!unityTexture) { return; }

                // Find its filename
                var textureSourceFullPath = AssetDatabase.GetAssetPath (unityTexture);
                if (textureSourceFullPath == "") { return; }

                // get absolute path
                textureSourceFullPath = Path.GetFullPath (textureSourceFullPath);

                // Find the corresponding property on the fbx material.
                var fbxMaterialProperty = fbxMaterial.FindProperty (fbxPropName);
                if (fbxMaterialProperty == null || !fbxMaterialProperty.IsValid ()) { return; }

                // Find or create an fbx texture and link it up to the fbx material.
                if (!TextureMap.ContainsKey (textureSourceFullPath)) {
                    var fbxTexture = FbxFileTexture.Create (fbxMaterial, fbxPropName + "_Texture");
                    fbxTexture.SetFileName (textureSourceFullPath);
                    fbxTexture.SetTextureUse (FbxTexture.ETextureUse.eStandard);
                    fbxTexture.SetMappingType (FbxTexture.EMappingType.eUV);
                    TextureMap.Add (textureSourceFullPath, fbxTexture);
                }
                TextureMap [textureSourceFullPath].ConnectDstProperty (fbxMaterialProperty);
            }

            /// <summary>
            /// Get the color of a material, or grey if we can't find it.
            /// </summary>
            public FbxDouble3 GetMaterialColor (Material unityMaterial, string unityPropName)
            {
                if (!unityMaterial) { return new FbxDouble3 (0.5); }
                if (!unityMaterial.HasProperty (unityPropName)) { return new FbxDouble3 (0.5); }
                var unityColor = unityMaterial.GetColor (unityPropName);
                return new FbxDouble3 (unityColor.r, unityColor.g, unityColor.b);
            }

            /// <summary>
            /// Export (and map) a Unity PBS material to FBX classic material
            /// </summary>
            public FbxSurfaceMaterial ExportMaterial (Material unityMaterial, FbxScene fbxScene)
            {
                var materialName = unityMaterial ? unityMaterial.name : "DefaultMaterial";
                if (MaterialMap.ContainsKey (materialName)) {
                    return MaterialMap [materialName];
                }

                // We'll export either Phong or Lambert. Phong if it calls
                // itself specular, Lambert otherwise.
                var shader = unityMaterial ? unityMaterial.shader : null;
                bool specular = shader && shader.name.ToLower ().Contains ("specular");

                var fbxMaterial = specular
                    ? FbxSurfacePhong.Create (fbxScene, materialName)
                    : FbxSurfaceLambert.Create (fbxScene, materialName);

                // Copy the flat colours over from Unity standard materials to FBX.
                fbxMaterial.Diffuse.Set (GetMaterialColor (unityMaterial, "_Color"));
                fbxMaterial.Emissive.Set (GetMaterialColor (unityMaterial, "_EmissionColor"));
                fbxMaterial.Ambient.Set (new FbxDouble3 ());
                fbxMaterial.BumpFactor.Set (unityMaterial ? unityMaterial.GetFloat ("_BumpScale") : 0);
                if (specular) {
                    (fbxMaterial as FbxSurfacePhong).Specular.Set (GetMaterialColor (unityMaterial, "_SpecColor"));
                }

                // Export the textures from Unity standard materials to FBX.
                ExportTexture (unityMaterial, "_MainTex", fbxMaterial, FbxSurfaceMaterial.sDiffuse);
                ExportTexture (unityMaterial, "_EmissionMap", fbxMaterial, "emissive");
                ExportTexture (unityMaterial, "_BumpMap", fbxMaterial, FbxSurfaceMaterial.sNormalMap);
                if (specular) {
                    ExportTexture (unityMaterial, "_SpecGlosMap", fbxMaterial, FbxSurfaceMaterial.sSpecular);
                }

                MaterialMap.Add (materialName, fbxMaterial);
                return fbxMaterial;
            }

            /// <summary>
            /// Unconditionally export this mesh object to the file.
            /// We have decided; this mesh is definitely getting exported.
            /// </summary>
            public FbxMesh ExportMesh (MeshInfo meshInfo, FbxNode fbxNode, FbxScene fbxScene)
            {
                if (!meshInfo.IsValid)
                    return null;

                // create the mesh structure.
                FbxMesh fbxMesh = FbxMesh.Create (fbxScene, MakeObjectName ("Mesh"));

                // Create control points.
                int NumControlPoints = meshInfo.VertexCount;

                fbxMesh.InitControlPoints (NumControlPoints);

                // copy control point data from Unity to FBX
                for (int v = 0; v < NumControlPoints; v++) 
                {
                    fbxMesh.SetControlPointAt (new FbxVector4 (meshInfo.Vertices [v].x, meshInfo.Vertices [v].y, meshInfo.Vertices [v].z), v);
                }

                ExportNormalsEtc (meshInfo, fbxMesh);
                ExportUVs (meshInfo, fbxMesh);

                var fbxMaterial = ExportMaterial (meshInfo.Material, fbxScene);
                fbxNode.AddMaterial (fbxMaterial);

                /* 
                 * Create polygons after FbxLayerElementMaterial have been created. 
                 */
                int vId = 0;
                for (int f = 0; f < meshInfo.Triangles.Length / 3; f++) {
                    fbxMesh.BeginPolygon ();
                    fbxMesh.AddPolygon (meshInfo.Triangles [vId++]);
                    fbxMesh.AddPolygon (meshInfo.Triangles [vId++]);
                    fbxMesh.AddPolygon (meshInfo.Triangles [vId++]);
                    fbxMesh.EndPolygon ();
                }

                return fbxMesh;
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
                    var fileFormat = fbxManager.GetIOPluginRegistry().FindWriterIDByDescription("FBX ascii (*.fbx)");
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
                    FbxDocumentInfo fbxSceneInfo = FbxDocumentInfo.Create (fbxManager, MakeObjectName("SceneInfo"));

                    // set some scene info values
                    fbxSceneInfo.mTitle = Title;
                    fbxSceneInfo.mSubject = Subject;
                    fbxSceneInfo.mAuthor = "Unity Technologies";
                    fbxSceneInfo.mRevision = "1.0";
                    fbxSceneInfo.mKeywords = Keywords;
                    fbxSceneInfo.mComment = Comments;

                    fbxScene.SetSceneInfo (fbxSceneInfo);

                    // set system units, Unity unit is meters
                    var fbxSettings = fbxScene.GetGlobalSettings ();
                    fbxSettings.SetSystemUnit(FbxSystemUnit.m); 

                    // set axis system
                    // The Unity axis system has Y up, Z forward, X to the right but since
                    // FbxAxisSystem.ConvertScene doesn't work we'll set it to right handed so 
                    // that it will import into Maya right way up just mirrored.
                    var fbxAxisSystem = new FbxAxisSystem (FbxAxisSystem.EUpVector.eYAxis,
                                                           FbxAxisSystem.EFrontVector.eParityOdd,
                                                           FbxAxisSystem.ECoordSystem.eRightHanded);
                    fbxSettings.SetAxisSystem(fbxAxisSystem);

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

                    // Export the scene to the file.
                    status = fbxExporter.Export (fbxScene);

                    // cleanup
                    fbxScene.Destroy ();
                    fbxExporter.Destroy ();

                    return status == true ? NumNodes : 0;
                }
            }

            void CreateScene(IEnumerable<UnityEngine.Object> unityObjects)
            {
                GameObject unityRootGo = null;
                Dictionary<string, Material> unityMaterials = new Dictionary<string, Material> ();

                // only static mesh objects
                foreach (var unityObj in unityObjects)
                {
                    if (!IsModelPrefab (unityObj as GameObject))
                        continue;
                    
                    UnityEngine.Object unityPrefab = unityObj;

                    if (unityRootGo == null) {
                        unityRootGo = new GameObject ("Root");
                    }

                    for (int i = 0; i < InstanceCount; i++) 
                    {
                        var instance = PrefabUtility.InstantiatePrefab (unityPrefab) as GameObject;

                        instance.transform.parent = unityRootGo.transform;
                        instance.transform.position = GetRandomPosition ();
                        instance.transform.localScale = GetRandomScale ();

                        // turn on material instancing
                        if (!unityMaterials.ContainsKey (unityPrefab.name)) 
                        {
                            Renderer unityRenderer = instance.GetComponent<Renderer> ();

                            if (unityRenderer != null) 
                            {
                                unityRenderer.sharedMaterial.enableInstancing = true;

                                unityMaterials [unityPrefab.name] = unityRenderer.sharedMaterial;
                            }
                        }
                    }
                }
            }

            private Vector3 GetRandomScale ()
            {
                float scale = Random.Range (1.2f, 10f) * ScaleFactor;
                return new Vector3 (scale, scale, scale);
            }

            private Vector3 GetRandomPosition ()
            {
                return new Vector3 (
                    Random.Range (-10f, 10f),
                    Random.Range (-10f, 10f),
                    Random.Range (-10f, 10f));
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
                return true;
            }

            /// <summary>
            /// create menu item in the File menu
            /// </summary>
            [MenuItem (MenuItemName1, false)]
            public static void OnMenuItem1 ()
            {
                OnCreateScene ();
            }

            /// <summary>
            // Validate the menu item defined by the function above.
            /// </summary>
            [MenuItem (MenuItemName1, true)]
            public static bool OnValidateMenuItem1 ()
            {
                GameObject unityGo = Selection.activeObject as GameObject;

                return (unityGo != null) && IsModelPrefab (unityGo);
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
                        if (m_Binormals == null || m_Binormals.Length == 0) {
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
                /// TODO: Gets the triangle vertex indices
                /// </summary>
                /// <value>The normals.</value>
                public int [] Indices {
                    get {
                        return mesh.triangles;
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
            /// Return true if GameObject is a model prefab
            /// </summary>
            private static bool IsModelPrefab (GameObject unityObj)
            {
                return PrefabUtility.GetPrefabType (unityObj) == PrefabType.Prefab ||
                                    PrefabUtility.GetPrefabType (unityObj) == PrefabType.ModelPrefab;
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
            /// Get a mesh renderer's mesh.
            /// </summary>
            private MeshInfo GetMeshInfo (GameObject gameObject, bool requireRenderer = true)
            {
                if (requireRenderer) {
                    // Verify that we are rendering. Otherwise, don't export.
                    var renderer = gameObject.gameObject.GetComponent<MeshRenderer> ();
                    if (!renderer || !renderer.enabled) {
                        return new MeshInfo ();
                    }
                }

                var meshFilter = gameObject.GetComponent<MeshFilter> ();
                if (!meshFilter) {
                    return new MeshInfo ();
                }
                var mesh = meshFilter.sharedMesh;
                if (!mesh) {
                    return new MeshInfo ();
                }

                return new MeshInfo (gameObject, mesh);
            }

            private static string MakeObjectName (string name)
            {
                 return NamePrefix + name;
            }

            private static string MakeFileName(string basename = "test", string extension = "fbx")
            {
                return basename + "." + extension;
            }

            // create an example scene from the selected prefab
            private static void OnCreateScene()
            {
                using (var fbxExporter = Create ()) {
                    fbxExporter.CreateScene (Selection.objects);
                }
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
