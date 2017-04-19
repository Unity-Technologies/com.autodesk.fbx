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
    }
}
