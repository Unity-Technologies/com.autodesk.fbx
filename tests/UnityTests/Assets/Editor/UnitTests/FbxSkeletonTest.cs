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
    public class FbxSkeletonTest : Base<FbxSkeleton>
    {

        [Test]
        public void TestSetSkeletonType ()
        {
            var skeleton = CreateObject ("skeleton");
            skeleton.SetSkeletonType(FbxSkeleton.EType.eLimb);
            Assert.AreEqual (FbxSkeleton.EType.eLimb, skeleton.GetSkeletonType ());

            TestGetter (skeleton.Size);
        }
    }
}
