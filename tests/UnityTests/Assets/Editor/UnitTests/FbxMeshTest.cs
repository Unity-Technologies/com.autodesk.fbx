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
        public void TestCreateFromObject ()
        {
            using (FbxObject obj = FbxObject.Create (Manager, "object")) {

                FbxMesh mesh = FbxMesh.Create (obj, "mesh");

                Assert.IsInstanceOf<FbxMesh> (mesh);
            }
        }
        
        [Test]
        public void TestBeginPolygonNoArgs ()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon ();

                // what happens if we call it a second time?
                mesh.BeginPolygon ();
            }
        }

        [Test]
        public void TestBeginPolygonInvalidMaterial()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon (0);
            }
        }

        [Test]
        public void TestBeginPolygonInvalidTexture()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon (-1, 0);
            }
        }

        [Test]
        public void TestBeginPolygonGroup()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon (-1, -1, 0);
            }
        }

        [Test]
        public void TestBeginPolygonNotLegacy()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon (-1,-1,-1,false);
            }
        }

        [Test]
        [Ignore("Calling BeginPolygon after AddPolygon crashes Unity")]
        public void TestBeginPolygonAfterAddPolygon()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.AddPolygon (0);
                mesh.BeginPolygon ();
            }
        }

        [Test]
        public void TestBeginPolygonAfterEndPolygon()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.EndPolygon ();
                mesh.BeginPolygon ();
            }
        }

        [Test]
        public void TestAddPolygonNegativeIndex ()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon ();

                mesh.AddPolygon (-1);
            }
        }

        [Test]
        public void TestAddPolygonZeroIndex ()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon ();

                mesh.AddPolygon (0);
            }
        }

        [Test]
        public void TestAddPolygonTextureUVIndex ()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon ();

                mesh.AddPolygon (0, 0);
            }
        }

        [Test]
        public void TestEndPolygonNoBegin ()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.EndPolygon ();
            }
        }

        [Test]
        public void TestEndPolygonNoAdd ()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon ();
                mesh.EndPolygon ();
            }
        }

        [Test]
        public void TestEndPolygonOneAdd ()
        {
            using (FbxMesh mesh = CreateObject ("mesh")) {
                mesh.BeginPolygon ();
                mesh.AddPolygon (0);
                mesh.EndPolygon ();
            }
        }
    }
}
