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
    public class FbxBlendShapeChannelTest : Base<FbxBlendShapeChannel>
    {
        [Test]
        public void TestGetDeformPercent ()
        {
            using (var blendShapeChannel = CreateObject ()) {
                TestGetter (blendShapeChannel.DeformPercent);
            }
        }

        [Test]
        public void TestAddTargetShape ()
        {
            using (var blendShapeChannel = CreateObject ()) {
                int origCount = blendShapeChannel.GetTargetShapeCount ();

                FbxShape shape = FbxShape.Create (Manager, "shape");
                blendShapeChannel.AddTargetShape (shape);

                Assert.AreEqual (origCount + 1, blendShapeChannel.GetTargetShapeCount ());
                Assert.AreEqual (shape, blendShapeChannel.GetTargetShape (origCount));

                // test AddTargetShape with double doesn't crash
                blendShapeChannel.AddTargetShape (shape, 45);

                // test null
                Assert.That (() => { blendShapeChannel.AddTargetShape (null); }, Throws.Exception.TypeOf<System.NullReferenceException>());

                // test destroyed
                shape.Destroy();
                Assert.That (() => { blendShapeChannel.AddTargetShape (shape); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
            }
        }
    }
}