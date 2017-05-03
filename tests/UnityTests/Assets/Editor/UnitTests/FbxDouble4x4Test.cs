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
    /// <summary>
    /// Run some tests that any vector type should be able to pass.
    /// If you add tests here, you probably want to add them to the other
    /// FbxDouble* test classes.
    /// </summary>
    public class FbxDouble4x4Test
    {

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxDouble4x4), this.GetType()); }
#endif

        [Test]
        public void TestEquality()
        {
            var a = new FbxDouble4(1,2,3,4);
            var b = new FbxDouble4(5,6,7,8);
            var c = new FbxDouble4(9,8,7,6);
            var d = new FbxDouble4(5,4,3,2);
            EqualityTester<FbxDouble4x4>.TestEquality(new FbxDouble4x4(a, b, c, d), new FbxDouble4x4(d, c, b, a));
        }

        /// <summary>
        /// Test the basics. Subclasses should override and add some calls
        /// e.g. to excercise all the constructors.
        /// </summary>
        [Test]
        public void TestBasics()
        {
            FbxDouble4x4 v;

            // We use these later.
            var a = new FbxDouble4(1,2,3,4);
            var b = new FbxDouble4(5,6,7,8);
            var c = new FbxDouble4(9,8,7,6);
            var d = new FbxDouble4(5,4,3,2);

            // make sure the no-arg constructor doesn't crash
            new FbxDouble4x4();

            // make sure we can dispose
            using (new FbxDouble4x4()) { }
            new FbxDouble4x4().Dispose();

            // Test other constructors
            v = new FbxDouble4x4(a, b, c, d);
            var u = new FbxDouble4x4(v);
            Assert.AreEqual(v, u);
            u[0] = c;
            Assert.AreEqual(c, u[0]);
            Assert.AreEqual(a, v[0]); // check that setting u doesn't set v
            var w = new FbxDouble4x4(c);
            Assert.AreEqual(c, w[0]);
            Assert.AreEqual(c, w[1]);
            Assert.AreEqual(c, w[2]);
            Assert.AreEqual(c, w[3]);

            // Test operator[]
            v = new FbxDouble4x4();
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
            v = new FbxDouble4x4(a, b, c, d);
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
    }
}
