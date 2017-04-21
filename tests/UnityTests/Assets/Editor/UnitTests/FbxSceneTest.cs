// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxSceneTest : Base<FbxScene>
    {
        [Test]
        public void TestBasics()
        {
            using (var scene = FbxScene.Create(Manager, "scene")) {
                // Just call every function. TODO: and test them at least minimally!
                scene.GetDocumentInfo();
                scene.GetSceneInfo();
                scene.GetGlobalSettings();
                scene.GetRootNode();
                var docInfo = FbxDocumentInfo.Create(Manager, "info");
                scene.SetDocumentInfo(docInfo);
                docInfo = FbxDocumentInfo.Create(Manager, "info2");
                scene.SetSceneInfo(docInfo);

                var sceneB = scene;
                Assert.AreEqual(scene, sceneB);
                var scene2 = FbxScene.Create(Manager, "scene2");
                Assert.AreNotEqual(scene, scene2);

                Assert.That(scene != scene2);
                Assert.That((FbxDocument)scene != (FbxDocument)scene2);
                Assert.That((FbxCollection)scene != (FbxCollection)scene2);
                Assert.That((FbxObject)scene != (FbxObject)scene2);
                Assert.That((FbxEmitter)scene != (FbxEmitter)scene2);

                scene.Clear();
            }
        }

        [Test]
        public void TestNodeCount ()
        {
            using (FbxScene newScene = FbxScene.Create (Manager, ""))
            {
                Assert.GreaterOrEqual (newScene.GetNodeCount (), 0);
            }
        }
    }
}
