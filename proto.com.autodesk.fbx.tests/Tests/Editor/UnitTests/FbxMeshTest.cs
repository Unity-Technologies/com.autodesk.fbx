// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

using NUnit.Framework;
using System.Collections;
using Autodesk.Fbx;

namespace Autodesk.Fbx.UnitTests
{
    public class FbxMeshTest : FbxGeometryTestBase<FbxMesh>
    {
        [Test]
        public void TestBasics()
        {
            base.TestBasics(CreateObject("mesh"), FbxNodeAttribute.EType.eMesh);

            using (FbxMesh mesh = CreateObject ("mesh")) {
                int polyCount = 0;
                int polyVertexCount = 0;

                mesh.InitControlPoints(4);
                mesh.SetControlPointAt(new FbxVector4(0,0,0), 0);
                mesh.SetControlPointAt(new FbxVector4(1,0,0), 1);
                mesh.SetControlPointAt(new FbxVector4(1,0,1), 2);
                mesh.SetControlPointAt(new FbxVector4(0,0,1), 3);
                mesh.BeginPolygon();
                mesh.AddPolygon(0); polyVertexCount++;
                mesh.AddPolygon(1); polyVertexCount++;
                mesh.AddPolygon(2); polyVertexCount++;
                mesh.AddPolygon(3); polyVertexCount++;
                mesh.EndPolygon();
                polyCount++;

                // Link a poly to a material (even though we don't have any).
                mesh.BeginPolygon(0);
                mesh.AddPolygon(0); polyVertexCount++;
                mesh.AddPolygon(1); polyVertexCount++;
                mesh.AddPolygon(2); polyVertexCount++;
                mesh.AddPolygon(3); polyVertexCount++;
                mesh.EndPolygon();
                polyCount++;

                // Link a poly to a material and texture (even though we don't have any).
                mesh.BeginPolygon(0, 0);
                mesh.AddPolygon(0); polyVertexCount++;
                mesh.AddPolygon(1); polyVertexCount++;
                mesh.AddPolygon(2); polyVertexCount++;
                mesh.AddPolygon(3); polyVertexCount++;
                mesh.EndPolygon();
                polyCount++;

                // Create a group.
                mesh.BeginPolygon(-1, -1, 0);
                mesh.AddPolygon(0); polyVertexCount++;
                mesh.AddPolygon(1); polyVertexCount++;
                mesh.AddPolygon(2); polyVertexCount++;
                mesh.AddPolygon(3); polyVertexCount++;
                mesh.EndPolygon();
                polyCount++;

                // Create a non-legacy group polygon.
                mesh.BeginPolygon(-1, -1, 0, false);
                mesh.AddPolygon(0); polyVertexCount++;
                mesh.AddPolygon(1); polyVertexCount++;
                mesh.AddPolygon(2); polyVertexCount++;
                mesh.AddPolygon(3); polyVertexCount++;
                mesh.EndPolygon();
                polyCount++;

                // Create a polygon with UV indices (even though we don't have any)
                mesh.BeginPolygon(0);
                mesh.AddPolygon(0, 0);  polyVertexCount++;
                mesh.AddPolygon(1, 1);  polyVertexCount++;
                mesh.AddPolygon(2, 2);  polyVertexCount++;
                mesh.AddPolygon(3, 3);  polyVertexCount++;
                mesh.EndPolygon();
                polyCount++;

                Assert.AreEqual (mesh.GetPolygonCount (), polyCount);
                Assert.AreEqual (mesh.GetPolygonSize (polyCount - 1), 4);
                Assert.AreEqual (mesh.GetPolygonVertex (polyCount - 1, 0), 0);
                Assert.AreEqual ( mesh.GetPolygonVertexCount (), polyVertexCount);
                Assert.AreEqual (mesh.GetPolygonCount (), polyCount);
            }
        }

        [Test]
        public void TestBeginBadPolygonCreation()
        {
            // Add before begin. This crashes in native FBX SDK.
            using (FbxMesh mesh = CreateObject ("mesh")) {
                Assert.That(() => mesh.AddPolygon(0), Throws.Exception.TypeOf<FbxMesh.BadBracketingException>());
            }

            // End before begin. This is benign in native FBX SDK.
            using (FbxMesh mesh = CreateObject ("mesh")) {
                Assert.That(() => mesh.EndPolygon(), Throws.Exception.TypeOf<FbxMesh.BadBracketingException>());
            }

            // Begin during begin. This is benign in native FBX SDK.
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon();
                Assert.That(() => mesh.BeginPolygon(), Throws.Exception.TypeOf<FbxMesh.BadBracketingException>());
            }

            // Negative polygon index. Benign in FBX SDK, but it will crash some importers.
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon ();
                Assert.That(() => mesh.AddPolygon (-1), Throws.Exception.TypeOf<System.ArgumentOutOfRangeException>());
            }
        }
    }

    public class FbxMeshBadBracketingExceptionTest {
#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxMesh.BadBracketingException), this.GetType()); }

        static FbxMeshBadBracketingExceptionTest()
        {
            // We don't test Exception.GetObjectData ; we assume that the C#
            // compiler and runtime can make it work.
            CoverageTester.RegisterReflectionCall(
                    typeof(FbxMeshBadBracketingExceptionTest).GetMethod("BasicTests"),
                    typeof(FbxMesh.BadBracketingException).GetMethod("GetObjectData"));
        }
#endif

        [Test]
        public void BasicTests()
        {
            // BadBracketingException()
            var xcp = new FbxMesh.BadBracketingException();
            xcp.HelpLink = "http://127.0.0.1";
            Assert.AreEqual("http://127.0.0.1", xcp.HelpLink);
            Assert.AreNotEqual("", xcp.Message);
            xcp.Source = "source";
            Assert.AreEqual("source", xcp.Source);
            Assert.AreNotEqual("", xcp.StackTrace);
            Assert.IsNull(xcp.InnerException);
            Assert.AreEqual(xcp, xcp.GetBaseException());
            Assert.IsNull(xcp.TargetSite);
            Assert.IsNotNull(xcp.Data);
            Assert.AreEqual(typeof(FbxMesh.BadBracketingException), xcp.GetType());

            // BadBracketingException(string message)
            xcp = new FbxMesh.BadBracketingException("oops");
            xcp.HelpLink = "http://127.0.0.1";
            Assert.AreEqual("http://127.0.0.1", xcp.HelpLink);
            Assert.AreNotEqual("", xcp.Message);
            xcp.Source = "source";
            Assert.AreEqual("source", xcp.Source);
            Assert.AreNotEqual("", xcp.StackTrace);
            Assert.IsNull(xcp.InnerException);
            Assert.AreEqual(xcp, xcp.GetBaseException());
            Assert.IsNull(xcp.TargetSite);
            Assert.IsNotNull(xcp.Data);
            Assert.AreEqual(typeof(FbxMesh.BadBracketingException), xcp.GetType());

            // BadBracketingException(string message, System.Exception innerException)
            xcp = new FbxMesh.BadBracketingException("oops", new System.Exception());
            xcp.HelpLink = "http://127.0.0.1";
            Assert.AreEqual("http://127.0.0.1", xcp.HelpLink);
            Assert.AreNotEqual("", xcp.Message);
            xcp.Source = "source";
            Assert.AreEqual("source", xcp.Source);
            Assert.AreNotEqual("", xcp.StackTrace);
            Assert.IsNotNull(xcp.InnerException);

            // The base exception becomes the inner exception here since this represents a chain of exceptions
            Assert.AreNotEqual(xcp, xcp.GetBaseException());
            Assert.AreEqual(xcp.InnerException, xcp.GetBaseException());
            Assert.IsNull(xcp.TargetSite);
            Assert.IsNotNull(xcp.Data);
            Assert.AreEqual(typeof(FbxMesh.BadBracketingException), xcp.GetType());


        }
    }
}
