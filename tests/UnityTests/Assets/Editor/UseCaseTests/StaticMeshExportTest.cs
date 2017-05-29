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
        protected FbxVector4 vertex0 = new FbxVector4(-50, 0, 50);
        protected FbxVector4 vertex1 = new FbxVector4(50, 0, 50);
        protected FbxVector4 vertex2 = new FbxVector4(50, 100, 50);
        protected FbxVector4 vertex3 = new FbxVector4(-50, 100, 50);
        protected FbxVector4 vertex4 = new FbxVector4(-50, 0, -50);
        protected FbxVector4 vertex5 = new FbxVector4(50, 0, -50);
        protected FbxVector4 vertex6 = new FbxVector4(50, 100, -50);
        protected FbxVector4 vertex7 = new FbxVector4(-50, 100, -50);

        // Control points for generating a simple cube
        protected FbxVector4[] m_controlPoints;

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

    public class StaticMeshWithNormalsExportTest : StaticMeshExportTest 
    {
        // Define normal vectors along each axis
        protected FbxVector4 normalXPos = new FbxVector4(1,0,0);
        protected FbxVector4 normalXNeg = new FbxVector4(-1,0,0);
        protected FbxVector4 normalYPos = new FbxVector4(0,1,0);
        protected FbxVector4 normalYNeg = new FbxVector4(0,-1,0);
        protected FbxVector4 normalZPos = new FbxVector4(0,0,1);
        protected FbxVector4 normalZNeg = new FbxVector4(0,0,-1);

        protected override FbxScene CreateScene (FbxManager manager)
        {
            FbxScene scene = base.CreateScene (manager);

            // Add normals, binormals, UVs, tangents and vertex colors to the cube
            FbxMesh cubeMesh = scene.GetRootNode ().GetChild (0).GetMesh ();

            // Add normals
            /// Set the Normals on Layer 0.
            FbxLayer fbxLayer = cubeMesh.GetLayer (0 /* default layer */);
            if (fbxLayer == null)
            {
                cubeMesh.CreateLayer ();
                fbxLayer = cubeMesh.GetLayer (0 /* default layer */);
            }

            using (var fbxLayerElement = FbxLayerElementNormal.Create (cubeMesh, "Normals")) 
            {
                fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);
                fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);

                // Add one normal per each vertex face index (3 per triangle)
                FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                // Assign the normal vectors in the same order the control points were defined
                FbxVector4[] normals = {normalZPos, normalXPos, normalZNeg, normalXNeg, normalYPos, normalYNeg};
                for (int n = 0; n < normals.Length; n++) {
                    for (int i = 0; i < 4; i++) {
                        fbxElementArray.Add (normals [n]);
                    }
                }
                fbxLayer.SetNormals (fbxLayerElement);
            }

            /// Set the binormals on Layer 0. 
            using (var fbxLayerElement = FbxLayerElementBinormal.Create (cubeMesh, "Binormals")) 
            {
                fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);
                fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);

                // Add one normal per each vertex face index (3 per triangle)
                FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                for (int n = 0; n < cubeMesh.GetControlPointsCount(); n++) {
                    fbxElementArray.Add (new FbxVector4 (-1,0,1)); // TODO: set to correct values
                }
                fbxLayer.SetBinormals (fbxLayerElement);
            }

            /// Set the tangents on Layer 0.
            using (var fbxLayerElement = FbxLayerElementTangent.Create (cubeMesh, "Tangents")) 
            {
                fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);
                fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);

                // Add one normal per each vertex face index (3 per triangle)
                FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                for (int n = 0; n < cubeMesh.GetControlPointsCount(); n++) {
                    fbxElementArray.Add (new FbxVector4 (0,-1,1)); // TODO: set to correct values
                }
                fbxLayer.SetTangents (fbxLayerElement);
            }

            // set the vertex colors
            using (var fbxLayerElement = FbxLayerElementVertexColor.Create (cubeMesh, "VertexColors")) 
            {
                fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);
                fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);

                // Add one normal per each vertex face index (3 per triangle)
                FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                // make each vertex either black or white
                for (int n = 0; n < cubeMesh.GetControlPointsCount (); n++) {
                    fbxElementArray.Add (new FbxColor (n % 2, n % 2, n % 2));
                }

                fbxLayer.SetVertexColors (fbxLayerElement);
            }

            // set the UVs
            using (var fbxLayerElement = FbxLayerElementUV.Create (cubeMesh, "UVSet"))
            {
                fbxLayerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByPolygonVertex);
                fbxLayerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eIndexToDirect);

                // set texture coordinates per vertex
                FbxLayerElementArray fbxElementArray = fbxLayerElement.GetDirectArray ();

                for (int n = 0; n < 8; n++) {
                    fbxElementArray.Add (new FbxVector2 (n % 2,1)); // TODO: switch to correct values
                }

                // For each face index, point to a texture uv
                FbxLayerElementArray fbxIndexArray = fbxLayerElement.GetIndexArray ();
                fbxIndexArray.SetCount (12);

                for (int vertIndex = 0; vertIndex < 12; vertIndex++)
                {
                    fbxIndexArray.SetAt (vertIndex, vertIndex % 8); // TODO: switch to correct values
                }
                fbxLayer.SetUVs (fbxLayerElement, FbxLayerElement.EType.eTextureDiffuse);
            }

            return scene;
        }

        protected override void CheckScene (FbxScene scene)
        {
            base.CheckScene (scene);

            FbxScene origScene = CreateScene (FbxManager);
            Assert.IsNotNull (origScene);

            // Retrieve the mesh from each scene
            FbxMesh origMesh = origScene.GetRootNode().GetChild(0).GetMesh();
            FbxMesh importMesh = scene.GetRootNode ().GetChild(0).GetMesh ();

            // get the layers
            FbxLayer origLayer = origMesh.GetLayer (0 /* default layer */);
            FbxLayer importLayer = importMesh.GetLayer (0 /* default layer */);

            // Check normals
            CheckFbxElementVector4(origLayer.GetNormals(), importLayer.GetNormals());

			// Check binormals
            CheckFbxElementVector4(origLayer.GetBinormals(), importLayer.GetBinormals());

			// Check tangents
            CheckFbxElementVector4(origLayer.GetTangents(), importLayer.GetTangents());

            // Check vertex colors
            var origVertexColorElement = origLayer.GetVertexColors();
            var importVertexColorElement = importLayer.GetVertexColors ();

            Assert.AreEqual (origVertexColorElement.GetMappingMode (), importVertexColorElement.GetMappingMode());
            Assert.AreEqual (origVertexColorElement.GetReferenceMode (), importVertexColorElement.GetReferenceMode ());

            var origVertexColorElementArray = origVertexColorElement.GetDirectArray ();
            var importVertexColorElementArray = importVertexColorElement.GetDirectArray ();

            Assert.AreEqual (origVertexColorElementArray.GetCount (), importVertexColorElementArray.GetCount ());

            for (int i = 0; i < origVertexColorElementArray.GetCount (); i++) {
                Assert.AreEqual (origVertexColorElementArray.GetAt (i), importVertexColorElementArray.GetAt (i));
            }

            // Check UVs
            var origUVElement = origLayer.GetUVs();
            var importUVElement = importLayer.GetUVs ();

            Assert.AreEqual (origUVElement.GetMappingMode (), importUVElement.GetMappingMode());
            Assert.AreEqual (origUVElement.GetReferenceMode (), importUVElement.GetReferenceMode ());

            var origUVElementArray = origUVElement.GetDirectArray ();
            var importUVElementArray = importUVElement.GetDirectArray ();

            Assert.AreEqual (origUVElementArray.GetCount (), importUVElementArray.GetCount ());

            for (int i = 0; i < origUVElementArray.GetCount (); i++) {
                Assert.AreEqual (origUVElementArray.GetAt (i), importUVElementArray.GetAt (i));
            }

            var origUVElementIndex = origUVElement.GetIndexArray ();
            var importUVElementIndex = origUVElement.GetIndexArray ();

            Assert.AreEqual (origUVElementIndex.GetCount (), importUVElementIndex.GetCount ());

            for (int i = 0; i < origUVElementIndex.GetCount (); i++) {
                Assert.AreEqual (origUVElementIndex.GetAt (i), importUVElementIndex.GetAt (i));
            }
        }

        // helper for above, to check normals, binormals, and tangents
        protected void CheckFbxElementVector4(
            FbxLayerElementTemplateFbxVector4 origElement,
            FbxLayerElementTemplateFbxVector4 importElement)
		{
            Assert.AreEqual (origElement.GetMappingMode (), importElement.GetMappingMode());
            Assert.AreEqual (origElement.GetReferenceMode (), importElement.GetReferenceMode ());

            var origElementArray = origElement.GetDirectArray ();
            var importElementArray = importElement.GetDirectArray ();

            Assert.AreEqual (origElementArray.GetCount (), importElementArray.GetCount ());

            for (int i = 0; i < origElementArray.GetCount (); i++) {
                Assert.AreEqual (origElementArray.GetAt (i), importElementArray.GetAt (i));
            }
		}
    }
}