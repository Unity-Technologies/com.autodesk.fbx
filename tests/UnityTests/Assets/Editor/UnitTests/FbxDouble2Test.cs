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
    public class FbxDouble2Test
    {

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxDouble2), this.GetType()); }
#endif

        /// <summary>
        /// Test the basics. Subclasses should override and add some calls
        /// e.g. to excercise all the constructors.
        /// </summary>
        [Test]
        public void TestBasics()
        {
            // make sure the no-arg constructor doesn't crash
            new FbxDouble2();

            // make sure we can dispose
            using (new FbxDouble2()) { }
            new FbxDouble2().Dispose();

            // make sure equality works.
            Assert.IsTrue(new FbxDouble2().Equals(new FbxDouble2()));

            Assert.IsTrue(new FbxDouble2() == new FbxDouble2());
            Assert.IsFalse(new FbxDouble2() != new FbxDouble2());

            Assert.IsFalse(new FbxDouble2() == (FbxDouble2)null);
            Assert.IsTrue(new FbxDouble2() != (FbxDouble2)null);

            Assert.IsFalse((FbxDouble2)null == new FbxDouble2());
            Assert.IsTrue((FbxDouble2)null != new FbxDouble2());

            Assert.IsTrue((FbxDouble2)null == (FbxDouble2)null);
            Assert.IsFalse((FbxDouble2)null != (FbxDouble2)null);

            Assert.IsTrue(new FbxDouble2(1,2) == new FbxDouble2(1,2));
            Assert.IsFalse(new FbxDouble2(1,2) != new FbxDouble2(1,2));

            Assert.IsFalse(new FbxDouble2(1,0) == new FbxDouble2(1,2));
            Assert.IsTrue(new FbxDouble2(1,0) != new FbxDouble2(1,2));

            // Test operator[]
            var v = new FbxDouble2();
            v[0] = 1;
            Assert.AreEqual(1, v[0]);
            v[1] = 2;
            Assert.AreEqual(2, v[1]);
            Assert.That(() => v[-1], Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[ 2], Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[-1] = 5, Throws.Exception.TypeOf<System.IndexOutOfRangeException>());
            Assert.That(() => v[ 2] = 5, Throws.Exception.TypeOf<System.IndexOutOfRangeException>());

            // Test 2-argument constructor and members X/Y
            v = new FbxDouble2(1, 2);
            Assert.AreEqual(1, v.X);
            Assert.AreEqual(2, v.Y);
            v.X = 3;
            v.Y = 4;
            Assert.AreEqual(3, v.X);
            Assert.AreEqual(4, v.Y);
        }
    }
}
