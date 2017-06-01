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
            foreach(var fbxProperty in new FbxProperty[]{ animNode.FindProperty ("Lcl Translation", false),
                animNode.FindProperty ("Lcl Rotation", false),
                animNode.FindProperty ("Lcl Scaling", false) }){

                Assert.IsNotNull (fbxProperty);
                Assert.IsTrue (fbxProperty.IsValid ());

                foreach (var component in new string[]{Globals.FBXSDK_CURVENODE_COMPONENT_X, 
                        Globals.FBXSDK_CURVENODE_COMPONENT_Y, 
                        Globals.FBXSDK_CURVENODE_COMPONENT_Z}) {
                    
                    // Create the AnimCurve on the channel
                    FbxAnimCurve fbxAnimCurve = fbxProperty.GetCurve (fbxAnimLayer, component, true);

                    fbxAnimCurve.KeyModifyBegin ();
                    for (int keyIndex = 0; keyIndex < 5; ++keyIndex) {
                        FbxTime fbxTime = FbxTime.FromSecondDouble(keyIndex * 2);
                        fbxAnimCurve.KeyAdd (fbxTime);
                        fbxAnimCurve.KeySet (keyIndex, fbxTime, keyIndex * 3 - 1);
                    }
                    fbxAnimCurve.KeyModifyEnd ();
                }
            }
            scene.GetRootNode().AddChild (animNode);
            return scene;
        }

        protected override void CheckScene (FbxScene scene)
        {}
    }
}