using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using FbxSdk;

namespace UnitTests
{
    public class FbxMeshTest : Base<FbxMesh>
    {
        [Test]
        public void TestBasics()
        {
            FbxGeometryBaseTest.GenericTests(CreateObject("mesh"));

            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.InitControlPoints(4);
                mesh.SetControlPointAt(new FbxVector4(0,0,0), 0);
                mesh.SetControlPointAt(new FbxVector4(1,0,0), 1);
                mesh.SetControlPointAt(new FbxVector4(1,0,1), 2);
                mesh.SetControlPointAt(new FbxVector4(0,0,1), 3);
                mesh.BeginPolygon();
                mesh.AddPolygon(0);
                mesh.AddPolygon(1);
                mesh.AddPolygon(2);
                mesh.AddPolygon(3);
                mesh.EndPolygon();

                // Link a poly to a material (even though we don't have any).
                mesh.BeginPolygon(0);
                mesh.AddPolygon(0);
                mesh.AddPolygon(1);
                mesh.AddPolygon(2);
                mesh.AddPolygon(3);
                mesh.EndPolygon();

                // Link a poly to a material and texture (even though we don't have any).
                mesh.BeginPolygon(0, 0);
                mesh.AddPolygon(0);
                mesh.AddPolygon(1);
                mesh.AddPolygon(2);
                mesh.AddPolygon(3);
                mesh.EndPolygon();

                // Create a group.
                mesh.BeginPolygon(-1, -1, 0);
                mesh.AddPolygon(0);
                mesh.AddPolygon(1);
                mesh.AddPolygon(2);
                mesh.AddPolygon(3);
                mesh.EndPolygon();

                // Create a non-legacy group polygon.
                mesh.BeginPolygon(-1, -1, 0, false);
                mesh.AddPolygon(0);
                mesh.AddPolygon(1);
                mesh.AddPolygon(2);
                mesh.AddPolygon(3);
                mesh.EndPolygon();

                // Create a polygon with UV indices (even though we don't have any)
                mesh.BeginPolygon(0);
                mesh.AddPolygon(0, 0);
                mesh.AddPolygon(1, 1);
                mesh.AddPolygon(2, 2);
                mesh.AddPolygon(3, 3);
                mesh.EndPolygon();
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
                Assert.That(() => mesh.AddPolygon (-1), Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            }
        }
    }
}
