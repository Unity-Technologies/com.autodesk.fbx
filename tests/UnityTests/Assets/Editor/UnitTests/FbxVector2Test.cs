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
    public class FbxVector2Test
    {
#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxVector2), this.GetType()); }
#endif

        [Test]
        public void TestEquality()
        {
            EqualityTester<FbxVector2>.TestEquality(
                    new FbxVector2(0, 1),
                    new FbxVector2(3, 2),
                    new FbxVector2(0, 1));
        }

        [Test]
        public void BasicTests ()
        {
            FbxVector2 v;

            // make sure the no-arg constructor doesn't crash
            new FbxVector2();

            // Test other constructors
            v = new FbxVector2(5);
            Assert.AreEqual(5, v.X);
            Assert.AreEqual(5, v.Y);

            v = new FbxVector2(1, 2);
            var u = new FbxVector2(v);
            Assert.AreEqual(v, u);
            u[0] = 5;
            Assert.AreEqual(5, u[0]);
            Assert.AreEqual(1, v[0]); // check that setting u doesn't set v
            Assert.AreEqual(1, v.X);
            Assert.AreEqual(2, v.Y);

            var d2 = new FbxDouble2(5, 6);
            v = new FbxVector2(d2);
            Assert.AreEqual(5, v.X);
            Assert.AreEqual(6, v.Y);

            // Test operator[]
            v = new FbxVector2();
            v[0] = 1;
            Assert.AreEqual(1, v[0]);
            v[1] = 2;
            Assert.AreEqual(2, v[1]);
            Assert.That(() => v[-1], Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[ 2], Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[-1] = 5, Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[ 2] = 5, Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
        }
    }
}
