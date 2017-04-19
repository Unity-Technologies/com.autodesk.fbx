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
        [Test]
        public void BasicTests ()
        {
            // make sure the no-arg constructor doesn't crash
            new FbxVector4();

            // make sure we can dispose
            using (new FbxVector4()) { }
            new FbxVector4().Dispose();

            // make sure equality works.
            Assert.IsTrue(new FbxVector4().Equals(new FbxVector4()));

            Assert.IsTrue(new FbxVector4() == new FbxVector4());
            Assert.IsFalse(new FbxVector4() != new FbxVector4());

            Assert.IsFalse(new FbxVector4() == (FbxVector4)null);
            Assert.IsTrue(new FbxVector4() != (FbxVector4)null);

            Assert.IsFalse((FbxVector4)null == new FbxVector4());
            Assert.IsTrue((FbxVector4)null != new FbxVector4());

            Assert.IsTrue((FbxVector4)null == (FbxVector4)null);
            Assert.IsFalse((FbxVector4)null != (FbxVector4)null);

            // Test operator[]
            var v = new FbxVector4();
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

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxVector4), this.GetType()); }
#endif
    }
}
