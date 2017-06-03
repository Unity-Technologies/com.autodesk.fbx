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
    public class FbxAMatrixTest : FbxDouble4x4TestBase<FbxAMatrix>
    {
        [Test]
        public void TestEquality()
        {
            var zero = new FbxVector4();
            var one = new FbxVector4(1,1,1);
            var mx1 = new FbxAMatrix(zero, zero, one);
            var mx2 = new FbxAMatrix(one, zero, one);
            var mx1copy = new FbxAMatrix(zero, zero, one);
            EqualityTester<FbxAMatrix>.TestEquality(mx1, mx2, mx1copy);
        }

        [Test]
        public void BasicTests ()
        {
            base.TestElementAccessAndDispose(new FbxAMatrix());

            // make sure the constructors compile and don't crash
            new FbxAMatrix();
            new FbxAMatrix(new FbxAMatrix());
            var mx = new FbxAMatrix(new FbxVector4(), new FbxVector4(), new FbxVector4(1,1,1));

            // check that the matrix is the id matrix
            Assert.IsTrue(mx.IsIdentity());
            for(int y = 0; y < 4; ++y) {
                for(int x = 0; x < 4; ++x) {
                    Assert.AreEqual(x == y ? 1 : 0, mx.Get(y, x));
                }
            }
        }
    }
}
