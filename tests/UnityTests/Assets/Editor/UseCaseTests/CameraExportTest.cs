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
    public class CameraExportTest : RoundTripTestBase
    {
        [SetUp]
        public override void Init ()
        {
            fileNamePrefix = "_safe_to_delete__camera_export_test";
            base.Init ();
        }

        protected override FbxScene CreateScene (FbxManager manager)
        {
            FbxScene scene = FbxScene.Create (manager, "myScene");
            FbxNode cameraNode = FbxNode.Create (scene, "cameraNode");
            FbxCamera camera = FbxCamera.Create (scene, "camera");

            camera.ProjectionType.Set (FbxCamera.EProjectionType.ePerspective);
            camera.SetAspect (FbxCamera.EAspectRatioMode.eFixedRatio, 300, 400);
            camera.FilmAspectRatio.Set (240);
            camera.SetApertureWidth (4);
            camera.SetApertureHeight (2);
            camera.SetApertureMode (FbxCamera.EApertureMode.eFocalLength);
            camera.FocalLength.Set (32);
            camera.SetNearPlane (1);
            camera.SetFarPlane (100);

            // set background color
            var bgColorProperty = FbxProperty.Create (cameraNode, Globals.FbxColor4DT, "backgroundColor");
            Assert.IsTrue (bgColorProperty.IsValid ());

            bgColorProperty.Set (new FbxColor(0.5, 0.4, 0.1, 1));

            // Must be marked user-defined or it won't be shown in most DCCs
            bgColorProperty.ModifyFlag (FbxPropertyFlags.EFlags.eUserDefined, true);
            bgColorProperty.ModifyFlag (FbxPropertyFlags.EFlags.eAnimatable, true);

            // set clear flags
            var clearFlagsProperty = FbxProperty.Create (cameraNode, Globals.FbxIntDT, "clearFlags");
            Assert.IsTrue (clearFlagsProperty.IsValid ());

            clearFlagsProperty.Set (4);

            // Must be marked user-defined or it won't be shown in most DCCs
            clearFlagsProperty.ModifyFlag (FbxPropertyFlags.EFlags.eUserDefined, true);
            clearFlagsProperty.ModifyFlag (FbxPropertyFlags.EFlags.eAnimatable, true);

            // Add an animation clip

            cameraNode.SetNodeAttribute (camera);
            scene.GetRootNode ().AddChild (cameraNode);

            // set the default camera
            scene.GetGlobalSettings ().SetDefaultCamera (cameraNode.GetName());

            return scene;
        }

        protected override void CheckScene (FbxScene scene)
        {
            
        }
    }
}