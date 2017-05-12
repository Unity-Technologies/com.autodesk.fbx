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
    public class FbxMarkerTest : Base<FbxMarker>
    {
        [Test]
        public void TestBasics ()
        {
            var marker = CreateObject ("marker");
            
            var defaultType = marker.GetType();
            
            marker.SetType(FbxMarker.EType.eStandard);
            Assert.AreEqual (FbxMarker.EType.eStandard, marker.GetType ());

            TestGetter (marker.Size);
            TestGetter (marker.ShowLabel);
            TestGetter (marker.Look);
            TestGetter (marker.IKPivot);
            TestGetter (marker.DrawLink);
            
            marker.Reset();
            Assert.AreEqual (defaultType, marker.GetType ());
        }
    }
}
