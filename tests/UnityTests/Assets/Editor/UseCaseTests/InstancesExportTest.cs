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
    public class InstancesExportTest : TransformExportTest
    {
        protected string m_meshName = "shared mesh";

        protected override FbxScene CreateScene (FbxManager manager)
        {
            FbxScene scene = base.CreateScene (manager);

            // mesh shared by all instances
            FbxMesh sharedMesh = FbxMesh.Create (scene, m_meshName);

            // add mesh to all nodes
            Queue<FbxNode> nodes = new Queue<FbxNode>();
            for (int i = 0; i < scene.GetRootNode ().GetChildCount (); i++) {
                nodes.Enqueue (scene.GetRootNode ().GetChild (i));
            }

            while (nodes.Count > 0) {
                FbxNode node = nodes.Dequeue ();
                node.SetNodeAttribute (sharedMesh);
                for (int i = 0; i < node.GetChildCount (); i++) {
                    nodes.Enqueue (node.GetChild (i));
                }
            }

            return scene;
        }

        protected override void CheckScene (FbxScene scene)
        {
            base.CheckScene (scene);

            FbxNode rootNode = scene.GetRootNode ().GetChild(0);
            Assert.IsNotNull (rootNode);

            FbxMesh sharedMesh = rootNode.GetMesh ();

            Assert.IsNotNull (sharedMesh);
            Assert.AreEqual (m_meshName, sharedMesh.GetName ());

            // check that the mesh is the same for all children
            CheckSceneHelper (rootNode, sharedMesh);
        }

        private void CheckSceneHelper(FbxNode node, FbxMesh mesh)
        {
            if (node == null) {
                return;
            }

            Assert.AreEqual (mesh, node.GetMesh ());

            for (int i = 0; i < node.GetChildCount (); i++) {
                // recurse through the hierarchy
                CheckSceneHelper (node.GetChild (i), mesh);
            }
        }
    }
}