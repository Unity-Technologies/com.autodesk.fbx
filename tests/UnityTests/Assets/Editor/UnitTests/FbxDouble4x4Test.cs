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

        /// <summary>
        /// Test the basics. Subclasses should override and add some calls
        /// e.g. to excercise all the constructors.
        /// </summary>
        [Test]
        public void TestBasics()
        {
            // make sure the no-arg constructor doesn't crash
            new FbxDouble4x4();

            // make sure we can dispose
            using (new FbxDouble4x4()) { }
            new FbxDouble4x4().Dispose();

            // make sure equality works.
            Assert.IsTrue(new FbxDouble4x4().Equals(new FbxDouble4x4()));

            Assert.IsTrue(new FbxDouble4x4() == new FbxDouble4x4());
            Assert.IsFalse(new FbxDouble4x4() != new FbxDouble4x4());

            Assert.IsFalse(new FbxDouble4x4() == (FbxDouble4x4)null);
            Assert.IsTrue(new FbxDouble4x4() != (FbxDouble4x4)null);

            Assert.IsFalse((FbxDouble4x4)null == new FbxDouble4x4());
            Assert.IsTrue((FbxDouble4x4)null != new FbxDouble4x4());

            Assert.IsTrue((FbxDouble4x4)null == (FbxDouble4x4)null);
            Assert.IsFalse((FbxDouble4x4)null != (FbxDouble4x4)null);

            // Test operator[]
            var v = new FbxDouble4x4();
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
