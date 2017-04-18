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
        //
        // The FbxExporter04 example illustrates how to:
        //
        //        1) create and initialize an exporter
        //        2) create a scene
        //        3) create a node with transform data
        //        4) add static mesh data to a node
        //        5) export a scene to a .FBX file (ASCII mode)
        //

        public class FbxExporter04 : System.IDisposable
        {
            public void Dispose () { }

            public int NumMeshes { private set; get; }
            public int NumTriangles { private set; get; }
            public int NumVertices { private set; get; }

            /// <summary>
            /// Unconditionally export this mesh object to the file.
            /// We have decided; this mesh is definitely getting exported.
            /// If the transform is null, we export in coordinates local to the mesh.
            /// </summary>
            public void ExportMesh (FbxNode lNode, FbxScene scene, MeshInfo mesh)
            {
                if (!mesh.IsValid)
                    return;

                NumMeshes++;
                NumTriangles += mesh.Triangles.Length / 3;
                NumVertices += mesh.VertexCount;

                // create the mesh structure.
                FbxMesh lMesh = FbxMesh.Create (scene, MakeObjectName ("Scene"));

                // Create control points.
                lMesh.InitControlPoints (mesh.VertexCount);

                // NOTE: we expect this is a reference to the array held by the mesh.
                // This seems to be the only way to copy across vertex data
                FbxVector4 [] vertex = lMesh.GetControlPoints ();

                // copy control point data from Unity to FBX
                for (int v = 0; v < mesh.VertexCount; v++)
                {
                    vertex [v].Set(mesh.Vertices[v].x, mesh.Vertices[v].y, mesh.Vertices[v].z);
                }

#if UNI-12952_STRETCH_MATERIALS
                /* create the materials.
                 * Each polygon face will be assigned a unique material.
                 */
                FbxGeometryElementMaterial lMaterialElement = lMesh.CreateElementMaterial ();

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
                    lMesh.BeginPolygon ();
                    lMesh.AddPolygon (mesh.Triangles[vId++]);
                    lMesh.AddPolygon (mesh.Triangles[vId++]);
                    lMesh.AddPolygon (mesh.Triangles[vId++]);
                    lMesh.EndPolygon ();
                }

#if UNI-12952_STRETCH_NORMALS
                // specify normals per control point.
                FbxGeometryElementNormal[] lNormalElement = lMesh.CreateElementNormal ();
                lNormalElement.SetMappingMode (FbxGeometryElement.eByControlPoint);
                lNormalElement.SetReferenceMode (FbxGeometryElement.eDirect);
                for (int n = 0; n < lNumControlPoints; n++)
                    lNormalElement->GetDirectArray ().Add (FbxVector4 (lNormals [n] [0], lNormals [n] [1], lNormals [n] [2]));
#endif

#if UNI-12952_STRETCH_UVSET
                // create UVset
                FbxGeometryElementUV[] lUVElement1 = lMesh.CreateElementUV ("UVSet1");

                lUVElement1.SetMappingMode (FbxGeometryElement::eByPolygonVertex);
                lUVElement1.SetReferenceMode (FbxGeometryElement::eIndexToDirect);
                for (int i = 0; i<4; i++)
                    lUVElement1.GetDirectArray ().Add (FbxVector2(lUVs [i] [0], lUVs [i] [1]));
                for (int i = 0; i<24; i++)
                    lUVElement1.GetIndexArray ().Add (uvsId [i % 4]);
#endif

                // set the node containing the mesh
                lNode.SetNodeAttribute (lMesh);
                lNode.SetShadingMode (FbxNode.eWireFrame);

            }

            // get a node's global default position.
            protected void ExportTransform (FbxNode node, UnityEngine.Transform transform)
            {
            	// get local position of node (from Unity)
            	UnityEngine.Vector3 ulT = transform.localPosition;
            	UnityEngine.Vector3 ulR = transform.localRotation.eulerAngles;
            	UnityEngine.Vector3 ulS = transform.localScale;

            	// transfer transform data from Unity to Fbx
            	FbxVector4 lT = new FbxVector4 (ulT.x, ulT.y, ulT.z);
            	FbxVector4 lR = new FbxVector4 (ulR.x, ulR.y, ulR.z);
            	FbxVector4 lS = new FbxVector4 (ulS.x, ulS.y, ulS.z);

                #if UNI_15317_TO_IMPLEMENT
                // set the local position of node
                node.LclTranslation.Set(lT);
                node.LclRotation.Set(lR);
                node.LclScaling.Set(lS);
                #endif

            	return;
            }

            /// <summary>
            /// Unconditionally export components on this game object
            /// </summary>
            protected void ExportComponents (GameObject go, FbxScene scene, FbxNode parent)
            {
        		// create an FbxNode and add it as a child of parent
        		FbxNode node = FbxNode.Create (scene, go.name);

                ExportTransform (node, go.transform);
                ExportMesh (node, scene, GetMeshInfo(go));

        		if (Verbose)
        			Debug.Log (string.Format ("exporting {0}", node.GetName ()));

        		parent.AddChild (node);

        		List<GameObject> childSet = new List<GameObject> ();
        		// now go through our children and recurse
        		foreach (Transform childT in go.transform) {
                    ExportComponents (childT.gameObject, scene, node);
        		}

                return ;
            }

            /// <summary>
            /// Export all the objects in the set.
            /// Return the number of objects in the set that we exported.
            /// </summary>
            public int ExportAll (IEnumerable<UnityEngine.Object> exportSet)
            {
                // Create fbx manager
                var manager = FbxManager.Create ();

                // Configure fbx IO settings.
                manager.SetIOSettings (FbxIOSettings.Create (manager, Globals.IOSROOT));

                // Create the exporter 
                var exporter = FbxExporter.Create (manager, MakeObjectName ("Exporter"));

                // Initialize the exporter.
                bool status = exporter.Initialize (LastFilePath, -1, manager.GetIOSettings ());
                // Check that initialization of the exporter was successful
                if (!status)
                    return 0;
                
                // Create a scene
                var scene = FbxScene.Create (manager, MakeObjectName ("Scene"));

            	// create scene info
            	FbxDocumentInfo sceneInfo = FbxDocumentInfo.Create (manager, MakeObjectName ("SceneInfo"));

            	// set some scene info values
            	sceneInfo.mTitle = " Example 04: exporting a static mesh";
            	sceneInfo.mSubject = "Example of a scene with a static mesh";
            	sceneInfo.mAuthor = "Unit Technologies";
            	sceneInfo.mRevision = "1.0";
            	sceneInfo.mKeywords = "example exporting mesh";
            	sceneInfo.mComment = "";

            	scene.SetSceneInfo (sceneInfo);

                FbxNode root = scene.GetRootNode ();

                // export set of object
                foreach (var obj in exportSet) {
                    var go = GetGameObject (obj);

                    if (go) {
                        this.ExportComponents (go, scene, root);
                    }
                }

                sceneInfo.mComment = 
                    string.Format( "Mesh Count : {0}, Triangle Count: {1}, Vertex Count: {2} ",
                                   NumMeshes, NumTriangles, NumVertices);

                // Export the scene to the file.
                status = exporter.Export (scene);

                // cleanup
                scene.Destroy ();
                exporter.Destroy ();
                manager.Destroy ();

                return status==true ? NumMeshes : 0; 
            }

            // 
            // Create a simple user interface (menu items)
            //
            /// <summary>
            /// create menu item in the File menu
            /// </summary>
            [MenuItem ("File/Export/Export (Mesh only) to FBX", false)]
            public static void OnMenuItem ()
            {
                OnExport();
            }

            /// <summary>
            // Validate the menu item defined by the function above.
            /// </summary>
            [MenuItem ("File/Export/Export (Mesh only) to FBX", true)]
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

            //
            // manage the selection of a filename
            //
            static string m_LastFilePath = "";
            static string LastFilePath { get { return m_LastFilePath; } set { m_LastFilePath = value; } }
            static string Basename { get { return GetActiveSceneName (); } }
            static string Extension { get { return "fbx"; } }

            public bool Verbose { private set; get; }

            private static string GetActiveSceneName()
            {
                var scene = SceneManager.GetActiveScene();

                return string.IsNullOrEmpty(scene.name) ? "Untitled" : scene.name;    
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

                using (FbxExporter03 exporter = new FbxExporter03()) {
                    
    				// ensure output directory exists
    				EnsureDirectory (filePath);

                    if (exporter.ExportAll(Selection.objects) > 0)
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

             private static string MakeObjectName (string name)
            {
                return name;
            }
        }
    }
}