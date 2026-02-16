// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using Autodesk.Fbx;
using System.IO;



namespace Autodesk.Fbx.UseCaseTests
{
    public class DowncastFbxType
    {
        [Test]
        public void TestDowncast ()
        {
            using(FbxManager man = FbxManager.Create())
            {
                FbxMesh m = FbxMesh.Create(man, "some mesh");
                FbxSkin s = FbxSkin.Create(man, "some deformer skin");
                int index = m.AddDeformer(s);
                // FbxSkin o = m.GetDeformer(index) as FbxSkin;
                FbxSkin o = (FbxSkin)m.GetDeformer(index);
                Assert.That(o, Is.Not.Null);
            }
        }

        [Test]
        public void TestWeirdCase()
        {
            string constraintName = "sdkjhsdkjhfkjsdhfjks";
            string fileName = "bobobo.fbx";
            string meshName = "some mesh";
            string skinName = "some skin";
            int index = 0;
            using(FbxManager manager = FbxManager.Create())
            {
                using (FbxExporter exporter = FbxExporter.Create (manager, "myExporter")) 
                {
                    var format = manager.GetIOPluginRegistry().FindWriterIDByDescription("FBX ascii (*.fbx)");
                    // Initialize the exporter.
                    bool status = exporter.Initialize (fileName, format, manager.GetIOSettings ());

                    // Create a scene with a single node that has an animation clip
                    // attached to it
                    FbxScene scene = FbxScene.Create(manager, "myScene");

                    FbxNode sourceNode = FbxNode.Create(scene, "source");
                    FbxNode constrainedNode = FbxNode.Create(scene, "constrained");

                    scene.GetRootNode().AddChild(sourceNode);
                    scene.GetRootNode().AddChild(constrainedNode);

                    FbxMesh m = FbxMesh.Create(manager, meshName);
                    FbxSkin s = FbxSkin.Create(manager, skinName);
                    index = m.AddDeformer(s);
                    scene.AddMember(m);
                    scene.AddMember(s);

                    // FbxConstraint posConstraint = CreatePositionConstraint(scene, sourceNode, constrainedNode);
                    FbxConstraintPosition constraint = FbxConstraintPosition.Create(scene, constraintName);

                    constraint.SetConstrainedObject(constrainedNode);
                    constraint.AddConstraintSource(sourceNode);

                    constraint.AffectX.Set(true);
                    constraint.AffectY.Set(true);
                    constraint.AffectZ.Set(true);

                    constraint.Translation.Set(new FbxDouble3(1, 2, 3));

                    Assert.That(constraint, Is.Not.Null);

                    bool result = constraint.ConnectDstObject(scene);
                    Assert.That(result, Is.True);

                    // export the scene
                    exporter.Export(scene);
                }
                FbxScene importedScene = null;
                // FbxMesh importedMesh = null;
                using (FbxImporter importer = FbxImporter.Create (manager, "myImporter")) 
                {

                    // Initialize the importer.
                    bool status = importer.Initialize (fileName, -1, manager.GetIOSettings ());

                    Assert.IsTrue (status);

                    // Create a new scene so it can be populated by the imported file.
                     importedScene = FbxScene.Create (manager, "myScene");

                    // Import the contents of the file into the scene.
                    importer.Import (importedScene);
                }
                FbxMesh importedMesh = (FbxMesh)importedScene.FindMemberObject(meshName);
                Assert.That(importedMesh, Is.Not.Null);
                FbxSkin importedSkin = (FbxSkin)importedScene.FindMemberObject(skinName);
                Assert.That(importedSkin, Is.Not.Null);
                FbxNode importSourceNode = importedScene.GetRootNode().GetChild(0);
                FbxNode importConstrainedNode = importedScene.GetRootNode().GetChild(1);
                FbxObject importPosConstraint = importedScene.FindSrcObject(constraintName);
                UnityEngine.Debug.Log($"imported type is {importPosConstraint.GetType()}");
                FbxConstraintPosition p = (FbxConstraintPosition)importPosConstraint;
                FbxConstraint pp = (FbxConstraint)importPosConstraint; // fails here
            }
        }
    }
}
// 