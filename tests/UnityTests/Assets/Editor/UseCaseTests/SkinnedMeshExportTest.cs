// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using System.Collections.Generic;
using FbxSdk;

namespace UseCaseTests
{
    public class SkinnedMeshExportTest : StaticMeshExportTest
    {
        [SetUp]
        public override void Init ()
        {
            fileNamePrefix = "_safe_to_delete__skinned_mesh_export_test";
            base.Init ();
        }

        protected override FbxScene CreateScene (FbxManager manager)
        {
            // Add a skeleton to the cube that we created in StaticMeshExportTest
            FbxScene scene = base.CreateScene (manager);

            FbxNode meshNode = scene.GetRootNode().GetChild(0);
            FbxNode skeletonRootNode = CreateSkeleton(scene);

            FbxNode rootNode = scene.GetRootNode ();
            rootNode.AddChild (skeletonRootNode);

            LinkMeshToSkeleton (scene, meshNode, skeletonRootNode);

            FbxNode limb1 = skeletonRootNode.GetChild (0);
            FbxNode limb2 = limb1.GetChild (0);

            ExportBindPose (meshNode, scene, new List<FbxNode> (){ skeletonRootNode, limb1, limb2 });

            return scene;
        }

        protected FbxNode CreateSkeleton(FbxScene scene)
        {
            FbxSkeleton skelRoot = FbxSkeleton.Create (scene, "SkelRoot");
            skelRoot.SetSkeletonType (FbxSkeleton.EType.eRoot);
            FbxNode skelRootNode = FbxNode.Create (scene, "SkelRootNode");
            skelRootNode.SetNodeAttribute (skelRoot);
            skelRootNode.LclTranslation.Set(new FbxDouble3(0.0, -40.0, 0.0));

            // create skeleton limb nodes
            FbxSkeleton skelLimb1 = FbxSkeleton.Create(scene, "SkelLimb1");
            skelLimb1.SetSkeletonType (FbxSkeleton.EType.eLimbNode);
            skelLimb1.Size.Set (1.5);
            FbxNode skelLimbNode1 = FbxNode.Create (scene, "SkelLimbNode1");
            skelLimbNode1.SetNodeAttribute (skelLimb1);
            skelLimbNode1.LclTranslation.Set (new FbxDouble3 (0.0, 40.0, 0.0));

            FbxSkeleton skelLimb2 = FbxSkeleton.Create(scene, "SkelLimb2");
            skelLimb2.SetSkeletonType (FbxSkeleton.EType.eLimbNode);
            skelLimb2.Size.Set (1.5);
            FbxNode skelLimbNode2 = FbxNode.Create (scene, "SkelLimbNode2");
            skelLimbNode2.SetNodeAttribute (skelLimb2);
            skelLimbNode2.LclTranslation.Set (new FbxDouble3 (0.0, 40.0, 0.0));

            // build skeleton hierarchy
            skelRootNode.AddChild (skelLimbNode1);
            skelLimbNode1.AddChild (skelLimbNode2);

            return skelRootNode;
        }

        protected void LinkMeshToSkeleton(FbxScene scene, FbxNode meshNode, FbxNode skelRootNode)
        {
            FbxNode limb1 = skelRootNode.GetChild (0);
            FbxNode limb2 = limb1.GetChild (0);

            FbxCluster rootCluster = FbxCluster.Create (scene, "RootCluster");
            rootCluster.SetLink (skelRootNode);
            rootCluster.SetLinkMode (FbxCluster.ELinkMode.eTotalOne);
            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    rootCluster.AddControlPointIndex (4 * i + j, 1.0 - 0.25 * i);
                }
            }

            FbxCluster limb1Cluster = FbxCluster.Create (scene, "Limb1Cluster");
            limb1Cluster.SetLink (limb1);
            limb1Cluster.SetLinkMode (FbxCluster.ELinkMode.eTotalOne);
            for (int i = 1; i < 6; i++) {
                for (int j = 0; j < 4; j++) {
                    limb1Cluster.AddControlPointIndex (4 * i + j, (i == 1 || i == 5 ? 0.25 : 0.5));
                }
            }

            FbxCluster limb2Cluster = FbxCluster.Create (scene, "Limb2Cluster");
            limb2Cluster.SetLink (limb2);
            limb2Cluster.SetLinkMode (FbxCluster.ELinkMode.eTotalOne);
            for (int i = 3; i < 7; i++) {
                for (int j = 0; j < 4; j++) {
                    limb2Cluster.AddControlPointIndex (4 * i + j, 0.25 * (i - 2));
                }
            }

            FbxAMatrix globalTransform = meshNode.EvaluateGlobalTransform ();

            rootCluster.SetTransformMatrix (globalTransform);
            limb1Cluster.SetTransformMatrix (globalTransform);
            limb2Cluster.SetTransformMatrix (globalTransform);

            rootCluster.SetTransformLinkMatrix (skelRootNode.EvaluateGlobalTransform ());
            limb1Cluster.SetTransformLinkMatrix (limb1.EvaluateGlobalTransform ());
            limb2Cluster.SetTransformLinkMatrix (limb2.EvaluateGlobalTransform ());

            FbxSkin skin = FbxSkin.Create (scene, "Skin");
            skin.AddCluster (rootCluster);
            skin.AddCluster (limb1Cluster);
            skin.AddCluster (limb2Cluster);
            meshNode.GetMesh ().AddDeformer (skin);
        }

        protected void ExportBindPose (FbxNode meshNode, FbxScene fbxScene, List<FbxNode> boneNodes)
        {
            FbxPose fbxPose = FbxPose.Create (fbxScene, "Pose");

            // set as bind pose
            fbxPose.SetIsBindPose (true);

            // assume each bone node has one weighted vertex cluster
            foreach (FbxNode fbxNode in boneNodes)
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

        protected override void CheckScene (FbxScene scene)
        {

        }
    }
}