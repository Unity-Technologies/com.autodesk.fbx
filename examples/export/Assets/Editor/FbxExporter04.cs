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
        public class FbxExporter04 : System.IDisposable
        {
            const string Title =
                "Example 04: exporting a static mesh";

            const string Subject =
                @"Example FbxExporter04 illustrates how to:
                    1) create and initialize an exporter
                    2) create a scene
                    3) create a node with transform data
                    4) add static mesh data to a node
                    5) export a scene to a FBX file (ASCII mode)
                            ";

            const string Keywords =
                "export mesh node transform";

            const string Comments =
                @"We are exporting rotations using the Euler angles from Unity.";

            const string MenuItemName = "File/Export/Export (static meshes) to FBX";

            /// <summary>
            /// Number of nodes exported including siblings and decendents
            /// </summary>
            public int NumNodes { private set; get; }

            /// <summary>
            /// Number of meshes exported
            /// </summary>
            public int NumMeshes { private set; get; }

            /// <summary>
            /// Number of triangles exported
            /// </summary>
            public int NumTriangles { private set; get; }

            /// <summary>
            /// Number of vertices
            /// </summary>
            public int NumVertices { private set; get; }

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter04 Create ()
            {
                return new FbxExporter04 ();
            }

            /// <summary>
            /// Clean up this class on garbage collection
            /// </summary>
            public void Dispose () { }

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
#if BLAH
                fbxMesh.InitControlPoints (mesh.VertexCount);

                // NOTE: we expect this is a reference to the array held by the mesh.
                // This seems to be the only way to copy across vertex data
                FbxVector4 [] vertex = fbxMesh.GetControlPoints ();

                // copy control point data from Unity to FBX
                for (int v = 0; v < mesh.VertexCount; v++)
                {
                    vertex [v].Set(mesh.Vertices[v].x, mesh.Vertices[v].y, mesh.Vertices[v].z);
                }
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

                    fbxSceneInfo.mComment =
                        string.Format ("Mesh Count : {0}, Triangle Count: {1}, Vertex Count: {2} ",
                                       NumMeshes, NumTriangles, NumVertices);

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
