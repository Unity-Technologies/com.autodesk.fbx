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
        public class FbxExporter12 : System.IDisposable
        {
            const string Title =
                "Example 12: exporting a skinned mesh with bones, materials, UVs, normals etc";

            const string Subject =
                @"Example FbxExporter12 illustrates how to:
                    1) create and initialize an exporter        
                    2) create a scene                           
                    3) create a skeleton
                    4) exported mesh, normals etc, UVs, material
                    5) bind mesh to skeleton
                    6) create a bind pose
                    7) export the skinned mesh to a FBX file (FBX201400 compatible, ASCII)
                ";

            const string Keywords =
                "export skeleton mesh skin cluster pose";

            const string Comments =
                "";

            const string MenuItemName = "File/Export FBX/12. Skinned mesh with bones, materials, UVs, normals etc";

            const string FileBaseName = "example_skinned_mesh_etc";

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter12 Create () { return new FbxExporter12 (); }

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
                    ExportBindPose (fbxNode, fbxScene, boneNodes);

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
                    t.Value.LclRotation.Set (new FbxDouble3(rotation.X, rotation.Y, rotation.Z));
                    t.Value.LclScaling.Set (new FbxDouble3 (scale.X, scale.Y, scale.Z));
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
            protected void ExportBindPose (FbxNode fbxRootNode, FbxScene fbxScene, Dictionary<Transform, FbxNode> boneNodes)
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

                // add the pose to the scene
                fbxScene.AddPose (fbxPose);
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
            /// Export the mesh's normals, binormals and tangents using 
            /// layer 0.
            /// </summary>
            /// 
            public void ExportNormalsEtc (MeshInfo mesh, FbxMesh fbxMesh)
            {
            	/// Set the Normals on Layer 0.
            	FbxLayer fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
            	if (fbxLayer == null) {
            		fbxMesh.CreateLayer ();
            		fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
            	}

            	using (var fbxLayerElement = FbxLayerElementNormal.Create (fbxMesh, MakeObjectName ("Normals"))) {
            		fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);

            		// TODO: normals for each triangle vertex instead of averaged per control point
            		//fbxNormalLayer.SetMappingMode (FbxLayerElement.eByPolygonVertex);

            		fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);

            		// Add one normal per each vertex face index (3 per triangle)
            		FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

            		for (int n = 0; n < mesh.Normals.Length; n++) {
            			fbxElementArray.Add (new FbxVector4 (mesh.Normals [n] [0],
            												 mesh.Normals [n] [1],
            												 mesh.Normals [n] [2]));
            		}
            		fbxLayer.SetNormals (fbxLayerElement);
            	}

            	/// Set the binormals on Layer 0. 
            	using (var fbxLayerElement = FbxLayerElementBinormal.Create (fbxMesh, MakeObjectName ("Binormals"))) {
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
            		fbxLayer.SetBinormals (fbxLayerElement);
            	}

            	/// Set the tangents on Layer 0.
            	using (var fbxLayerElement = FbxLayerElementTangent.Create (fbxMesh, MakeObjectName ("Tangents"))) {
            		fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);

            		// TODO: normals for each triangle vertex instead of averaged per control point
            		//fbxBinormalLayer.SetMappingMode (FbxLayerElement.eByPolygonVertex);

            		fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);

            		// Add one normal per each vertex face index (3 per triangle)
            		FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

            		for (int n = 0; n < mesh.Normals.Length; n++) {
            			fbxElementArray.Add (new FbxVector4 (mesh.Tangents [n] [0],
            												 mesh.Tangents [n] [1],
            												 mesh.Tangents [n] [2]));
            		}
            		fbxLayer.SetTangents (fbxLayerElement);
            	}
            }
            /// <summary>
            /// Export the mesh's UVs using layer 0.
            /// </summary>
            public void ExportUVs (MeshInfo mesh, FbxMesh fbxMesh)
            {
            	// Set the normals on Layer 0.
            	FbxLayer fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
            	if (fbxLayer == null) {
            		fbxMesh.CreateLayer ();
            		fbxLayer = fbxMesh.GetLayer (0 /* default layer */);
            	}

            	using (var fbxLayerElement = FbxLayerElementUV.Create (fbxMesh, "UVSet")) {
            		fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByPolygonVertex);
            		fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eIndexToDirect);

            		// set texture coordinates per vertex
            		FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

            		for (int n = 0; n < mesh.UV.Length; n++) {
            			fbxElementArray.Add (new FbxVector2 (mesh.UV [n] [0],
            											  mesh.UV [n] [1]));
            		}

            		// For each face index, point to a texture uv
            		var unityTriangles = mesh.Triangles;
            		FbxLayerElementArray fbxIndexArray = fbxLayerElement.GetIndexArray ();
            		fbxIndexArray.SetCount (unityTriangles.Length);

            		for (int i = 0, n = unityTriangles.Length; i < n; ++i) {
            			fbxIndexArray.SetAt (i, unityTriangles [i]);
            		}
            		fbxLayer.SetUVs (fbxLayerElement, FbxLayerElement.EType.eTextureDiffuse);
            	}
            }

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

            	// get absolute filepath to texture
            	textureSourceFullPath = Path.GetFullPath (textureSourceFullPath);

            	if (Verbose)
            		Debug.Log (string.Format ("{1} setting texture path {0}", textureSourceFullPath, fbxPropName));

            	// Find the corresponding property on the fbx material.
            	var fbxMaterialProperty = fbxMaterial.FindProperty (fbxPropName);
            	if (fbxMaterialProperty == null || !fbxMaterialProperty.IsValid ()) { return; }

            	// Find or create an fbx texture and link it up to the fbx material.
            	if (!TextureMap.ContainsKey (textureSourceFullPath)) {
            		var fbxTexture = FbxFileTexture.Create (fbxMaterial, fbxPropName + "_Texture");
            		fbxTexture.SetFileName (textureSourceFullPath);
            		fbxTexture.SetTextureUse (FbxTexture.ETextureUse.eStandard);
                    //fbxTexture.SetMaterialUse(FbxFileTexture.EMaterialUse.eModelMaterial);
            		fbxTexture.SetMappingType (FbxTexture.EMappingType.eUV);
            		TextureMap.Add (textureSourceFullPath, fbxTexture);
            	}
                TextureMap[textureSourceFullPath].ConnectDstProperty(fbxMaterialProperty);
                //fbxMaterialProperty.ConnectSrcObject(TextureMap[textureSourceFullPath]);
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
                if (Verbose)
                    Debug.Log(string.Format ("exporting material {0}", unityMaterial.name));
                
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
            public FbxNode ExportMesh (MeshInfo meshInfo, FbxScene fbxScene, FbxNode fbxNode)
            {
                if (Verbose)
                    Debug.Log(string.Format("exporting mesh {0}",fbxNode.GetName()));
                
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

                ExportNormalsEtc (meshInfo, fbxMesh);
                ExportUVs (meshInfo, fbxMesh);

                var fbxMaterial = ExportMaterial (meshInfo.Material, fbxScene);
                meshNode.AddMaterial (fbxMaterial);

            	/* 
            	 * Create polygons
            	 */
                for (int f = 0; f<meshInfo.Triangles.Length / 3; f++)
                {
                    fbxMesh.BeginPolygon ();
                    fbxMesh.AddPolygon (meshInfo.Triangles [3 * f]);
                    fbxMesh.AddPolygon (meshInfo.Triangles [3 * f + 1]);
                    fbxMesh.AddPolygon (meshInfo.Triangles [3 * f + 2]);
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

                    // Export embedded textures
                    fbxManager.GetIOSettings ().SetBoolProp (Globals.EXP_FBX_EMBEDDED, true);

                    // Create the exporter 
                    var fbxExporter = FbxExporter.Create (fbxManager, MakeObjectName ("fbxExporter"));

                    // Initialize the exporter.
                    int fileFormat = fbxManager.GetIOPluginRegistry().FindWriterIDByDescription("FBX ascii (*.fbx)");

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

                    /// NOTE: The Unity axis system has Y up, Z forward (parity odd), X to the right (left handed)
                    /// The Maya axis system has Y up, Z forward, X to the left (right handed).
                    /// FbxAxisSystem.ConvertScene doesn't work so we cannot automatically convert from Left-Handed 
                    /// to Right-Handed. We export as a right handed axis system but will the 
                    /// models flipped about the -X axis.
                    var axisSystem = FbxAxisSystem.MayaYUp;

                    fbxSettings.SetAxisSystem(axisSystem);

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
                /// Gets the binormals for the vertices.
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
                /// Gets the tangents for the vertices.
                /// </summary>
                /// <value>The tangents.</value>
                public Vector4 [] Tangents { get { return mesh.tangents; } }

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
                        var renderer = unityObject.GetComponentInChildren<Renderer>();
                        if (!renderer) { Debug.LogError("no mesh renderer"); return null; }
                		// .material instantiates a new material, which is bad
                		// most of the time.
                		return renderer.sharedMaterial;
                	}
                }

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
                    this.m_Binormals = null;
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
