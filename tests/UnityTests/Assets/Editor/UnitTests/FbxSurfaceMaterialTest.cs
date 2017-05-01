// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using FbxSdk;
using System.Collections.Generic;

namespace UnitTests
{
    public class FbxSurfaceMaterialTest : Base<FbxSurfaceMaterial>
    {
        public static void TestSurface<T>(T material) where T:FbxSurfaceMaterial
        {
            material.ShadingModel.Get();
            material.MultiLayer.Get();
        }

        public static void TestGetter<T>(T item) { /* we tested the getter by passing the argument! */ }

        [Test]
        public void TestBasics()
        {
            using (var surface = CreateObject()) { TestSurface(surface); }

            // Use all the getters
            TestGetter(FbxSurfaceMaterial.sShadingModel);
            TestGetter(FbxSurfaceMaterial.sMultiLayer);
            TestGetter(FbxSurfaceMaterial.sMultiLayerDefault);
            TestGetter(FbxSurfaceMaterial.sEmissive);
            TestGetter(FbxSurfaceMaterial.sEmissiveFactor);
            TestGetter(FbxSurfaceMaterial.sAmbient);
            TestGetter(FbxSurfaceMaterial.sAmbientFactor);
            TestGetter(FbxSurfaceMaterial.sDiffuse);
            TestGetter(FbxSurfaceMaterial.sDiffuseFactor);
            TestGetter(FbxSurfaceMaterial.sSpecular);
            TestGetter(FbxSurfaceMaterial.sSpecularFactor);
            TestGetter(FbxSurfaceMaterial.sShininess);
            TestGetter(FbxSurfaceMaterial.sBump);
            TestGetter(FbxSurfaceMaterial.sNormalMap);
            TestGetter(FbxSurfaceMaterial.sBumpFactor);
            TestGetter(FbxSurfaceMaterial.sTransparentColor);
            TestGetter(FbxSurfaceMaterial.sTransparencyFactor);
            TestGetter(FbxSurfaceMaterial.sReflection);
            TestGetter(FbxSurfaceMaterial.sReflectionFactor);
            TestGetter(FbxSurfaceMaterial.sDisplacementColor);
            TestGetter(FbxSurfaceMaterial.sDisplacementFactor);
            TestGetter(FbxSurfaceMaterial.sVectorDisplacementColor);
            TestGetter(FbxSurfaceMaterial.sVectorDisplacementFactor);
            TestGetter(FbxSurfaceMaterial.sShadingModelDefault);
        }
    }

    public class FbxSurfaceLambertTest : Base<FbxSurfaceLambert>
    {
        public static void TestLambert<T>(T lambert) where T:FbxSurfaceLambert
        {
            FbxSurfaceMaterialTest.TestSurface(lambert);
            lambert.Emissive.Get();
            lambert.EmissiveFactor.Get();
            lambert.Ambient.Get();
            lambert.AmbientFactor.Get();
            lambert.Diffuse.Get();
            lambert.DiffuseFactor.Get();
            lambert.NormalMap.Get();
            lambert.Bump.Get();
            lambert.BumpFactor.Get();
            lambert.TransparentColor.Get();
            lambert.TransparencyFactor.Get();
            lambert.DisplacementColor.Get();
            lambert.DisplacementFactor.Get();
            lambert.VectorDisplacementColor.Get();
            lambert.VectorDisplacementFactor.Get();
        }

        [Test]
        public void TestBasics()
        {
            using (var lambert = CreateObject()) { TestLambert(lambert); }
        }
    }

    public class FbxSurfacePhongTest : Base<FbxSurfacePhong>
    {
        [Test]
        public void TestBasics()
        {
            using (var phong = CreateObject()) {
                FbxSurfaceLambertTest.TestLambert(phong);
                phong.Specular.Get();
                phong.SpecularFactor.Get();
                phong.Shininess.Get();
                phong.Reflection.Get();
                phong.ReflectionFactor.Get();
            }
        }
    }
}
