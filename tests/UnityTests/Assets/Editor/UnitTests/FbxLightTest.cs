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
        public void TestBasics()
        {
            using (var fbxLight = CreateObject ("light")) {
                var shadowTexture = FbxTexture.Create (Manager, "tex");
                fbxLight.SetShadowTexture (shadowTexture);
                Assert.AreEqual (shadowTexture, fbxLight.GetShadowTexture ());

                // test setting null shadow texture
                Assert.That (() => { fbxLight.SetShadowTexture(null); }, Throws.Exception.TypeOf<System.NullReferenceException>());

                // test setting invalid texture
                shadowTexture.Destroy();
                Assert.That (() => { fbxLight.SetShadowTexture(shadowTexture); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
            }
        }

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
                TestGetter (fbxLight.AreaLightShape);
                TestGetter (fbxLight.BottomBarnDoor);
                TestGetter (fbxLight.CastLight);
                TestGetter (fbxLight.CastShadows);
                TestGetter (fbxLight.DecayStart);
                TestGetter (fbxLight.DecayType);
                TestGetter (fbxLight.EnableBarnDoor);
                TestGetter (fbxLight.EnableFarAttenuation);
                TestGetter (fbxLight.EnableNearAttenuation);
                TestGetter (fbxLight.FarAttenuationEnd);
                TestGetter (fbxLight.FarAttenuationStart);
                TestGetter (fbxLight.Fog);
                TestGetter (fbxLight.LeftBarnDoor);
                TestGetter (fbxLight.NearAttenuationEnd);
                TestGetter (fbxLight.NearAttenuationStart);
                TestGetter (fbxLight.RightBarnDoor);
                TestGetter (fbxLight.ShadowColor);
                TestGetter (fbxLight.TopBarnDoor);
            }
        }
    }
}