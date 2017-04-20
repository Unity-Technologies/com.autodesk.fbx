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

namespace FbxSdk.Examples
{
    namespace Editor
    {
        public class FbxExporter05 : System.IDisposable
        {
            const string Title =
                "Example 05: exporting a static mesh with normals, binormals and tangents";

            const string Subject =
                @"Example FbxExporter05 illustrates how to:
                    1) create and initialize an exporter
                    2) create a scene
                    3) create a node with transform data
                    4) add static mesh to a node
                    5) add normals, binormals and tangents to the static mesh 
                    6) export the static mesh to a FBX file (ASCII mode)
                            ";

            const string Keywords =
                "export mesh node transform";

            const string Comments =
                @"The example uses layers to add normals, binormals and tangents. Adding normals using the geometry element management convenience functions is not supported.";

            const string MenuItemName = "File/Export/Export (mesh with normals, binormals and tangents) to FBX";

            /// <summary>
            /// Number of nodes exported including siblings and decendents
            /// </summary>
            public int NumNodes { private set; get; }

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter05 Create () { return new FbxExporter05 (); }

            /// <summary>
            /// Clean up this class on garbage collection
            /// </summary>
            public void Dispose () { }

            /// <summary>
            /// Export the mesh's normals, binormals and tangents using 
            /// layer 0.
            /// </summary>
            /// 
            public void ExportNormalsEtc (MeshInfo mesh, FbxMesh fbxMesh)
            {
                /// Set the Normals on Layer 0.
                FbxLayer fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
                if (fbxLayer == null)
                {
                    fbxMesh.CreateLayer ();
                    fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
                }

                using (var fbxLayerElement = FbxLayerElementNormal::Create (fbxMesh, MakeObjectName ("Normals")))
                {
                    fbxLayerElement.SetMappingMode (FbxLayerElement.eByControlPoint);

                    // TODO: normals for each triangle vertex instead of averaged per control point
                    //fbxNormalLayer.SetMappingMode (FbxLayerElement.eByPolygonVertex);

                    fbxLayerElement.SetReferenceMode (FbxLayerElement.eDirect);

                    // Add one normal per each vertex face index (3 per triangle)
                    FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                    for (int n = 0; n < mesh.Normals.Length; n++) 
                    {
                        fbxElementArray.Add (new FbxVector4 (mesh.Normals [n] [0], 
                                                             mesh.Normals [n] [1], 
                                                             mesh.Normals [n] [2]));
                    }
                    fbxLayer.SetNormals (fbxLayerElement);
                }

                /// Set the binormals on Layer 0. 
                using (var fbxLayerElement = FbxLayerElementBinormal::Create (fbxMesh, MakeObjectName ("Binormals"))) 
                {
                    fbxLayerElement.SetMappingMode (FbxLayerElement.eByControlPoint);

                    // TODO: normals for each triangle vertex instead of averaged per control point
                    //fbxBinormalLayer.SetMappingMode (FbxLayerElement.eByPolygonVertex);

                    fbxLayerElement.SetReferenceMode (FbxLayerElement.eDirect);

                    // Add one normal per each vertex face index (3 per triangle)
                    FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                    for (int n = 0; n < mesh.Binormals.Length; n++) {
                        fbxElementArray.Add (new FbxVector4 (mesh.Binormals [n] [0], 
                                                             mesh.Binormals [n] [1], 
                                                             mesh.Binormals [n] [2]));
                    }
                    fbxLayer.SetNormals (fbxLayerElement);
                }

                /// Set the tangents on Layer 0.
                using (var fbxLayerElement = FbxLayerElementTangent::Create (fbxMesh, MakeObjectName ("Tangents"))) 
                {
                    fbxLayerElement.SetMappingMode (FbxLayerElement.eByControlPoint);

                    // TODO: normals for each triangle vertex instead of averaged per control point
                    //fbxBinormalLayer.SetMappingMode (FbxLayerElement.eByPolygonVertex);

                    fbxLayerElement.SetReferenceMode (FbxLayerElement.eDirect);

                    // Add one normal per each vertex face index (3 per triangle)
                    FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                    for (int n = 0; n<mesh.Normals.Length; n++) {
                        fbxElementArray.Add (new FbxVector4 (mesh.Tangents [n] [0],
                                                             mesh.Tangents [n] [1],
                                                             mesh.Tangents [n] [2]));
                    }
                    fbxLayer.SetNormals (fbxLayerElement);
                }

            }

            /// <summary>
            /// Export the mesh's vertex color using layer 0.
            /// </summary>
            /// 
            public void ExportVertexColors (MeshInfo mesh, FbxMesh fbxMesh)
            {
                // Set the normals on Layer 0.
                FbxLayer fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
                if (fbxLayer == null) 
                {
                    fbxMesh.CreateLayer ();
                    fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
                }

               using (var fbxLayerElement = FbxLayerElementVertexColor::Create (fbxMesh, MakeObjectName ("VertexColor"));
                {
                    fbxLayerElement.SetMappingMode (FbxLayerElement.eByControlPoint);

                    // TODO: normals for each triangle vertex instead of averaged per control point
                    //fbxNormalLayer.SetMappingMode (FbxLayerElement.eByPolygonVertex);

                    fbxLayerElement.SetReferenceMode (FbxLayerElement.eDirect);

                    // Add one normal per each vertex face index (3 per triangle)
                    FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                    for (int n = 0; n < mesh.VertexColors.Length; n++) 
                    {
                        fbxElementArray.Add (new FbxColor(mesh.VertexColor[n][0], 
                                                          mesh.VertexColor[n][1], 
                                                          mesh.VertexColor[n][2]));
                    }

                    fbxLayer.SetVertexColor(fbxLayerElement);
                }
            }

            /// <summary>
            /// Export the mesh's UVs using layer 0.
            /// </summary>
            /// 
            public void ExportUVs (MeshInfo mesh, FbxMesh fbxMesh)
            {
                // Set the normals on Layer 0.
                FbxLayer fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
                if (fbxLayer == null) 
                {
                    fbxMesh.CreateLayer ();
                    fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
                }

                using (var fbxLayerElement = FbxLayerElementVertexColor::Create (fbxMesh, MakeObjectName ("UVSet"))
                {
                    fbxLayerElement.SetMappingMode (FbxLayerElement.eByPolygonVertex);
                    fbxLayerElement.SetReferenceMode (FbxLayerElement.eIndexToDirect);

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

                    fbxLayer.SetUVs (fbxLayerElement, FbxLayerElement.eTextureDiffuse);
                }
            }

            /// <summary>
            /// Unconditionally export this mesh object to the file.
            /// We have decided; this mesh is definitely getting exported.
            /// </summary>
            public void ExportMesh (MeshInfo mesh, FbxNode fbxNode, FbxScene fbxScene)
            {
                if (!mesh.IsValid)
                    return;

                NumMeshes++;
                NumTriangles += mesh.Triangles.Length / 3;
                NumVertices += mesh.VertexCount;

                // create the mesh structure.
                FbxMesh fbxMesh = FbxMesh.Create (fbxScene, MakeObjectName ("Scene"));

                // Create control points.
                int NumControlPoints = mesh.VertexCount;

                fbxMesh.InitControlPoints (NumControlPoints);

                // NOTE: we expect this is a reference to the array held by the mesh.
                // This seems to be the only way to copy across vertex data
                FbxVector4 [] vertex = fbxMesh.GetControlPoints ();

                // copy control point data from Unity to FBX
                for (int v = 0; v < NumControlPoints; v++)
                {
                    vertex [v].Set(mesh.Vertices[v].x, mesh.Vertices[v].y, mesh.Vertices[v].z);
                }

#if UNI_12952_STRETCH_MATERIALS
                /* create the materials.
                 * Each polygon face will be assigned a unique material.
                 */
                FbxGeometryElementMaterial lMaterialElement = fbxMesh.CreateElementMaterial ();

                lMaterialElement.SetMappingMode (FbxGeometryElement.eAllSame);
                lMaterialElement.SetReferenceMode (FbxGeometryElement.eIndexToDirect);
                lMaterialElement.GetIndexArray ().Add (0);
#endif

                /* 
                 * Create polygons after FbxGeometryElementMaterial are created. 
                 * TODO: Assign material indices.
                 */
                int vId = 0;
                for (int f = 0; f < mesh.Triangles.Length / 3; f++) {
                    fbxMesh.BeginPolygon ();
                    fbxMesh.AddPolygon (mesh.Triangles[vId++]);
                    fbxMesh.AddPolygon (mesh.Triangles[vId++]);
                    fbxMesh.AddPolygon (mesh.Triangles[vId++]);
                    fbxMesh.EndPolygon ();
                }

                ExportNormalsEtc (mesh, fbxMesh);
                ExportVertexColors (mesh, fbxMesh);
                ExportUVs (mesh, fbxMesh);

                // set the fbxNode containing the mesh
                fbxNode.SetNodeAttribute (fbxMesh);
                fbxNode.SetShadingMode (FbxNode.EShadingMode.eWireFrame);
            }

            // get a fbxNode's global default position.
            protected void ExportTransform (UnityEngine.Transform transform, FbxNode fbxNode)
            {
                // get local position of fbxNode (from Unity)
                UnityEngine.Vector3 ulT = transform.localPosition;
                UnityEngine.Vector3 ulR = transform.localRotation.eulerAngles;
                UnityEngine.Vector3 ulS = transform.localScale;

#if UNI_15317_TO_IMPLEMENT
                // transfer transform data from Unity to Fbx
                FbxVector4 lT = new FbxVector4 (ulT.x, ulT.y, ulT.z);
                FbxVector4 lR = new FbxVector4 (ulR.x, ulR.y, ulR.z);
                FbxVector4 lS = new FbxVector4 (ulS.x, ulS.y, ulS.z);

                // set the local position of fbxNode
                fbxNode.LclTranslation.Set(lT);
                fbxNode.LclRotation.Set(lR);
                fbxNode.LclScaling.Set(lS);
#endif

                return;
            }

            /// <summary>
            /// Unconditionally export components on this game object
            /// </summary>
            protected void ExportComponents (GameObject uniGo, FbxScene fbxScene, FbxNode fbxNodeParent)
            {
                // create an FbxNode and add it as a child of parent
                FbxNode fbxNode = FbxNode.Create (fbxScene, uniGo.name);
                NumNodes++;

                ExportTransform (uniGo.transform, fbxNode);
                ExportMesh (GetMeshInfo(uniGo), fbxNode, fbxScene);

                if (Verbose)
                    Debug.Log (string.Format ("exporting {0}", fbxNode.GetName ()));

                fbxNodeParent.AddChild (fbxNode);

                // now uniGo through our children and recurse
                foreach (Transform childT in uniGo.transform) {
                    ExportComponents (childT.gameObject, fbxScene, fbxNode);
                }

                return ;
            }

            /// <summary>
            /// Export all the objects in the set.
            /// Return the number of objects in the set that we exported.
            /// </summary>
            public int ExportAll (IEnumerable<UnityEngine.Object> uniExportSet)
            {
                // Create fbx fbxManager
                using (var fbxManager = FbxManager.Create ()) {
                    // Configure fbx IO settings.
                    fbxManager.SetIOSettings (FbxIOSettings.Create (fbxManager, Globals.IOSROOT));

                    // Create the fbxExporter 
                    var fbxExporter = FbxExporter.Create (fbxManager, MakeObjectName ("fbxExporter"));

                    // Initialize the fbxExporter.
                    bool status = fbxExporter.Initialize (LastFilePath, -1, fbxManager.GetIOSettings ());
                    // Check that initialization of the fbxExporter was successful
                    if (!status)
                        return 0;

                    // Create a fbxScene
                    var fbxScene = FbxScene.Create (fbxManager, MakeObjectName ("Scene"));

                    // create fbxScene info
                    FbxDocumentInfo fbxSceneInfo = FbxDocumentInfo.Create (fbxManager, MakeObjectName ("SceneInfo"));

                    // set some fbxScene info values
                    fbxSceneInfo.mTitle     = Title;
                    fbxSceneInfo.mSubject   = Subject;
                    fbxSceneInfo.mAuthor    = "Unit Technologies";
                    fbxSceneInfo.mRevision  = "1.0";
                    fbxSceneInfo.mKeywords  = Keywords;
                    fbxSceneInfo.mComment   = Comments;

                    fbxScene.SetSceneInfo (fbxSceneInfo);

                    FbxNode fbxRootNode = fbxScene.GetRootNode ();

                    // export set of object
                    foreach (var obj in uniExportSet) {
                        var uniGo = GetGameObject (obj);

                        if (uniGo) {
                            this.ExportComponents (uniGo, fbxScene, fbxRootNode);
                        }
                    }

                    // Export the fbxScene to the file.
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
                // Return false if no transform is selected.
                return Selection.activeTransform != null;
            }

            //
            // export mesh info from Unity
            //
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
                /// The gameobject in the fbxScene to which this mesh is attached.
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
                public Vector3 [] Binormals { get {
                    if (m_Binormals.Length == 0) {
                        m_Binormals = new Vector3 [mesh.normals.Length];

                        for (int i = 0; i < mesh.normals.Length; i++)
                            m_Binormals [i] = Vector3.Cross (mesh.normals [i], 
                                                             mesh.tangents [i]) 
                                                     * mesh.tangents [i].w;

                    }
                    return m_Binormals;

                    /// NOTE: LINQ
                    ///    return mesh.normals.Zip (mesh.tangents, (first, second)
                    ///    => Math.cross (normal, tangent.xyz) * tangent.w
                }

                /// <summary>
                /// TODO: Gets the triangle vertex indices
                /// </summary>
                /// <value>The normals.</value>
                int [] m_Indices;

                public int [] Indices {
                    get {
                        if (m_Indices.Length == 0) {
                            m_Indices = new int [mesh.triangles.Length * 3];
                            int i = 0;
                            for (int triIndex = 0; triIndex < mesh.triangles.Length; triIndex++)
                            {
                                for (int vtxIndex = 0; vtxIndex < 3; vtxIndex++)
                                {
                                    m_Indices[i++] = (triIndex * 3) + vtxIndex;
                                }
                           }
                        }
                        return m_Indices;
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
                /// Initializes a new instance of the <see cref="MeshInfo"/> struct.
                /// </summary>
                /// <param name="mesh">A mesh we want to export</param>
                public MeshInfo(Mesh mesh) {
                    this.mesh = mesh;
                    this.xform = Matrix4x4.identity;
                    this.unityObject = null;
                }

                /// <summary>
                /// Initializes a new instance of the <see cref="MeshInfo"/> struct.
                /// </summary>
                /// <param name="gameObject">The GameObject the mesh is attached to.</param>
                /// <param name="mesh">A mesh we want to export</param>
                public MeshInfo(GameObject gameObject, Mesh mesh) {
                    this.mesh = mesh;
                    this.xform = gameObject.transform.localToWorldMatrix;
                    this.unityObject = gameObject;
                }
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
                        return new MeshInfo();
                    }
                }

                var meshFilter = gameObject.GetComponent<MeshFilter> ();
                if (!meshFilter) {
                    return new MeshInfo();
                }
                var mesh = meshFilter.sharedMesh; 
                if (!mesh) {
                    return new MeshInfo();
                }

                return new MeshInfo (gameObject, mesh);
            }

            /// <summary>
            /// manage the selection of a filename
            /// </summary>
            static string LastFilePath { get; set; }
            static string Basename { get { return GetActiveSceneName (); } }
            const string Extension = "fbx";

            const string NamePrefix = "";
            public bool Verbose { private set; get; }

            private static string GetActiveSceneName()
            {
                var uniScene = SceneManager.GetActiveScene();

                return string.IsNullOrEmpty(uniScene.name) ? "Untitled" : uniScene.name;    
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
                                     ? MakeFileName(basename: Basename, extension: Extension) 
                                     : System.IO.Path.GetFileName (LastFilePath);
                
                var title = string.Format ("Export FBX ({0})", Basename);

                var filePath = EditorUtility.SaveFilePanel (title, directory, filename, "");

                if (string.IsNullOrEmpty (filePath)) {
                    return;
                }

                LastFilePath = filePath;

                using (FbxExporter04 fbxExporter = new FbxExporter04()) {
                    
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

                if (!fileInfo.Exists) {
                    Directory.CreateDirectory (fileInfo.Directory.FullName);
                }
            }

        }
    }
}