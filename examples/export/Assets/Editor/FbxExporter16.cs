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
using Autodesk.Fbx;

namespace Autodesk.Fbx.Examples
{
    namespace Editor
    {
        public class FbxExporter16 : System.IDisposable
        {
            const string Title =
                "Example 16: exporting blend shapes";

            const string Subject =
                @"Example FbxExporter16 illustrates how to:
                    1) create and initialize an exporter
                    2) create a scene
                    3) export blend shapes to an FBX file (FBX201400 compatible)
                ";

            const string Keywords =
                "export blend shapes";

            const string Comments = "";

            const string MenuItemName = "File/Export FBX/16. Blend shapes";

            const string FileBaseName = "example_blend_shapes";

            /// <summary>
            /// Create instance of example
            /// </summary>
            public static FbxExporter16 Create () { return new FbxExporter16(); }

            /// <summary>
            /// keep a map between GameObject and FbxNode for quick lookup when we export
            /// animation.
            /// </summary>
            Dictionary<GameObject, FbxNode> MapUnityObjectToFbxNode = new Dictionary<GameObject, FbxNode> ();

            /// <summary>
            /// Exports the blend shapes.
            /// </summary>
            protected void ExportBlendShapes (SkinnedMeshRenderer unitySkin, FbxMesh fbxMesh, FbxScene fbxScene)
            {
                Mesh unityMesh = unitySkin.sharedMesh;
                for (int i = 0; i < unityMesh.blendShapeCount; i++) {
                    if(Verbose)
                        Debug.Log ("Adding blend shape: " + unityMesh.GetBlendShapeName (i));

                    FbxBlendShape fbxBlendShape = FbxBlendShape.Create(fbxScene, unityMesh.GetBlendShapeName(i));
                    FbxBlendShapeChannel fbxBlendShapeChannel = FbxBlendShapeChannel.Create(fbxScene, unityMesh.GetBlendShapeName(i));

                    for (int j = 0; j < unityMesh.GetBlendShapeFrameCount (i); j++) {
                        Vector3[] deltaVertices = new Vector3[unityMesh.vertexCount];
                        Vector3[] deltaNormals = new Vector3[unityMesh.vertexCount];
                        Vector3[] deltaTangents = new Vector3[unityMesh.vertexCount];

                        unityMesh.GetBlendShapeFrameVertices (i, j, deltaVertices, deltaNormals, deltaTangents);

                        FbxShape fbxShape = FbxShape.Create(fbxScene, unityMesh.GetBlendShapeName(i) + "_" + j);
                        fbxShape.InitControlPoints(deltaVertices.Length);
                        for (int v = 0; v < deltaVertices.Length; v++)
                        {
                            fbxShape.SetControlPointAt(new FbxVector4 (
                                unityMesh.vertices[v].x + deltaVertices [v].x,
                                unityMesh.vertices[v].y + deltaVertices [v].y,
                                unityMesh.vertices[v].z + deltaVertices [v].z
                            ), v);
                        }
                        
                        FbxLayerElementNormal fbxElementNormal = fbxShape.CreateElementNormal();
                        fbxElementNormal.SetMappingMode(FbxLayerElement.EMappingMode.eByControlPoint);
                        fbxElementNormal.SetReferenceMode(FbxLayerElement.EReferenceMode.eDirect);

                        var fbxElementArray = fbxElementNormal.GetDirectArray();
                        for(int n = 0; n < deltaNormals.Length; n++){
                            fbxElementArray.Add(new FbxVector4(
                                unityMesh.normals[n].x + deltaNormals[n].x,
                                unityMesh.normals[n].y + deltaNormals[n].y,
                                unityMesh.normals[n].z + deltaNormals[n].z
                            ));
                        }

                        FbxLayerElementTangent fbxElementTangent = fbxShape.CreateElementTangent();
                        fbxElementTangent.SetMappingMode(FbxLayerElement.EMappingMode.eByControlPoint);
                        fbxElementTangent.SetReferenceMode(FbxLayerElement.EReferenceMode.eDirect);

                        fbxElementArray = fbxElementTangent.GetDirectArray();
                        for(int t = 0; t < deltaTangents.Length; t++){
                            fbxElementArray.Add(new FbxVector4(
                                unityMesh.tangents[t].x + deltaTangents[t].x,
                                unityMesh.tangents[t].y + deltaTangents[t].y,
                                unityMesh.tangents[t].z + deltaTangents[t].z
                            ));
                        }
                        fbxBlendShapeChannel.AddTargetShape(fbxShape, unityMesh.GetBlendShapeFrameWeight(i,j));
                    }

                    fbxBlendShape.AddBlendShapeChannel(fbxBlendShapeChannel);
                    fbxMesh.AddDeformer(fbxBlendShape);
                }
            }

            /// <summary>
            /// Export GameObject as a skinned mesh with material, bones, a skin and, a bind pose.
            /// </summary>
            protected void ExportSkinnedMesh (GameObject unityGo, FbxScene fbxScene, FbxNode fbxParentNode)
            {
                Animator unityAnimator = unityGo.GetComponent<Animator> ();
                if (!unityAnimator)
                    return;

                SkinnedMeshRenderer unitySkin
                = unityGo.GetComponentInChildren<SkinnedMeshRenderer> ();

                if (!unitySkin) {
                    Debug.LogError ("could not find skinned mesh");
                    return;
                }

                var meshInfo = GetSkinnedMeshInfo (unityGo);

                if (!meshInfo.renderer) {
                    Debug.LogError ("mesh has no renderer");
                    return;
                }

                // create node hierarchy
                ExportNodeHierarchy (unityGo, fbxParentNode);

                bool result = ExportSkeleton (unitySkin.gameObject, fbxScene);

                // export skeleton
                if (!result) {
                    Debug.LogWarning ("Could not export skeleton");
                }

                // export skin mesh
                FbxMesh fbxMesh = ExportMesh (meshInfo, fbxScene);

                if (fbxMesh == null) {
                    Debug.LogError ("Could not find mesh");
                    return;
                }

                // lookup node w skin renderer
                FbxNode fbxMeshNode = MapUnityObjectToFbxNode[unitySkin.gameObject];

                // export material for mesh
                var fbxMaterial = ExportMaterial (meshInfo.Material, fbxScene);
                fbxMeshNode.AddMaterial (fbxMaterial);

                // set the fbxNode containing the mesh
                fbxMeshNode.SetNodeAttribute (fbxMesh);
                fbxMeshNode.SetShadingMode (FbxNode.EShadingMode.eWireFrame);

                // bind mesh to skeleton
                ExportSkin (meshInfo, fbxScene, fbxMesh, fbxMeshNode);

                // add bind pose
                ExportBindPose (fbxParentNode, fbxMeshNode, fbxScene);

                ExportBlendShapes (unitySkin, fbxMesh, fbxScene);

                NumNodes++;

                if (Verbose)
                    Debug.Log (string.Format ("exporting {0} {1}", "Skin", fbxMeshNode.GetName ()));
            }

            /// <summary>
            /// Export each animation clip found in the Animator component on this game object
            /// </summary>
            protected void ExportAnimationClips (GameObject unityRoot, FbxScene fbxScene)
            {
                var animator = unityRoot.GetComponent<Animator> ();
                if (!animator) { return; }

                // Get the controller.
                var controller = animator.runtimeAnimatorController;
                if (!controller) { return; }

                // Only export each clip once per game object.
                var exported = new HashSet<AnimationClip> ();
                foreach (var clip in controller.animationClips) {
                    if (exported.Add (clip)) {
                        ExportAnimationClip (clip, unityRoot, fbxScene);
                    }
                }
            }

            /// <summary>
            /// Create the node hierarchy and store the mapping from Unity
            /// object to FBX node.
            /// </summary>
            protected void ExportNodeHierarchy (GameObject unityGo, FbxNode fbxParentNode)
            {
                FbxNode fbxNode = FbxNode.Create (fbxParentNode, unityGo.name);

                MapUnityObjectToFbxNode [unityGo] = fbxNode;

                foreach (Transform unityChild in unityGo.transform)
                {
                    ExportNodeHierarchy (unityChild.gameObject, fbxNode);
                }
            }

            /// <summary>
            /// Export bones of skinned mesh, if this is a skinned mesh with
            /// bones and bind poses.
            /// </summary>
            private bool ExportSkeleton (GameObject unityGo, FbxScene fbxScene)
            {
                var unitySkinnedMeshRenderer = unityGo.GetComponentInChildren<SkinnedMeshRenderer>();
                if (!unitySkinnedMeshRenderer) { return false; }
                var bones = unitySkinnedMeshRenderer.bones;
                if (bones == null || bones.Length == 0) { return false; }
                var mesh = unitySkinnedMeshRenderer.sharedMesh;
                if (!mesh) { return false; }

                var bindPoses = mesh.bindposes;
                if (bindPoses == null || bindPoses.Length != bones.Length) { return false; }

                // Three steps:
                // 0. Set up the map from bone to index.
                // 1. Create the bones, in arbitrary order.
                // 2. Connect up the hierarchy.
                // 3. Set the transforms.
                // Step 0 supports step 1 (finding which is the root bone) and step 3
                // (setting up transforms; the complication is the use of pivots).

                // Step 0: map transform to index so we can look up index by bone.
                Dictionary<Transform, int> index = new Dictionary<Transform, int>();
                for (int boneIndex = 0, n = bones.Length; boneIndex < n; boneIndex++) {
                    Transform unityBoneTransform = bones [boneIndex];
                    index[unityBoneTransform] = boneIndex;
                }

                // Step 1: create the bones.
                for (int boneIndex = 0, n = bones.Length; boneIndex < n; boneIndex++) {
                    Transform unityBoneTransform = bones [boneIndex];

                    // Create the bone node if we haven't already. Parent it to
                    // its corresponding parent, or to the scene if there is none.
                    FbxNode fbxBoneNode;
                    if (!MapUnityObjectToFbxNode.TryGetValue(unityBoneTransform.gameObject, out fbxBoneNode)) {
                        var unityParent = unityBoneTransform.parent;
                        FbxNode fbxParent;
                        if (MapUnityObjectToFbxNode.TryGetValue(unityParent.gameObject, out fbxParent)) {
                            fbxBoneNode = FbxNode.Create (fbxParent, unityBoneTransform.name);
                        } else {
                            fbxBoneNode = FbxNode.Create (fbxScene, unityBoneTransform.name);
                        }
                        MapUnityObjectToFbxNode.Add(unityBoneTransform.gameObject, fbxBoneNode);
                    }

                    // Set it up as a skeleton node if we haven't already.
                    if (fbxBoneNode.GetSkeleton() == null) {
                        FbxSkeleton fbxSkeleton = FbxSkeleton.Create (fbxScene, unityBoneTransform.name + "_Skel");
                        var fbxSkeletonType = index.ContainsKey(unityBoneTransform.parent)
                            ? FbxSkeleton.EType.eLimbNode : FbxSkeleton.EType.eRoot;
                        fbxSkeleton.SetSkeletonType (fbxSkeletonType);
                        fbxSkeleton.Size.Set (1.0f);
                        fbxBoneNode.SetNodeAttribute (fbxSkeleton);
                        if (Verbose) { Debug.Log("Converted " + unityBoneTransform.name + " to a " + fbxSkeletonType + " bone"); }
                    }
                }

                // Step 2: connect up the hierarchy.
                foreach (var unityBone in bones) {
                    var fbxBone = MapUnityObjectToFbxNode[unityBone.gameObject];
                    var fbxParent = MapUnityObjectToFbxNode[unityBone.parent.gameObject];
                    fbxParent.AddChild(fbxBone);
                }

                // Step 3: set up the transforms.
                for (int boneIndex = 0, n = bones.Length; boneIndex < n; boneIndex++) {
                    var unityBone = bones[boneIndex];
                    var fbxBone = MapUnityObjectToFbxNode[unityBone.gameObject];

                    Matrix4x4 pose;
                    if (fbxBone.GetSkeleton().GetSkeletonType() == FbxSkeleton.EType.eRoot) {
                        // bind pose is local -> root. We want root -> local, so invert.
                        pose = bindPoses[boneIndex].inverse; // assuming parent is identity matrix
                    } else {
                        // Bind pose is local -> parent -> ... -> root.
                        // We want parent -> local.
                        // Invert our bind pose to get root -> local.
                        // The apply parent -> root to leave just parent -> local.
                        pose = bindPoses[index[unityBone.parent]] * bindPoses[boneIndex].inverse;
                    }

                    // FBX is transposed relative to Unity: transpose as we convert.
                    FbxMatrix matrix = new FbxMatrix ();
                    matrix.SetColumn (0, new FbxVector4 (pose.GetRow (0).x, pose.GetRow (0).y, pose.GetRow (0).z, pose.GetRow (0).w));
                    matrix.SetColumn (1, new FbxVector4 (pose.GetRow (1).x, pose.GetRow (1).y, pose.GetRow (1).z, pose.GetRow (1).w));
                    matrix.SetColumn (2, new FbxVector4 (pose.GetRow (2).x, pose.GetRow (2).y, pose.GetRow (2).z, pose.GetRow (2).w));
                    matrix.SetColumn (3, new FbxVector4 (pose.GetRow (3).x, pose.GetRow (3).y, pose.GetRow (3).z, pose.GetRow (3).w));

                    // FBX wants translation, rotation (in euler angles) and scale.
                    // We assume there's no real shear, just rounding error.
                    FbxVector4 translation, rotation, shear, scale;
                    double sign;
                    matrix.GetElements (out translation, out rotation, out shear, out scale, out sign);

                    // Bones should have zero rotation, and use a pivot instead.
                    fbxBone.LclTranslation.Set (new FbxDouble3(translation.X, translation.Y, translation.Z));
                    fbxBone.LclRotation.Set (new FbxDouble3(0,0,0));
                    fbxBone.LclScaling.Set (new FbxDouble3 (scale.X, scale.Y, scale.Z));

                    fbxBone.SetRotationActive (true);
                    fbxBone.SetPivotState (FbxNode.EPivotSet.eSourcePivot, FbxNode.EPivotState.ePivotReference);
                    fbxBone.SetPreRotation (FbxNode.EPivotSet.eSourcePivot, new FbxVector4 (rotation.X, rotation.Y, rotation.Z));
                }

                return true;
            }

            /// <summary>
            /// Export binding of mesh to skeleton
            /// </summary>
            protected void ExportSkin (MeshInfo meshInfo, FbxScene fbxScene, FbxMesh fbxMesh,
                FbxNode fbxRootNode)
            {
                SkinnedMeshRenderer unitySkinnedMeshRenderer
                = meshInfo.renderer as SkinnedMeshRenderer;

                FbxSkin fbxSkin = FbxSkin.Create (fbxScene, (meshInfo.unityObject.name + "_Skin"));

                FbxAMatrix fbxMeshMatrix = fbxRootNode.EvaluateGlobalTransform ();

                // keep track of the bone index -> fbx cluster mapping, so that we can add the bone weights afterwards
                Dictionary<int, FbxCluster> boneCluster = new Dictionary<int, FbxCluster> ();

                for(int i = 0; i < unitySkinnedMeshRenderer.bones.Length; i++) {
                    FbxNode fbxBoneNode = MapUnityObjectToFbxNode [unitySkinnedMeshRenderer.bones[i].gameObject];

                    // Create the deforming cluster
                    FbxCluster fbxCluster = FbxCluster.Create (fbxScene, "BoneWeightCluster");

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
            protected void ExportBindPose (FbxNode fbxRootNode, FbxNode fbxMeshNode, FbxScene fbxScene)
            {
                FbxPose fbxPose = FbxPose.Create (fbxScene, fbxRootNode.GetName());

                // set as bind pose
                fbxPose.SetIsBindPose (true);

                // assume each bone node has one weighted vertex cluster
                foreach (FbxNode fbxNode in MapUnityObjectToFbxNode.Values)
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

                fbxPose.Add (fbxMeshNode, new FbxMatrix (fbxMeshNode.EvaluateGlobalTransform ()));

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

                using (var fbxLayerElement = FbxLayerElementNormal.Create (fbxMesh, "Normals")) {
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
                using (var fbxLayerElement = FbxLayerElementBinormal.Create (fbxMesh, "Binormals")) {
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
                using (var fbxLayerElement = FbxLayerElementTangent.Create (fbxMesh, "Tangents")) {
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
            public FbxMesh ExportMesh (MeshInfo meshInfo, FbxScene fbxScene)
            {
                if (!meshInfo.IsValid) {
                    Debug.LogError ("Invalid mesh info");
                    return null;
                }

                // create the mesh structure.
                FbxMesh fbxMesh = FbxMesh.Create (fbxScene, "Mesh");

                // Create control points.
                int NumControlPoints = meshInfo.VertexCount;
                fbxMesh.InitControlPoints (NumControlPoints);

                // copy control point data from Unity to FBX
                for (int v = 0; v < NumControlPoints; v++) {
                    fbxMesh.SetControlPointAt (new FbxVector4 (meshInfo.Vertices [v].x, meshInfo.Vertices [v].y, meshInfo.Vertices [v].z), v);
                }

                ExportNormalsEtc (meshInfo, fbxMesh);
                ExportUVs (meshInfo, fbxMesh);

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

                return fbxMesh;
            }

            struct PropertyChannel {
                public string Property { get ; private set; }
                public string Channel { get ; private set; }
                public PropertyChannel(string p, string c) {
                    Property = p;
                    Channel = c;
                }

                /// <summary>
                /// Map a Unity property name to the corresponding FBX property and
                /// channel names.
                /// </summary>
                public static bool TryGetValue(string unityName, out PropertyChannel prop)
                {
                    System.StringComparison ct = System.StringComparison.CurrentCulture;

                    if (unityName.StartsWith ("m_LocalPosition.x", ct) || unityName.EndsWith ("T.x", ct)) {
                        prop = new PropertyChannel ("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_X);
                        return true;
                    }
                    if (unityName.StartsWith ("m_LocalPosition.y", ct) || unityName.EndsWith ("T.y", ct)) {
                        prop = new PropertyChannel ("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_Y);
                        return true;
                    }

                    if (unityName.StartsWith ("m_LocalPosition.z", ct) || unityName.EndsWith ("T.z", ct)) {
                        prop = new PropertyChannel ("Lcl Translation", Globals.FBXSDK_CURVENODE_COMPONENT_Z);
                        return true;
                    }
                    if (unityName.StartsWith ("m_LocalScale.x", ct) || unityName.EndsWith ("S.x", ct)) {
                        prop = new PropertyChannel ("Lcl Scaling", Globals.FBXSDK_CURVENODE_COMPONENT_X);
                        return true;
                    }
                    if (unityName.StartsWith ("m_LocalScale.y", ct) || unityName.EndsWith ("S.y", ct)) {
                        prop = new PropertyChannel ("Lcl Scaling", Globals.FBXSDK_CURVENODE_COMPONENT_Y);
                        return true;
                    }
                    if (unityName.StartsWith ("m_LocalScale.z", ct) || unityName.EndsWith ("S.z", ct)) {
                        prop = new PropertyChannel ("Lcl Scaling", Globals.FBXSDK_CURVENODE_COMPONENT_Z);
                        return true;
                    }

                    prop = new PropertyChannel ();
                    return false;
                }
            }

            /// <summary>
            /// Exporting rotations is more complicated. We need to convert
            /// from quaternion to euler. We use this class to help.
            /// </summary>
            class QuaternionCurve {
                public AnimationCurve x;
                public AnimationCurve y;
                public AnimationCurve z;
                public AnimationCurve w;

                public struct Key {
                    public FbxTime time;
                    public FbxVector4 euler;
                }

                public QuaternionCurve() { }

                public static int GetQuaternionIndex(string unityPropertyName) {
                    System.StringComparison ct = System.StringComparison.CurrentCulture;
                    bool isQuaternionComponent = false;

                    isQuaternionComponent |= unityPropertyName.StartsWith ("m_LocalRotation.", ct);
                    isQuaternionComponent |= unityPropertyName.EndsWith ("Q.x", ct);
                    isQuaternionComponent |= unityPropertyName.EndsWith ("Q.y", ct);
                    isQuaternionComponent |= unityPropertyName.EndsWith ("Q.z", ct);
                    isQuaternionComponent |= unityPropertyName.EndsWith ("Q.w", ct);

                    if (!isQuaternionComponent) { return -1; }

                    switch(unityPropertyName[unityPropertyName.Length - 1]) {
                    case 'x': return 0;
                    case 'y': return 1;
                    case 'z': return 2;
                    case 'w': return 3;
                    default: return -1;
                    }
                }

                public void SetCurve(int i, AnimationCurve curve) {
                    switch(i) {
                    case 0: x = curve; break;
                    case 1: y = curve; break;
                    case 2: z = curve; break;
                    case 3: w = curve; break;
                    default: throw new System.IndexOutOfRangeException();
                    }
                }

                Key [] ComputeKeys(UnityEngine.Quaternion restRotation, FbxNode node) {
                    // Get the source pivot pre-rotation if any, so we can
                    // remove it from the animation we get from Unity.
                    var fbxPreRotationEuler = node.GetRotationActive() ? node.GetPreRotation(FbxNode.EPivotSet.eSourcePivot)
                        : new FbxVector4();
                    var fbxPreRotationInverse = new FbxQuaternion();
                    fbxPreRotationInverse.ComposeSphericalXYZ(fbxPreRotationEuler);
                    fbxPreRotationInverse.Inverse();

                    // If we're only animating along certain coords for some
                    // reason, we'll need to fill in the other coords with the
                    // rest-pose value.
                    var lclQuaternion = new FbxQuaternion(restRotation.x, restRotation.y, restRotation.z, restRotation.w);

                    // Find when we have keys set.
                    var keyTimes = new HashSet<float>();
                    if (x != null) { foreach(var key in x.keys) { keyTimes.Add(key.time); } }
                    if (y != null) { foreach(var key in y.keys) { keyTimes.Add(key.time); } }
                    if (z != null) { foreach(var key in z.keys) { keyTimes.Add(key.time); } }
                    if (w != null) { foreach(var key in w.keys) { keyTimes.Add(key.time); } }

                    // Convert to the Key type.
                    var keys = new Key[keyTimes.Count];
                    int i = 0;
                    foreach(var seconds in keyTimes) {

                        // The final animation, including the effect of pre-rotation.
                        // If we have no curve, assume the node has the correct rotation right now.
                        // We need to evaluate since we might only have keys in one of the axes.
                        var fbxFinalAnimation = new FbxQuaternion(
                            (x == null) ? lclQuaternion[0] : x.Evaluate(seconds),
                            (y == null) ? lclQuaternion[1] : y.Evaluate(seconds),
                            (z == null) ? lclQuaternion[2] : z.Evaluate(seconds),
                            (w == null) ? lclQuaternion[3] : w.Evaluate(seconds));

                        // Cancel out the pre-rotation. Order matters. FBX reads left-to-right.
                        // When we run animation we will apply:
                        //      pre-rotation
                        //      then pre-rotation inverse
                        //      then animation.
                        var fbxAnimation = fbxPreRotationInverse * fbxFinalAnimation;

                        // Store the key so we can sort them later.
                        Key key;
                        key.time = FbxTime.FromSecondDouble(seconds);
                        key.euler = fbxAnimation.DecomposeSphericalXYZ();
                        keys[i++] = key;
                    }

                    // Sort the keys by time
                    System.Array.Sort(keys, (Key a, Key b) => a.time.CompareTo(b.time));

                    return keys;
                }

                public void Animate(Transform unityTransform, FbxNode fbxNode, FbxAnimLayer fbxAnimLayer, bool Verbose) {
                    /* Find or create the three curves. */
                    var x = fbxNode.LclRotation.GetCurve(fbxAnimLayer, Globals.FBXSDK_CURVENODE_COMPONENT_X, true);
                    var y = fbxNode.LclRotation.GetCurve(fbxAnimLayer, Globals.FBXSDK_CURVENODE_COMPONENT_Y, true);
                    var z = fbxNode.LclRotation.GetCurve(fbxAnimLayer, Globals.FBXSDK_CURVENODE_COMPONENT_Z, true);

                    /* set the keys */
                    x.KeyModifyBegin();
                    y.KeyModifyBegin();
                    z.KeyModifyBegin();

                    var keys = ComputeKeys(unityTransform.localRotation, fbxNode);
                    for(int i = 0, n = keys.Length; i < n; ++i) {
                        var key = keys[i];
                        x.KeyAdd(key.time);
                        x.KeySet(i, key.time, (float)key.euler.X);

                        y.KeyAdd(key.time);
                        y.KeySet(i, key.time, (float)key.euler.Y);

                        z.KeyAdd(key.time);
                        z.KeySet(i, key.time, (float)key.euler.Z);
                    }

                    z.KeyModifyEnd();
                    y.KeyModifyEnd();
                    x.KeyModifyEnd();

                    if (Verbose) {
                        Debug.Log("Exported rotation animation for " + fbxNode.GetName());
                    }
                }
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
                FbxScene fbxScene,
                FbxAnimLayer fbxAnimLayer)
            {
                PropertyChannel fbxName;
                // if we don't find the property it might be because it is a blend shape (which can have any name)
                bool foundProperty = PropertyChannel.TryGetValue (unityPropertyName, out fbxName);

                GameObject unityGo = GetGameObject(unityObj);
                if (unityGo == null) {
                    Debug.LogError (string.Format("cannot find gameobject for {0}", unityObj.name)); 
                    return;
                }

                FbxNode fbxNode;
                if (!MapUnityObjectToFbxNode.TryGetValue(unityGo, out fbxNode)) {
                    Debug.LogError ("no fbx node");
                    return;
                }

                // map unity property name to fbx property
                FbxProperty fbxProperty = null;
                if (foundProperty) {
                    fbxProperty = fbxNode.FindProperty (fbxName.Property, false);
                    if (!fbxProperty.IsValid ()) {
                        Debug.LogError (string.Format ("no fbx property {0} found on {1} ", fbxName.Property, fbxNode.GetName ()));
                        return;
                    }
                } else {
                    // check if the property is in a blend shape
                    FbxMesh fbxMesh = fbxNode.GetMesh();
                    if (fbxMesh == null) {
                        Debug.LogError (string.Format ("no mesh or blend shape found on {0}, could not find proeprty {1}",
                            fbxNode.GetName (), unityPropertyName));
                        return;
                    }

                    // find an FbxDeformer with the same name as the property
                    for(int i = 0; i < fbxMesh.GetDeformerCount(FbxDeformer.EDeformerType.eBlendShape); i++){
                        FbxBlendShape fbxDeformer = fbxMesh.GetBlendShapeDeformer(i);
                        if(fbxDeformer == null){
                            continue;
                        }
                        if (fbxDeformer.GetName ().Equals (unityPropertyName.Replace ("blendShape.", ""))) {
                            fbxProperty = fbxDeformer.GetBlendShapeChannel(0).FindProperty ("DeformPercent", false);
                            if (fbxProperty.IsValid ()) {
                                break;
                            }
                        }
                      }

                    if (fbxProperty == null || !fbxProperty.IsValid ()) {
                        Debug.LogError (string.Format ("no blend shape found on {0}, could not find mapping for unity property {1}",
                            fbxNode.GetName (), unityPropertyName));
                        return;
                    }
                }

                if (Verbose) {
                    Debug.Log("Exporting animation for " + unityObj.name + " (" + unityPropertyName + ")");
                }

                // Create the AnimCurve on the channel
                FbxAnimCurve fbxAnimCurve = fbxProperty.GetCurve (fbxAnimLayer, foundProperty? fbxName.Channel : null, true);

                // copy Unity AnimCurve to FBX AnimCurve.
                fbxAnimCurve.KeyModifyBegin();

                for(int keyIndex = 0, n = unityAnimCurve.length; keyIndex < n; ++keyIndex) {
                    var key = unityAnimCurve[keyIndex];
                    var fbxTime = FbxTime.FromSecondDouble(key.time);
                    fbxAnimCurve.KeyAdd (fbxTime);
                    fbxAnimCurve.KeySet (keyIndex, fbxTime, key.value);
                }

                fbxAnimCurve.KeyModifyEnd();
            }

            /// <summary>
            /// Export an AnimationClip as a single take
            /// </summary>
            protected void ExportAnimationClip (AnimationClip unityAnimClip, GameObject unityRoot, FbxScene fbxScene)
            {
                if (Verbose)
                    Debug.Log (string.Format ("exporting clip {1} for {0}", unityRoot.name, unityAnimClip.name));

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
                    timeMode = FbxTime.ConvertFrameRateToTimeMode(unityAnimClip.frameRate, precision);
                    precision *= 10;
                }
                if (timeMode == FbxTime.EMode.eCustom) {
                    timeMode = FbxTime.EMode.eFrames30;
                }
                FbxTime.SetGlobalTimeMode (timeMode);

                // set time correctly
                var fbxStartTime = FbxTime.FromSecondDouble(0);
                var fbxStopTime = FbxTime.FromSecondDouble(unityAnimClip.length);

                fbxAnimStack.SetLocalTimeSpan (new FbxTimeSpan(fbxStartTime, fbxStopTime));

                /* The major difficulty: Unity uses quaternions for rotation
                 * (which is how it should be) but FBX uses euler angles. So we
                 * need to gather up the list of transform curves per object. */
                var quaternions = new Dictionary<UnityEngine.GameObject, QuaternionCurve>();

                foreach (EditorCurveBinding unityCurveBinding in AnimationUtility.GetCurveBindings(unityAnimClip))
                {
                    Object unityObj = AnimationUtility.GetAnimatedObject (unityRoot, unityCurveBinding);
                    if (!unityObj) { continue; }

                    if (Verbose)
                        Debug.Log (string.Format ("export binding {1} for {0}", unityCurveBinding.propertyName, unityObj.ToString()));

                    AnimationCurve unityAnimCurve = AnimationUtility.GetEditorCurve (unityAnimClip, unityCurveBinding);
                    if (unityAnimCurve == null) { continue; }

                    int index = QuaternionCurve.GetQuaternionIndex(unityCurveBinding.propertyName);
                    if (index == -1) {
                        /* Some normal property (e.g. translation), export right away */
                        ExportAnimCurve (unityObj, unityAnimCurve, unityCurveBinding.propertyName,
                            fbxScene, fbxAnimLayer);
                    } else {
                        /* Rotation property; save it to convert quaternion -> euler later. */

                        var unityGo = GetGameObject(unityObj);
                        if (!unityGo) { continue; }

                        QuaternionCurve quat;
                        if (!quaternions.TryGetValue(unityGo, out quat)) {
                            quat = new QuaternionCurve();
                            quaternions.Add(unityGo, quat);
                        }
                        quat.SetCurve(index, unityAnimCurve);
                    }
                }

                /* now export all the quaternion curves */
                foreach(var kvp in quaternions) {
                    var unityGo = kvp.Key;
                    var quat = kvp.Value;

                    FbxNode fbxNode;
                    if (!MapUnityObjectToFbxNode.TryGetValue(unityGo, out fbxNode)) {
                        Debug.LogError (string.Format("no fbxnode found for '0'", unityGo.name));
                        continue;
                    }
                    quat.Animate(unityGo.transform, fbxNode, fbxAnimLayer, Verbose);
                }
            }

            protected void ExportComponents (GameObject  unityGo, FbxScene fbxScene, FbxNode fbxParentNode)
            {
                ExportSkinnedMesh (unityGo, fbxScene, fbxParentNode);
                ExportAnimationClips(unityGo, fbxScene);

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
                    var fbxIOSettings = FbxIOSettings.Create (fbxManager, Globals.IOSROOT);

                    // Export embedded textures
                    fbxIOSettings.SetBoolProp (Globals.EXP_FBX_EMBEDDED, true);

                    fbxManager.SetIOSettings (fbxIOSettings);

                    // Export embedded textures
                    fbxManager.GetIOSettings ().SetBoolProp (Globals.EXP_FBX_EMBEDDED, true);

                    // Create the exporter 
                    var fbxExporter = FbxExporter.Create (fbxManager, "Exporter");

                    // Initialize the exporter.
                    // NOTE: only the binary FBX file format supports embedded media
                    int fileFormat = -1; // automatically detect the file format

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

                    var fbxSettings = fbxScene.GetGlobalSettings();
                    fbxSettings.SetSystemUnit(FbxSystemUnit.m); // Unity unit is meters

                    /// The Unity axis system has Y up, Z forward, X to the right (left handed system with odd parity).
                    /// The Maya axis system has Y up, Z forward, X to the left (right handed system with odd parity).
                    /// We need to export right-handed for Maya because ConvertScene can't switch handedness:
                    /// https://forums.autodesk.com/t5/fbx-forum/get-confused-with-fbxaxissystem-convertscene/td-p/4265472
                    /// NOTE: models will flipped about the -X axis.
                    var axisSystem = FbxAxisSystem.MayaYUp;

                    fbxSettings.SetAxisSystem(axisSystem);

                    FbxNode fbxRootNode = fbxScene.GetRootNode ();

                    // export set of objects
                    foreach (var obj in unityExportSet) 
                    {
                        var  unityGo  = GetGameObject (obj);

                        if ( unityGo ) 
                        {
                            ExportComponents (unityGo, fbxScene, fbxRootNode);
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
                else if (obj is SkinnedMeshRenderer)
                {
                    var skinnedMesh = obj as SkinnedMeshRenderer;
                    return skinnedMesh.gameObject;   
                }

                return null;
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
