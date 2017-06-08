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
    public class FbxBlendShapeTest : Base<FbxBlendShape>
    {

        [Test]
        public void TestAddBlendShapeChannel ()
        {
            using (var fbxBlendShape = CreateObject ()) {
                int origCount = fbxBlendShape.GetBlendShapeChannelCount ();

                var fbxBlendShapeChannel = FbxBlendShapeChannel.Create (Manager, "blendShapeChannel");
                fbxBlendShape.AddBlendShapeChannel (fbxBlendShapeChannel);

                Assert.AreEqual (origCount+1, fbxBlendShape.GetBlendShapeChannelCount ());
                Assert.AreEqual (fbxBlendShapeChannel, fbxBlendShape.GetBlendShapeChannel (origCount));

                // test null
                Assert.That (() => { fbxBlendShape.AddBlendShapeChannel (null); }, Throws.Exception.TypeOf<System.NullReferenceException>());

                // test destroyed
                fbxBlendShapeChannel.Destroy();
                Assert.That (() => { fbxBlendShape.AddBlendShapeChannel (fbxBlendShapeChannel); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
            }
        }
    }
}