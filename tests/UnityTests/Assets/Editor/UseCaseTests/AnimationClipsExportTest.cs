// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using System.Collections;
using FbxSdk;

namespace UseCaseTests
{
    public class AnimationClipsExportTest : RoundTripTestBase
    {
        private int m_keyCount = 5;

        [SetUp]
        public override void Init ()
        {
            fileNamePrefix = "_safe_to_delete__animation_clips_export_test";
            base.Init ();
        }

        protected override FbxScene CreateScene (FbxManager manager)
        {
            // Create a scene with a single node that has an animation clip
            // attached to it
            FbxScene scene = FbxScene.Create (manager, "myScene");

            FbxNode animNode = FbxNode.Create (scene, "animNode");

            // setup anim stack
            FbxAnimStack fbxAnimStack = FbxAnimStack.Create (scene, "animClip");
            fbxAnimStack.Description.Set ("Animation Take");

            // add an animation layer
            FbxAnimLayer fbxAnimLayer = FbxAnimLayer.Create (scene, "animBaseLayer");
            fbxAnimStack.AddMember (fbxAnimLayer);

            // Set up the FPS so our frame-relative math later works out
            // Custom frame rate isn't really supported in FBX SDK (there's
            // a bug), so try hard to find the nearest time mode.
            FbxTime.EMode timeMode = FbxTime.EMode.eCustom;
            double precision = 1e-6;
            while (timeMode == FbxTime.EMode.eCustom && precision < 1000) {
                timeMode = FbxTime.ConvertFrameRateToTimeMode (30, precision);
                precision *= 10;
            }
            if (timeMode == FbxTime.EMode.eCustom) {
                timeMode = FbxTime.EMode.eFrames30;
            }
            FbxTime.SetGlobalTimeMode (timeMode);

            // set time correctly
            var fbxStartTime = FbxTime.FromSecondDouble (0);
            var fbxStopTime = FbxTime.FromSecondDouble (25);

            fbxAnimStack.SetLocalTimeSpan (new FbxTimeSpan (fbxStartTime, fbxStopTime));

            // set up the translation
            foreach(var propStr in new string[]{ "Lcl Translation", "Lcl Rotation", "Lcl Scaling" }){
                FbxProperty fbxProperty = animNode.FindProperty (propStr, false);

                Assert.IsNotNull (fbxProperty);
                Assert.IsTrue (fbxProperty.IsValid ());

                foreach (var component in new string[]{Globals.FBXSDK_CURVENODE_COMPONENT_X, 
                        Globals.FBXSDK_CURVENODE_COMPONENT_Y, 
                        Globals.FBXSDK_CURVENODE_COMPONENT_Z}) {
                    
                    // Create the AnimCurve on the channel
                    FbxAnimCurve fbxAnimCurve = fbxProperty.GetCurve (fbxAnimLayer, component, true);

                    fbxAnimCurve.KeyModifyBegin ();
                    for (int keyIndex = 0; keyIndex < m_keyCount; ++keyIndex) {
                        FbxTime fbxTime = FbxTime.FromSecondDouble(keyIndex * 2);
                        fbxAnimCurve.KeyAdd (fbxTime);
                        fbxAnimCurve.KeySet (keyIndex, fbxTime, keyIndex * 3 - 1);
                    }
                    fbxAnimCurve.KeyModifyEnd ();
                }
            }

            // TODO: avoid needing to this by creating typemaps for
            //       FbxObject::GetSrcObjectCount and FbxCast.
            //       Not trivial to do as both fbxobject.i and fbxemitter.i
            //       have to be moved up before the ignore all statement
            //       to allow use of templates.
            scene.SetCurrentAnimationStack (fbxAnimStack);
            scene.GetRootNode().AddChild (animNode);
            return scene;
        }

        protected override void CheckScene (FbxScene scene)
        {
            FbxScene origScene = CreateScene (FbxManager);

            FbxNode origAnimNode = origScene.GetRootNode ().GetChild (0);
            FbxNode importAnimNode = scene.GetRootNode ().GetChild (0);

            Assert.AreEqual (origScene.GetRootNode ().GetChildCount (), scene.GetRootNode ().GetChildCount ());
            Assert.IsNotNull (origAnimNode);
            Assert.IsNotNull (importAnimNode);
            Assert.AreEqual (origAnimNode.GetName (), importAnimNode.GetName ());

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

            foreach (var propStr in new string[]{ "Lcl Translation", "Lcl Rotation", "Lcl Scaling" }) {
                FbxProperty origProperty = origAnimNode.FindProperty (propStr, false);
                FbxProperty importProperty = importAnimNode.FindProperty (propStr, false);

                Assert.IsNotNull (origProperty);
                Assert.IsNotNull (importProperty);
                Assert.IsTrue (origProperty.IsValid ());
                Assert.IsTrue (importProperty.IsValid ());

                foreach (var component in new string[]{Globals.FBXSDK_CURVENODE_COMPONENT_X, 
                    Globals.FBXSDK_CURVENODE_COMPONENT_Y, 
                    Globals.FBXSDK_CURVENODE_COMPONENT_Z}) {

                    FbxAnimCurve origAnimCurve = origProperty.GetCurve (origLayer, component, false);
                    FbxAnimCurve importAnimCurve = importProperty.GetCurve (importLayer, component, false);

                    Assert.IsNotNull (origAnimCurve);
                    Assert.IsNotNull (importAnimCurve);

                    Assert.AreEqual (origAnimCurve.KeyGetCount (), importAnimCurve.KeyGetCount ());

                    for (int i = 0; i < origAnimCurve.KeyGetCount (); i++) {
                        Assert.AreEqual (origAnimCurve.KeyGetTime (i), importAnimCurve.KeyGetTime (i));
                        Assert.AreEqual (origAnimCurve.KeyGetValue (i), importAnimCurve.KeyGetValue (i));
                    }
                }
            }
        }
    }
}