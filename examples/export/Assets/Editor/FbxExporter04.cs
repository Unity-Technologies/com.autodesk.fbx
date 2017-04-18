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

        //
        // The exporter01 example illustrates how to:
        //
        //        1) create and initialize an exporter
        //        2) create a scene
        //        3) export a scene to a .FBX file (ASCII mode)
        //

        public class FbxExporter03 : System.IDisposable
        {
            public void Dispose () { }

            public int NumMeshes { private set ; get ; }
            public int NumTriangles { private set ; get ; }
            public int NumVertices { private set ; get ; }

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
            	sceneInfo.mTitle = " Example 03: exporting a static mesh";
            	sceneInfo.mSubject = "Example of a scene with a static mesh";
            	sceneInfo.mAuthor = "Unit Technologies";
            	sceneInfo.mRevision = "1.0";
            	sceneInfo.mKeywords = "example exporting mesh";
            	sceneInfo.mComment = "";

            	scene.SetSceneInfo (sceneInfo);

                // export set of object
                NumMeshes = 0;
                NumTriangles = 0;
                NumVertices = 0;

                bool requireRenderer = true;
                int n = 0;
                foreach (var obj in exportSet) {
                    ++n;
                        
                    if (obj is UnityEngine.Transform) {
                        var xform = obj as UnityEngine.Transform;
                        this.ExportMeshRenderer (xform.gameObject, requireRenderer);
                    } else if (obj is UnityEngine.GameObject) {
                        this.ExportMeshRenderer (obj as UnityEngine.GameObject, requireRenderer);
                    } else if (obj is MonoBehaviour) {
                        var mono = obj as MonoBehaviour;
                        this.ExportMeshRenderer (mono.gameObject, requireRenderer);
                    } else if (obj is UnityEngine.Mesh) {
                        this.ExportMesh (new MeshInfo (obj as UnityEngine.Mesh));
                    } else {
                        --n;
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

                return status==true ? n : 0; 
            }

            /// <summary>
            /// Unconditionally export this mesh object to the file.
            /// We have decided; this mesh is definitely getting exported.
            /// If the transform is null, we export in coordinates local to the mesh.
            /// </summary>
            public void ExportMesh (MeshInfo mesh)
            {
                NumMeshes++;
                NumTriangles += mesh.triangles.Length / 3;
                NumVertices += mesh.vertexCount;
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
                /// Gets the vertex count.
                /// </summary>
                /// <value>The vertex count.</value>
                public int vertexCount { get { return mesh.vertexCount; } }

                /// <summary>
                /// Gets the triangles. Each triangle is represented as 3 indices from the vertices array.
                /// Ex: if triangles = [3,4,2], then we have one triangle with vertices vertices[3], vertices[4], and vertices[2]
                /// </summary>
                /// <value>The triangles.</value>
                public int [] triangles { get { return mesh.triangles; } }

                /// <summary>
                /// Gets the vertices, represented in local coordinates.
                /// </summary>
                /// <value>The vertices.</value>
                public Vector3 [] vertices { get { return mesh.vertices; } }

                /// <summary>
                /// Gets the normals for the vertices.
                /// </summary>
                /// <value>The normals.</value>
                public Vector3 [] normals { get { return mesh.normals; } }

                /// <summary>
                /// Gets the uvs.
                /// </summary>
                /// <value>The uv.</value>
                public Vector2 [] uv { get { return mesh.uv; } }

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
            /// Export a mesh renderer's mesh.
            /// </summary>
            private void ExportMeshRenderer (GameObject gameObject, bool requireRenderer)
            {
                if (requireRenderer) {
                    // Verify that we are rendering. Otherwise, don't export.
                    var renderer = gameObject.gameObject.GetComponent<MeshRenderer> ();
                    if (!renderer || !renderer.enabled) {
                        return;
                    }
                }

                var meshFilter = gameObject.GetComponent<MeshFilter> ();
                if (!meshFilter) {
                    return;
                }
                var mesh = meshFilter.sharedMesh; 
                if (!mesh) {
                    return;
                }
                ExportMesh (new MeshInfo (gameObject, mesh));
            }

            //
            // manage the selection of a filename
            //
            static string m_LastFilePath = "";
            static string LastFilePath { get { return m_LastFilePath; } set { m_LastFilePath = value; } }
            static string Basename { get { return GetActiveSceneName (); } }
            static string Extension { get { return "fbx"; } }

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
                return "_fbxexporter03_" + name;
            }
        }
    }
}