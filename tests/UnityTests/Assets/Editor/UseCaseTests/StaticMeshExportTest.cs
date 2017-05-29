// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using FbxSdk;

namespace UseCaseTests
{
    public class StaticMeshExportTest : RoundTripTest
    {
        // Define the corners of a cube that spans from
        // -50 to 50 on the x and z axis, and 0 to 100 on the y axis
        private FbxVector4 vertex0 = new FbxVector4(-50, 0, 50);
        private FbxVector4 vertex1 = new FbxVector4(50, 0, 50);
        private FbxVector4 vertex2 = new FbxVector4(50, 100, 50);
        private FbxVector4 vertex3 = new FbxVector4(-50, 100, 50);
        private FbxVector4 vertex4 = new FbxVector4(-50, 0, -50);
        private FbxVector4 vertex5 = new FbxVector4(50, 0, -50);
        private FbxVector4 vertex6 = new FbxVector4(50, 100, -50);
        private FbxVector4 vertex7 = new FbxVector4(-50, 100, -50);

        // Control points for generating a simple cube
        private FbxVector4[] m_controlPoints;

        [SetUp]
        public override void Init ()
        {
            fileNamePrefix = "_safe_to_delete__static_mesh_export_test_";
            base.Init ();

            m_controlPoints = new FbxVector4[] {
                vertex0, vertex1, vertex2, vertex3, // Face 1
                vertex1, vertex5, vertex6, vertex2, // Face 2
                vertex5, vertex4, vertex7, vertex6, // Face 3
                vertex4, vertex0, vertex3, vertex7, // Face 4
                vertex3, vertex2, vertex6, vertex7, // Face 5
                vertex1, vertex0, vertex4, vertex5, // Face 6
            };
        }

        protected override FbxScene CreateScene (FbxManager manager)
        {
            // Create a cube as a static mesh

            FbxScene scene = FbxScene.Create (manager, "myScene");

            FbxNode meshNode = FbxNode.Create (scene, "MeshNode");
            FbxMesh cubeMesh = FbxMesh.Create (scene, "cube");

            meshNode.SetNodeAttribute (cubeMesh);

            scene.GetRootNode ().AddChild (meshNode);

            cubeMesh.InitControlPoints (24);

            for (int i = 0; i < cubeMesh.GetControlPointsCount (); i++) {
                cubeMesh.SetControlPointAt (m_controlPoints [i], i);
            }

            return scene;
        }

        protected override void CheckScene (FbxScene scene)
        {
            FbxScene origScene = CreateScene (FbxManager);

            Assert.IsNotNull (origScene);
            Assert.IsNotNull (scene);

            // Retrieve the mesh from each scene
            FbxMesh origMesh = origScene.GetRootNode().GetChild(0).GetMesh();
            FbxMesh importMesh = scene.GetRootNode ().GetChild(0).GetMesh ();

            Assert.IsNotNull (origMesh);
            Assert.IsNotNull (importMesh);

            // check that the control points match
            Assert.AreEqual(origMesh.GetControlPointsCount(), importMesh.GetControlPointsCount());

            for (int i = 0; i < origMesh.GetControlPointsCount (); i++) {
                FbxVector4 origControlPoint = origMesh.GetControlPointAt (i);
                FbxVector4 importControlPoint = importMesh.GetControlPointAt (i);

                // Note: Ignoring W as no matter what it is set to it always imports as 0
                Assert.AreEqual (origControlPoint.X, importControlPoint.X);
                Assert.AreEqual (origControlPoint.Y, importControlPoint.Y);
                Assert.AreEqual (origControlPoint.Z, importControlPoint.Z);
            }
        }
    }
}