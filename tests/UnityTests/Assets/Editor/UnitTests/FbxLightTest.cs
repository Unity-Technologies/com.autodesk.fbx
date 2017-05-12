// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using System.Collections;
using FbxSdk;

namespace UnitTests
{
    public class FbxLightTest : Base<FbxLight>
    {

        [Test]
        public void TestProperties ()
        {
            using (var fbxLight = CreateObject ("light")) {
                TestGetter (fbxLight.Color);
                TestGetter (fbxLight.DrawFrontFacingVolumetricLight);
                TestGetter (fbxLight.DrawGroundProjection);
                TestGetter (fbxLight.DrawVolumetricLight);
                TestGetter (fbxLight.FileName);
                TestGetter (fbxLight.InnerAngle);
                TestGetter (fbxLight.Intensity);
                TestGetter (fbxLight.LightType);
                TestGetter (fbxLight.OuterAngle);
            }
        }
    }
}