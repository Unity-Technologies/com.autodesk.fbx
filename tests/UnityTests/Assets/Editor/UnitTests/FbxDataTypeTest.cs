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
    public class FbxDataTypeTest
    {
#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxDataType), this.GetType()); }
#endif

        [Test]
        public void TestEquality()
        {
            // Left here in case we add equality operations back in.
            // For now, equality is just reference equality.
            EqualityTester<FbxDataType>.TestEquality(Globals.FbxBoolDT, Globals.FbxFloatDT, Globals.FbxBoolDT);
        }

        [Test]
        public void BasicTests ()
        {
            // Try all the constructors; make sure they don't crash
            new FbxDataType();
            var v = Globals.FbxBoolDT;
            var v2 = new FbxDataType(v);

            // Call the basic functions, make sure they're reasonable.
            Assert.IsTrue(v.Valid());
            Assert.AreEqual(EFbxType.eFbxBool, v.ToEnum());
            Assert.AreEqual("Bool", v.GetName());
            Assert.AreEqual("bool", v.GetNameForIO());
            Assert.IsTrue(v.Is(v2));

            using(new FbxDataType(EFbxType.eFbxFloat));
            using(new FbxDataType("name", EFbxType.eFbxFloat));
            using(new FbxDataType("name", v));

            // make sure disposing doesn't crash in either case (disposing a handle to a
            // global, or disposing a handle to a copy)
            v.Dispose();
            v2.Dispose();
        }
    }
}
