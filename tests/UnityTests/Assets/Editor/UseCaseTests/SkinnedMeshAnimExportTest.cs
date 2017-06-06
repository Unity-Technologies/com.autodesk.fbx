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
    public class SkinnedMeshAnimExportTest : SkinnedMeshExportTest
    {
        protected override FbxScene CreateScene (FbxManager manager)
        {
            FbxScene scene = base.CreateScene (manager);
            FbxNode animNode = scene.GetRootNode ().GetChild (0);

            // setup anim stack
            FbxAnimStack fbxAnimStack = FbxAnimStack.Create (scene, "animClip");
            fbxAnimStack.Description.Set ("Animation Take");

            // add an animation layer
            FbxAnimLayer fbxAnimLayer = FbxAnimLayer.Create (scene, "animBaseLayer");
            fbxAnimStack.AddMember (fbxAnimLayer);

            FbxTime.SetGlobalTimeMode (FbxTime.EMode.eFrames30);

            // set time correctly
            var fbxStartTime = FbxTime.FromSecondDouble (0);
            var fbxStopTime = FbxTime.FromSecondDouble (25);
            fbxAnimStack.SetLocalTimeSpan (new FbxTimeSpan (fbxStartTime, fbxStopTime));

            AnimationClipsExportTest.CreateAnimCurves (animNode, fbxAnimLayer, new List<AnimationClipsExportTest.PropertyComponentPair> () {
                new AnimationClipsExportTest.PropertyComponentPair ("Lcl Translation", AnimationClipsExportTest.TransformComponents),
                new AnimationClipsExportTest.PropertyComponentPair ("Lcl Rotation", AnimationClipsExportTest.TransformComponents),
                new AnimationClipsExportTest.PropertyComponentPair ("Lcl Scaling", AnimationClipsExportTest.TransformComponents)
            }, (index) => { return index+1; }, (index) => { return index/4.0f; }, /* node attribute =*/ null,  /* keyCount =*/ 10);

            // TODO: avoid needing to this by creating typemaps for
            //       FbxObject::GetSrcObjectCount and FbxCast.
            //       Not trivial to do as both fbxobject.i and fbxemitter.i
            //       have to be moved up before the ignore all statement
            //       to allow use of templates.
            scene.SetCurrentAnimationStack (fbxAnimStack);
            return scene;
        }

        protected override void CheckScene (FbxScene scene)
        {
            base.CheckScene (scene);

            FbxScene origScene = CreateScene (FbxManager);

            FbxNode origAnimNode = origScene.GetRootNode ().GetChild (0);
            FbxNode importAnimNode = scene.GetRootNode ().GetChild (0);

            FbxAnimStack origStack = origScene.GetCurrentAnimationStack ();
            FbxAnimStack importStack = scene.GetCurrentAnimationStack ();

            Assert.IsNotNull (origStack);
            Assert.IsNotNull (importStack);
            Assert.AreEqual (origStack.GetName (), importStack.GetName ());
            Assert.AreEqual (origStack.Description.Get (), importStack.Description.Get ());
            Assert.AreEqual (origStack.GetMemberCount (), importStack.GetMemberCount ());

            FbxAnimLayer origLayer = origStack.GetAnimLayerMember ();
            FbxAnimLayer importLayer = importStack.GetAnimLayerMember ();

            Assert.IsNotNull (origLayer);
            Assert.IsNotNull (importLayer);

            Assert.AreEqual (FbxTime.EMode.eFrames30, FbxTime.GetGlobalTimeMode ());
            Assert.AreEqual (origStack.GetLocalTimeSpan (), importStack.GetLocalTimeSpan ());

            AnimationClipsExportTest.CheckAnimCurve (origAnimNode, importAnimNode, origLayer, importLayer,
                new List<AnimationClipsExportTest.PropertyComponentPair>(){
                new AnimationClipsExportTest.PropertyComponentPair("Lcl Translation", AnimationClipsExportTest.TransformComponents),
                new AnimationClipsExportTest.PropertyComponentPair("Lcl Rotation", AnimationClipsExportTest.TransformComponents),
                new AnimationClipsExportTest.PropertyComponentPair("Lcl Scaling", AnimationClipsExportTest.TransformComponents)
            });
        }
    }
}