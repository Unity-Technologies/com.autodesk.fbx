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
    public class FbxLayerContainerTest : Base<FbxLayerContainer>
    {

        [Test]
        public void TestCreateLayer()
        {
            using (FbxLayerContainer layerContainer = CreateObject ("layerContainer")) {
                int index = layerContainer.CreateLayer ();
                Assert.GreaterOrEqual (index, -1); // check an index is returned (-1 is error)
            }
        }

        [Test]
        public void TestGetLayer()
        {
            using (FbxLayerContainer layerContainer = CreateObject ("layerContainer")) {
                int index = layerContainer.CreateLayer ();
                Assert.GreaterOrEqual (index, 0); // check we created a valid layer

                // make sure doesn't crash and returns expected value
                Assert.IsNotNull (layerContainer.GetLayer (index));
                Assert.IsNull (layerContainer.GetLayer (int.MinValue));
                Assert.IsNull (layerContainer.GetLayer (int.MaxValue));
            }
        }
    }
}
