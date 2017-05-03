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
    public class FbxMatrixTest
    {
#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxMatrix), this.GetType()); }
#endif

        [Test]
        public void TestEquality()
        {
            var zero = new FbxVector4();
            var one = new FbxVector4(1,1,1);
            var mx1 = new FbxMatrix(zero, zero, one);
            var mx2 = new FbxMatrix(one, zero, one);
            EqualityTester<FbxMatrix>.TestEquality(mx1, mx2);
        }

        [Test]
        public void BasicTests ()
        {
            FbxMatrix mx;

            // make sure the constructors compile and don't crash
            mx = new FbxMatrix();
            mx = new FbxMatrix(new FbxMatrix());
            mx = new FbxMatrix(new FbxAMatrix());
            mx = new FbxMatrix(new FbxVector4(), new FbxVector4(), new FbxVector4(1,1,1));
            mx = new FbxMatrix(0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15);

            /* Check the values we typed in match up. */
            for(int y = 0; y < 4; ++y) {
                for(int x = 0; x < 4; ++x) {
                    Assert.AreEqual(x + 4 * y, mx.Get(y, x));
                }
            }

            /* Check that set and get work (silly transpose operation). */
            FbxMatrix mx2 = new FbxMatrix();
            for(int y = 0; y < 4; ++y) {
                for(int x = 0; x < 4; ++x) {
                    mx2.Set(y, x, y + 4 * x);
                    Assert.AreEqual(mx.Get(x, y), mx2.Get(y, x));
                }
            }

            //////
            // Tests for the inherited Double4x4

            // make sure the no-arg constructor doesn't crash
            new FbxMatrix();

            // make sure we can dispose
            using (new FbxMatrix()) { }
            new FbxMatrix().Dispose();

            // make sure equality works.
            Assert.IsTrue(new FbxMatrix().Equals(new FbxMatrix()));

            Assert.IsTrue(new FbxMatrix() == new FbxMatrix());
            Assert.IsFalse(new FbxMatrix() != new FbxMatrix());

            Assert.IsFalse(new FbxMatrix() == (FbxMatrix)null);
            Assert.IsTrue(new FbxMatrix() != (FbxMatrix)null);

            Assert.IsFalse((FbxMatrix)null == new FbxMatrix());
            Assert.IsTrue((FbxMatrix)null != new FbxMatrix());

            Assert.IsTrue((FbxMatrix)null == (FbxMatrix)null);
            Assert.IsFalse((FbxMatrix)null != (FbxMatrix)null);

            // Test operator[]
            var v = new FbxMatrix();
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

            // Test 4-argument constructor and members W/X/Y/Z
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

            // Test getting the elements from a matrix built by TRS
            mx = new FbxMatrix(new FbxVector4(1,2,3), new FbxVector4(0,90,0), new FbxVector4(1,1,1));
            FbxVector4 t,r,s, shear;
            double sign;
            mx.GetElements(out t, out r, out shear, out s, out sign);
            Assert.AreEqual(1, sign);
            Assert.AreEqual(new FbxVector4(1,2,3, 1), t);
            Assert.AreEqual(new FbxVector4(0,90,0, 0), r); /* for some reason w is zero for rotation */
            Assert.AreEqual(new FbxVector4(1,1,1, 0), s); /* and similarly for scaling */
        }
    }
}
