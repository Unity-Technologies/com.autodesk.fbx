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
    public class FbxVector4Test
    {
#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxVector4), this.GetType()); }
#endif

        [Test]
        public void TestEquality()
        {
            EqualityTester<FbxVector4>.TestEquality(
                    new FbxVector4(0, 1, 2, 3),
                    new FbxVector4(3, 2, 1, 0),
                    new FbxVector4(0, 1, 2, 3));
        }

        [Test]
        public void BasicTests ()
        {
            FbxVector4 v;

            // make sure the no-arg constructor doesn't crash
            new FbxVector4();

            // Test other constructors
            v = new FbxVector4(1, 2, 3, 4);
            var u = new FbxVector4(v);
            Assert.AreEqual(v, u);
            u[0] = 5;
            Assert.AreEqual(5, u[0]);
            Assert.AreEqual(1, v[0]); // check that setting u doesn't set v

            v = new FbxVector4(1, 2, 3);
            Assert.AreEqual(1, v[3]); // w is assumed to be a homogenous coordinate
            v = new FbxVector4(new FbxDouble3(1, 2, 3));
            Assert.AreEqual(1, v[3]); // w is assumed to be a homogenous coordinate
            Assert.AreEqual(1, v[0]);
            Assert.AreEqual(2, v[1]);
            Assert.AreEqual(3, v[2]);

            // Test operator[]
            v = new FbxVector4();
            v[0] = 1;
            Assert.AreEqual(1, v[0]);
            v[1] = 2;
            Assert.AreEqual(2, v[1]);
            v[2] = 3;
            Assert.AreEqual(3, v[2]);
            v[3] = 4;
            Assert.AreEqual(4, v[3]);
            Assert.That(() => v[-1], Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[ 4], Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[-1] = 5, Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[ 4] = 5, Throws.Exception.TypeOf<System.IndexOutOfRangeException>());

            // Test 4-argument constructor and members X/Y/Z/W
            v = new FbxVector4(1, 2, 3, 4);
            Assert.AreEqual(1, v.X);
            Assert.AreEqual(2, v.Y);
            Assert.AreEqual(3, v.Z);
            Assert.AreEqual(4, v.W);
            v.X = 3;
            v.Y = 4;
            v.Z = 5;
            v.W = 6;
            Assert.AreEqual(3, v.X);
            Assert.AreEqual(4, v.Y);
            Assert.AreEqual(5, v.Z);
            Assert.AreEqual(6, v.W);
        }
    }
}
