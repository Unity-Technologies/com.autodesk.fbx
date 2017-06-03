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
    public class FbxMatrixTest : FbxDouble4x4TestBase<FbxMatrix>
    {
        [Test]
        public void TestEquality()
        {
            var zero = new FbxVector4();
            var one = new FbxVector4(1,1,1);
            var mx1 = new FbxMatrix(zero, zero, one);
            var mx2 = new FbxMatrix(one, zero, one);
            var mx1copy = new FbxMatrix(zero, zero, one);
            EqualityTester<FbxMatrix>.TestEquality(mx1, mx2, mx1copy);
        }

        [Test]
        public void BasicTests ()
        {
            base.TestElementAccessAndDispose(new FbxMatrix());

            FbxMatrix mx;

            // make sure the constructors compile and don't crash
            mx = new FbxMatrix();
            mx = new FbxMatrix(new FbxMatrix());
            mx = new FbxMatrix(new FbxAMatrix());
            mx = new FbxMatrix(new FbxVector4(), new FbxVector4(), new FbxVector4(1,1,1));
            mx = new FbxMatrix(new FbxVector4(), new FbxQuaternion(), new FbxVector4(1,1,1));
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

            /* normal transpose operation */
            Assert.AreEqual(mx, mx2.Transpose());

            // Test getting the elements from a matrix built by TRS
            mx = new FbxMatrix(new FbxVector4(1,2,3), new FbxVector4(0,90,0), new FbxVector4(1,1,1));
            FbxVector4 t,r,s, shear;
            double sign;
            mx.GetElements(out t, out r, out shear, out s, out sign);
            Assert.AreEqual(1, sign);
            Assert.AreEqual(new FbxVector4(1,2,3, 1), t);
            Assert.AreEqual(new FbxVector4(0,90,0, 0), r); /* for some reason w is zero for rotation */
            Assert.AreEqual(new FbxVector4(1,1,1, 0), s); /* and similarly for scaling */

            // Test set column + set row
            mx = new FbxMatrix();
            mx.SetColumn (1, new FbxVector4 (1, 2, 3, 4));
            mx.SetRow (2, new FbxVector4 (5, 6, 7, 8));
            //check that the column is what we expect
            Assert.AreEqual (1, mx.Get (0, 1));
            Assert.AreEqual (2, mx.Get (1, 1));
            Assert.AreEqual (6, mx.Get (2, 1)); // this value got changed by SetRow
            Assert.AreEqual (4, mx.Get (3, 1));
            // check that the row is what we expect
            Assert.AreEqual (new FbxDouble4 (5, 6, 7, 8), mx [2]);
        }
    }
}
