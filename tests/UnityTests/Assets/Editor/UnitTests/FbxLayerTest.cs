// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

using NUnit.Framework;
using System.Collections;
using FbxSdk;

namespace UnitTests
{
    public class FbxLayerTest
    {

        private FbxMesh m_fbxMesh;
        private FbxManager m_fbxManager;
        private FbxLayer m_fbxLayer;

        [SetUp]
        public void Init ()
        {
            m_fbxManager = FbxManager.Create ();
            m_fbxMesh = FbxMesh.Create (m_fbxManager, "");
            m_fbxLayer = m_fbxMesh.GetLayer (0);
            if (m_fbxLayer == null)
            {
                m_fbxMesh.CreateLayer ();
                m_fbxLayer = m_fbxMesh.GetLayer (0 /* default layer */);
            }
        }

        [TearDown]
        public void Term ()
        {
            m_fbxManager.Destroy ();
        }

        [Test]
        public void TestSetNormals ()
        {
            // make sure nothing crashes

            m_fbxLayer.SetNormals (FbxLayerElementNormal.Create (m_fbxMesh, ""));

            // test null
            m_fbxLayer.SetNormals(null);

            // test destroyed
            FbxLayerElementNormal normals = FbxLayerElementNormal.Create (m_fbxMesh, "");
            normals.Dispose ();
            m_fbxLayer.SetNormals (normals);
        }

        [Test]
        public void TestSetBinormals ()
        {
            // make sure nothing crashes

            m_fbxLayer.SetBinormals (FbxLayerElementBinormal.Create (m_fbxMesh, ""));

            // test null
            m_fbxLayer.SetBinormals(null);

            // test destroyed
            FbxLayerElementBinormal binormals = FbxLayerElementBinormal.Create (m_fbxMesh, "");
            binormals.Dispose ();
            m_fbxLayer.SetBinormals (binormals);
        }

        [Test]
        public void TestSetTangents ()
        {
            // make sure nothing crashes

            m_fbxLayer.SetTangents (FbxLayerElementTangent.Create (m_fbxMesh, ""));

            // test null
            m_fbxLayer.SetTangents(null);

            // test destroyed
            FbxLayerElementTangent tangents = FbxLayerElementTangent.Create (m_fbxMesh, "");
            tangents.Dispose ();
            m_fbxLayer.SetTangents (tangents);
        }

        [Test]
        public void TestSetVertexColors ()
        {
            // make sure nothing crashes

            m_fbxLayer.SetVertexColors (FbxLayerElementVertexColor.Create (m_fbxMesh, ""));

            // test null
            m_fbxLayer.SetVertexColors(null);

            // test destroyed
            FbxLayerElementVertexColor vertexColor = FbxLayerElementVertexColor.Create (m_fbxMesh, "");
            vertexColor.Dispose ();
            m_fbxLayer.SetVertexColors(vertexColor);
        }

        [Test]
        public void TestSetMaterials()
        {
            // make sure nothing crashes

            m_fbxLayer.SetMaterials(FbxLayerElementMaterial.Create (m_fbxMesh, ""));

            // test null
            m_fbxLayer.SetMaterials(null);

            // test destroyed
            FbxLayerElementMaterial material = FbxLayerElementMaterial.Create (m_fbxMesh, "");
            material.Dispose ();
            m_fbxLayer.SetMaterials(material);
        }

        [Test]
        public void TestSetUVs ()
        {
            // make sure nothing crashes

            m_fbxLayer.SetUVs (FbxLayerElementUV.Create (m_fbxMesh, ""));

            // test with type identifier
            m_fbxLayer.SetUVs(FbxLayerElementUV.Create (m_fbxMesh, ""), FbxLayerElement.EType.eEdgeCrease);

            // test null
            m_fbxLayer.SetUVs(null);

            // test destroyed
            FbxLayerElementUV uvs = FbxLayerElementUV.Create (m_fbxMesh, "");
            uvs.Dispose ();
            m_fbxLayer.SetUVs (uvs);
        }

        [Test]
        public void TestDispose()
        {
            // make sure that calling SetNormals on a disposed layer throws
            m_fbxLayer.Dispose ();
            Assert.That(() => m_fbxLayer.SetNormals (FbxLayerElementNormal.Create(m_fbxMesh, "")),
                Throws.Exception.TypeOf<System.NullReferenceException>());
        }

        #if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxLayer), this.GetType()); }
        #endif
    }
}
