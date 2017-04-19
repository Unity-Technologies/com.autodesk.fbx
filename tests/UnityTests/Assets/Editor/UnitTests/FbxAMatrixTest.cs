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
            // make sure the constructors compile and don't crash
            new FbxAMatrix();
            new FbxAMatrix(new FbxAMatrix());
            var mx = new FbxAMatrix(new FbxVector4(), new FbxVector4(), new FbxVector4(1,1,1));

            // check that the matrix is the id matrix */
            for(int y = 0; y < 4; ++y) {
                for(int x = 0; x < 4; ++x) {
                    Assert.AreEqual(x == y ? 1 : 0, mx.Get(y, x));
                }
            }

            //////
            // Tests for the inherited Double4x4

            // make sure the no-arg constructor doesn't crash
            new FbxAMatrix();

            // make sure we can dispose
            using (new FbxAMatrix()) { }
            new FbxAMatrix().Dispose();

            // make sure equality works.
            Assert.IsTrue(new FbxAMatrix().Equals(new FbxAMatrix()));

            Assert.IsTrue(new FbxAMatrix() == new FbxAMatrix());
            Assert.IsFalse(new FbxAMatrix() != new FbxAMatrix());

            Assert.IsFalse(new FbxAMatrix() == (FbxAMatrix)null);
            Assert.IsTrue(new FbxAMatrix() != (FbxAMatrix)null);

            Assert.IsFalse((FbxAMatrix)null == new FbxAMatrix());
            Assert.IsTrue((FbxAMatrix)null != new FbxAMatrix());

            Assert.IsTrue((FbxAMatrix)null == (FbxAMatrix)null);
            Assert.IsFalse((FbxAMatrix)null != (FbxAMatrix)null);

            // Test operator[]
            var v = new FbxAMatrix();
            var a = new FbxDouble4(1,2,3,4);
            var b = new FbxDouble4(5,6,7,8);
            var c = new FbxDouble4(9,8,7,6);
            var d = new FbxDouble4(5,4,3,2);
            v[0] = a;
            Assert.AreEqual(a.X, v[0].X);
            Assert.AreEqual(a.Y, v[0].Y);
            Assert.AreEqual(a.Z, v[0].Z);
            Assert.AreEqual(a.W, v[0].W);
            Assert.AreEqual(a, v[0]);
            v[1] = b;
            Assert.AreEqual(b, v[1]);
            v[2] = c;
            Assert.AreEqual(c, v[2]);
            v[3] = d;
            Assert.AreEqual(d, v[3]);
            Assert.That(() => v[-1], Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[ 4], Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[-1] = a, Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[ 4] = a, Throws.Exception.TypeOf<System.IndexOutOfRangeException>());

            // Test members W/X/Y/Z
            Assert.AreEqual(a, v.X);
            Assert.AreEqual(b, v.Y);
            Assert.AreEqual(c, v.Z);
            Assert.AreEqual(d, v.W);
            v.X = d;
            v.Y = c;
            v.Z = b;
            v.W = a;
            Assert.AreEqual(d, v.X);
            Assert.AreEqual(c, v.Y);
            Assert.AreEqual(b, v.Z);
            Assert.AreEqual(a, v.W);
        }

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxAMatrix), this.GetType()); }
#endif
    }
}
