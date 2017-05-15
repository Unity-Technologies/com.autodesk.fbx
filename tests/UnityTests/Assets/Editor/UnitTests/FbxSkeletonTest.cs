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
    public class FbxSkeletonTest : FbxNodeAttributeBase<FbxSkeleton>
    {
        [Test]
        public void TestBasics ()
        {
            var skeleton = CreateObject ("skeleton");
            base.TestBasics(skeleton, FbxNodeAttribute.EType.eSkeleton);

            skeleton.SetSkeletonType(FbxSkeleton.EType.eLimb);
            Assert.AreEqual (FbxSkeleton.EType.eLimb, skeleton.GetSkeletonType ());

            TestGetter (skeleton.Size);
        }
    }
}
