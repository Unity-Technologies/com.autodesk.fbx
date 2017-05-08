//#define UNI_15280
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
        public class FbxExporter07 : System.IDisposable
        {
            const string Title =
                "Example 07: exporting a skinned mesh with bones";

            const string Subject =
                @"Example FbxExporter07 illustrates how to:
                    1) create and initialize an exporter        
                    2) create a scene                           
                    3) create a skeleton
                    4) exported mesh
                    5) bind mesh to skeleton
                    6) create a bind pose
                    7) export the skinned mesh to a FBX file (ASCII mode)
                ";

            const string Keywords =
                "export skeleton mesh skin cluster pose";

            const string Comments =
                "";

            const string MenuItemName = "File/Export FBX/7. Skinned mesh with bones";

            const string FileBaseName = "example_skinned_mesh_with_bones";

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter07 Create () { return new FbxExporter07 (); }

            /// <summary>
            /// Export GameObject's as a skinned mesh with bones
            /// </summary>
            protected void ExportSkinnedMesh (Animator unityAnimator, FbxScene fbxScene, FbxNode fbxParentNode)
            {
                GameObject unityGo = unityAnimator.gameObject;

                SkinnedMeshRenderer unitySkin
                    = unityGo.GetComponentInChildren<SkinnedMeshRenderer> ();

                if (unitySkin == null) {
                    Debug.LogError ("could not find skinned mesh");
                    return;
                }

                var meshInfo = GetSkinnedMeshInfo (unityGo);

                if (meshInfo.renderer == null) {
                    Debug.LogError ("mesh has no renderer");
                    return;
                }

                // create an FbxNode and add it as a child of fbxParentNode
                FbxNode fbxNode = FbxNode.Create (fbxScene, unityAnimator.name);


                Dictionary<Transform, FbxNode> boneNodes
                    = new Dictionary<Transform, FbxNode> ();

                // export skeleton
                if (ExportSkeleton (meshInfo, fbxScene, fbxNode, ref boneNodes)) {
                    // export skin
                    FbxNode fbxMeshNode = ExportMesh (meshInfo, fbxScene, fbxNode);

                    FbxMesh fbxMesh = fbxMeshNode.GetMesh ();

                    if (fbxMesh == null) {
                        Debug.LogError ("Could not find mesh");
                        return;
                    }

                    // bind mesh to skeleton
                    ExportSkin (meshInfo, fbxScene, fbxMesh, fbxMeshNode, boneNodes);

                    // add bind pose
                    ExportBindPose (fbxNode, fbxMeshNode, fbxScene, boneNodes);

                    fbxParentNode.AddChild (fbxNode);
                    NumNodes++;

                    if (Verbose)
                        Debug.Log (string.Format ("exporting {0} {1}", "Skin", fbxNode.GetName ()));
                }
                else{
                    Debug.LogError("failed to export skeleton");
                }
            }

            /// <summary>
            /// Export bones of skinned mesh
            /// </summary>
            protected bool ExportSkeleton (MeshInfo meshInfo, FbxScene fbxScene, FbxNode fbxParentNode,
                                           ref Dictionary<Transform, FbxNode> boneNodes)
            {
                SkinnedMeshRenderer unitySkinnedMeshRenderer
                    = meshInfo.renderer as SkinnedMeshRenderer;

                if (unitySkinnedMeshRenderer.bones.Length <= 0) {
                    return false;
                }

                Dictionary<Transform, Matrix4x4> boneBindPose = new Dictionary<Transform, Matrix4x4>();

                for (int boneIndex = 0; boneIndex < unitySkinnedMeshRenderer.bones.Length; boneIndex++) {
                    Transform unityBoneTransform = unitySkinnedMeshRenderer.bones [boneIndex];

                    FbxNode fbxBoneNode = FbxNode.Create (fbxScene, unityBoneTransform.name);

                    // Create the node's attributes
                    FbxSkeleton fbxSkeleton = FbxSkeleton.Create (fbxScene, unityBoneTransform.name + "_Skel");

                    var fbxSkeletonType = FbxSkeleton.EType.eLimbNode;
                    if(unityBoneTransform == unityBoneTransform.root || fbxParentNode.GetName().Equals(unityBoneTransform.parent.name)){
                        fbxSkeletonType = FbxSkeleton.EType.eRoot;
                    }
                    fbxSkeleton.SetSkeletonType (fbxSkeletonType);
                    fbxSkeleton.Size.Set (1.0f);

                    // Set the node's attribute
                    fbxBoneNode.SetNodeAttribute (fbxSkeleton);

                    boneBindPose.Add (unityBoneTransform, meshInfo.BindPoses [boneIndex]);

                    // save relatation between unity transform and fbx bone node for skinning
                    boneNodes [unityBoneTransform] = fbxBoneNode;
                }

                // set the hierarchy for the FbxNodes
                foreach (KeyValuePair<Transform, FbxNode> t in boneNodes) {

                    Matrix4x4 pose;

                    // if this is a root node then don't need to do anything
                    if (t.Key == t.Key.root || !boneNodes.ContainsKey (t.Key.parent)) {
                        fbxParentNode.AddChild (t.Value);

                        pose = boneBindPose[t.Key].inverse; // assuming parent is identity matrix
                    } else {
                        boneNodes [t.Key.parent].AddChild (t.Value);

                        // inverse of my bind pose times parent bind pose
                        pose = boneBindPose[t.Key.parent] * boneBindPose[t.Key].inverse;
                    }

                    // use FbxMatrix to get translation and rotation relative to parent
                    FbxMatrix matrix = new FbxMatrix ();
                    matrix.SetColumn (0, new FbxVector4 (pose.GetRow (0).x, pose.GetRow (0).y, pose.GetRow (0).z, pose.GetRow (0).w));
                    matrix.SetColumn (1, new FbxVector4 (pose.GetRow (1).x, pose.GetRow (1).y, pose.GetRow (1).z, pose.GetRow (1).w));
                    matrix.SetColumn (2, new FbxVector4 (pose.GetRow (2).x, pose.GetRow (2).y, pose.GetRow (2).z, pose.GetRow (2).w));
                    matrix.SetColumn (3, new FbxVector4 (pose.GetRow (3).x, pose.GetRow (3).y, pose.GetRow (3).z, pose.GetRow (3).w));

                    FbxVector4 translation, rotation, shear, scale;
                    double sign;
                    matrix.GetElements (out translation, out rotation, out shear, out scale, out sign);

                    t.Value.LclTranslation.Set (new FbxDouble3(translation.X, translation.Y, translation.Z));
                    t.Value.LclRotation.Set (new FbxDouble3(0,0,0));
                    t.Value.LclScaling.Set (new FbxDouble3 (scale.X, scale.Y, scale.Z));

                    t.Value.SetRotationActive (true);
                    t.Value.SetPivotState (FbxNode.EPivotSet.eSourcePivot, FbxNode.EPivotState.ePivotReference);
                    t.Value.SetPreRotation (FbxNode.EPivotSet.eSourcePivot, new FbxVector4 (rotation.X, rotation.Y, rotation.Z));
                }

                return true;
            }

            /// <summary>
            /// Export binding of mesh to skeleton
            /// </summary>
            protected void ExportSkin (MeshInfo meshInfo, FbxScene fbxScene, FbxMesh fbxMesh,
                                       FbxNode fbxRootNode,
                                       Dictionary<Transform, FbxNode> boneNodes)
            {
                SkinnedMeshRenderer unitySkinnedMeshRenderer
                    = meshInfo.renderer as SkinnedMeshRenderer;

                FbxSkin fbxSkin = FbxSkin.Create (fbxScene, MakeObjectName (meshInfo.unityObject.name + "_Skin"));

                FbxAMatrix fbxMeshMatrix = fbxRootNode.EvaluateGlobalTransform ();

                // keep track of the bone index -> fbx cluster mapping, so that we can add the bone weights afterwards
                Dictionary<int, FbxCluster> boneCluster = new Dictionary<int, FbxCluster> ();

                for(int i = 0; i < unitySkinnedMeshRenderer.bones.Length; i++) {
                    FbxNode fbxBoneNode = boneNodes [unitySkinnedMeshRenderer.bones[i]];

                    // Create the deforming cluster
                    FbxCluster fbxCluster = FbxCluster.Create (fbxScene, MakeObjectName ("Cluster"));

                    fbxCluster.SetLink (fbxBoneNode);
                    fbxCluster.SetLinkMode (FbxCluster.ELinkMode.eTotalOne);

                    boneCluster.Add (i, fbxCluster);

                    // set the Transform and TransformLink matrix
                    fbxCluster.SetTransformMatrix (fbxMeshMatrix);

                    FbxAMatrix fbxLinkMatrix = fbxBoneNode.EvaluateGlobalTransform ();
                    fbxCluster.SetTransformLinkMatrix (fbxLinkMatrix);

                    // add the cluster to the skin
                    fbxSkin.AddCluster (fbxCluster);
                }

                // set the vertex weights for each bone
                SetVertexWeights(meshInfo, boneCluster);

                // Add the skin to the mesh after the clusters have been added
                fbxMesh.AddDeformer (fbxSkin);
            }

            /// <summary>
            /// set weight vertices to cluster
            /// </summary>
            protected void SetVertexWeights (MeshInfo meshInfo, Dictionary<int, FbxCluster> boneCluster)
            {
                // set the vertex weights for each bone
                for (int i = 0; i < meshInfo.BoneWeights.Length; i++) {
                    var boneWeights = meshInfo.BoneWeights;
                    int[] indices = {
                        boneWeights [i].boneIndex0,
                        boneWeights [i].boneIndex1,
                        boneWeights [i].boneIndex2,
                        boneWeights [i].boneIndex3
                    };
                    float[] weights = {
                        boneWeights [i].weight0,
                        boneWeights [i].weight1,
                        boneWeights [i].weight2,
                        boneWeights [i].weight3
                    };

                    for (int j = 0; j < indices.Length; j++) {
                        if (weights [j] <= 0) {
                            continue;
                        }
                        if (!boneCluster.ContainsKey (indices [j])) {
                            continue;
                        }
                        boneCluster [indices [j]].AddControlPointIndex (i, weights [j]);
                    }
                }
            }

            /// <summary>
            /// Export bind pose of mesh to skeleton
            /// </summary>
            protected void ExportBindPose (FbxNode fbxRootNode, FbxNode meshNode, FbxScene fbxScene, Dictionary<Transform, FbxNode> boneNodes)
            {
                FbxPose fbxPose = FbxPose.Create (fbxScene, MakeObjectName(fbxRootNode.GetName()));

                // set as bind pose
                fbxPose.SetIsBindPose (true);

                // assume each bone node has one weighted vertex cluster
                foreach (FbxNode fbxNode in boneNodes.Values)
                {
                    // EvaluateGlobalTransform returns an FbxAMatrix (affine matrix)
                    // which has to be converted to an FbxMatrix so that it can be passed to fbxPose.Add().
                    // The hierarchy for FbxMatrix and FbxAMatrix is as follows:
                    //
                    //      FbxDouble4x4
                    //      /           \
                    // FbxMatrix     FbxAMatrix
                    //
                    // Therefore we can't convert directly from FbxAMatrix to FbxMatrix,
                    // however FbxMatrix has a constructor that takes an FbxAMatrix.
                    FbxMatrix fbxBindMatrix = new FbxMatrix(fbxNode.EvaluateGlobalTransform ());

                    fbxPose.Add (fbxNode, fbxBindMatrix);
                }

                FbxMatrix bindMatrix = new FbxMatrix(meshNode.EvaluateGlobalTransform ());

                fbxPose.Add (meshNode, bindMatrix);

                // add the pose to the scene
                fbxScene.AddPose (fbxPose);
            }

            /// <summary>
            /// Unconditionally export this mesh object to the file.
            /// We have decided; this mesh is definitely getting exported.
            /// </summary>
            public FbxNode ExportMesh (MeshInfo meshInfo, FbxScene fbxScene, FbxNode fbxNode)
            {
                if (!meshInfo.IsValid) {
                    Debug.LogError ("Invalid mesh info");
                    return null;
                }

                // create a node for the mesh
                FbxNode meshNode = FbxNode.Create(fbxScene, "geo");

            	// create the mesh structure.
            	FbxMesh fbxMesh = FbxMesh.Create (fbxScene, MakeObjectName ("Scene"));

            	// Create control points.
            	int NumControlPoints = meshInfo.VertexCount;
            	fbxMesh.InitControlPoints (NumControlPoints);

            	// copy control point data from Unity to FBX
            	for (int v = 0; v < NumControlPoints; v++) {
            		fbxMesh.SetControlPointAt (new FbxVector4 (meshInfo.Vertices [v].x, meshInfo.Vertices [v].y, meshInfo.Vertices [v].z), v);
            	}

            	/* 
            	 * Create polygons
            	 */
            	int vId = 0;
            	for (int f = 0; f < meshInfo.Triangles.Length / 3; f++) {
            		fbxMesh.BeginPolygon ();
            		fbxMesh.AddPolygon (meshInfo.Triangles [vId++]);
            		fbxMesh.AddPolygon (meshInfo.Triangles [vId++]);
            		fbxMesh.AddPolygon (meshInfo.Triangles [vId++]);
            		fbxMesh.EndPolygon ();
            	}

            	// set the fbxNode containing the mesh
            	meshNode.SetNodeAttribute (fbxMesh);
            	meshNode.SetShadingMode (FbxNode.EShadingMode.eWireFrame);

                fbxNode.AddChild (meshNode);

                return meshNode;
            }

            protected void ExportComponents (GameObject  unityGo, FbxScene fbxScene, FbxNode fbxParentNode)
            {
                Animator unityAnimator = unityGo.GetComponent<Animator> ();

                if (unityAnimator == null)
                    return;

                ExportSkinnedMesh (unityAnimator, fbxScene, fbxParentNode);

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
                    // Configure IO settings.
                    fbxManager.SetIOSettings (FbxIOSettings.Create (fbxManager, Globals.IOSROOT));

                    // Create the exporter 
                    var fbxExporter = FbxExporter.Create (fbxManager, MakeObjectName ("fbxExporter"));

                    // Initialize the exporter.
                    int fileFormat = -1;
#if EXPORT_ASCII_FBX
                    fileFormat = fbxManager.GetIOPluginRegistry().FindWriterIDByDescription("FBX ascii (*.fbx)");
#endif
                    bool status = fbxExporter.Initialize (LastFilePath, fileFormat, fbxManager.GetIOSettings ());

                    // Check that initialization of the fbxExporter was successful
                    if (!status) 
                    {
                        Debug.LogError ("failed to initialize exporter");
                        return 0;
                    }

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

                    var fbxSettings = fbxScene.GetGlobalSettings();
                    fbxSettings.SetSystemUnit(FbxSystemUnit.m); // Unity unit is meters

                    // The Unity axis system has Y up, Z forward, X to the right:
                    //var axisSystem = new FbxAxisSystem(FbxAxisSystem.EUpVector.eYAxis, FbxAxisSystem.EFrontVector.eParityOdd, FbxAxisSystem.ECoordSystem.eLeftHanded);
                    //fbxSettings.SetAxisSystem(axisSystem);

                    FbxNode fbxRootNode = fbxScene.GetRootNode ();

                    // export set of objects
                    foreach (var obj in unityExportSet) 
                    {
                        var  unityGo  = GetGameObject (obj);

                        if ( unityGo ) 
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

            /// <summary>
            /// create menu item in the File menu
            /// </summary>
            [MenuItem (MenuItemName, false)]
            public static void OnMenuItem () 
            {
                OnExport();
            }

            /// <summary>
            /// Validate the menu item defined by the function above.
            /// Return false if no transform is selected.
            /// </summary>
            [MenuItem (MenuItemName, true)]
            public static bool OnValidateMenuItem ()
            {
                return Selection.activeTransform != null;
            }

            /// <summary>
            /// Number of nodes exported including siblings and decendents
            /// </summary>
            public int NumNodes { private set; get; }

            /// <summary>
            /// Clean up this class on garbage collection
            /// </summary>
            public void Dispose () { }

            static bool Verbose { get { return true; } }
            const string NamePrefix = "";

            /// <summary>
            /// manage the selection of a filename
            /// </summary>
            static string   LastFilePath { get; set; }
            const string    Extension = "fbx";

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
                public Renderer renderer;

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

                public BoneWeight[] BoneWeights { get { return mesh.boneWeights; } }

                public Matrix4x4[] BindPoses { get { return mesh.bindposes; } }

            	/// <summary>
            	/// Initializes a new instance of the <see cref="MeshInfo"/> struct.
            	/// </summary>
            	/// <param name="gameObject">The GameObject the mesh is attached to.</param>
            	/// <param name="mesh">A mesh we want to export</param>
            	public MeshInfo (GameObject gameObject, Mesh mesh, Renderer renderer)
            	{
                    this.renderer = renderer;
                    this.mesh = mesh;
            		this.xform = gameObject.transform.localToWorldMatrix;
            		this.unityObject = gameObject;
            	}
            }

            /// <summary>
            /// Get a mesh renderer's mesh.
            /// </summary>
            private MeshInfo GetSkinnedMeshInfo (GameObject gameObject)
            {
        		// Verify that we are rendering. Otherwise, don't export.
        		var renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer> ();
        		if (!renderer || !renderer.enabled) {
                    Debug.LogError ("could not find renderer");
        			return new MeshInfo ();
        		}

            	var mesh = renderer.sharedMesh;
            	if (!mesh) {
                    Debug.LogError ("Could not find mesh");
            		return new MeshInfo ();
            	}

            	return new MeshInfo (gameObject, mesh, renderer);
            }

            /// <summary>
            /// Get the GameObject
            /// </summary>
            private static GameObject GetGameObject (Object obj)
            {
                if (obj is UnityEngine.Transform) 
                {
                    var xform = obj as UnityEngine.Transform;
                    return xform.gameObject;
                } 
                else if (obj is UnityEngine.GameObject) 
                {
                    return obj as UnityEngine.GameObject;
                } 
                else if (obj is MonoBehaviour) 
                {
                    var mono = obj as MonoBehaviour;
                    return mono.gameObject;
                }

                return null;
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
