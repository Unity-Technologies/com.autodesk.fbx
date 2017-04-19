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
    public class FbxDouble3Test
    {

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxDouble3), this.GetType()); }
#endif

        /// <summary>
        /// Test the basics. Subclasses should override and add some calls
        /// e.g. to excercise all the constructors.
        /// </summary>
        [Test]
        public void TestBasics()
        {
            // make sure the no-arg constructor doesn't crash
            new FbxDouble3();

            // make sure we can dispose
            using (new FbxDouble3()) { }
            new FbxDouble3().Dispose();

            // make sure equality works.
            Assert.IsTrue(new FbxDouble3().Equals(new FbxDouble3()));

            Assert.IsTrue(new FbxDouble3() == new FbxDouble3());
            Assert.IsFalse(new FbxDouble3() != new FbxDouble3());

            Assert.IsFalse(new FbxDouble3() == (FbxDouble3)null);
            Assert.IsTrue(new FbxDouble3() != (FbxDouble3)null);

            Assert.IsFalse((FbxDouble3)null == new FbxDouble3());
            Assert.IsTrue((FbxDouble3)null != new FbxDouble3());

            Assert.IsTrue((FbxDouble3)null == (FbxDouble3)null);
            Assert.IsFalse((FbxDouble3)null != (FbxDouble3)null);

            Assert.IsTrue(new FbxDouble3(1,2,3) == new FbxDouble3(1,2,3));
            Assert.IsFalse(new FbxDouble3(1,2,3) != new FbxDouble3(1,2,3));

            Assert.IsFalse(new FbxDouble3(1,2,0) == new FbxDouble3(1,2,3));
            Assert.IsTrue(new FbxDouble3(1,2,0) != new FbxDouble3(1,2,3));

            // Test operator[]
            var v = new FbxDouble3();
            v[0] = 1;
            Assert.AreEqual(1, v[0]);
            v[1] = 2;
            Assert.AreEqual(2, v[1]);
            v[2] = 3;
            Assert.AreEqual(3, v[2]);
            Assert.That(() => v[-1], Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[ 3], Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[-1] = 5, Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[ 3] = 5, Throws.Exception.TypeOf<System.IndexOutOfRangeException>());

            // Test 3-argument constructor and members X/Y/Z
            v = new FbxDouble3(1, 2, 3);
            Assert.AreEqual(1, v.X);
            Assert.AreEqual(2, v.Y);
            Assert.AreEqual(3, v.Z);
            v.X = 3;
            v.Y = 4;
            v.Z = 5;
            Assert.AreEqual(3, v.X);
            Assert.AreEqual(4, v.Y);
            Assert.AreEqual(5, v.Z);
        }
    }
}
