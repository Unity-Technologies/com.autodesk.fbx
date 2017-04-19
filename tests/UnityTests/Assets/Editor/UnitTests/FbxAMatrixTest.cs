// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using FbxSdk;

namespace UnitTests
{
    public class FbxAMatrixTest
    {
        [Test]
        public void BasicTests ()
        {
            FbxAMatrix mx;

            // make sure the constructors compile and don't crash
            mx = new FbxAMatrix();
            mx = new FbxAMatrix(new FbxAMatrix());
            mx = new FbxAMatrix(new FbxVector4(), new FbxVector4(), new FbxVector4(1,1,1));

            // check that the matrix is the id matrix */
            for(int y = 0; y < 4; ++y) {
                for(int x = 0; x < 4; ++x) {
                    Assert.AreEqual(x == y ? 1 : 0, mx.Get(y, x));
                }
            }
        }

        [Test]
        public void TestUsing ()
        {
            /* make sure that the using form compiles and doesn't crash */
            using (new FbxAMatrix()) { }
        }
    }
}
