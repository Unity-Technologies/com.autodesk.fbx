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

        static FbxDataTypeTest() { EqualityTester<FbxDataType>.RegisterCoverage(); }
#endif

        [Test]
        public void TestEquality()
        {
            // Left here in case we add equality operations back in.
            // For now, equality is just reference equality.
            EqualityTester<FbxDataType>.TestEquality(Globals.FbxBoolDT, Globals.FbxFloatDT);
        }

        [Test]
        public void BasicTests ()
        {
            FbxDataType v, v2;

            // try all the constructors; make sure they don't crash
            new FbxDataType();
            v = Globals.FbxBoolDT;
            v2 = new FbxDataType(v);

            Assert.AreEqual("Byte", v.GetNameForIO()); // bool is serialized as a byte

            // make sure disposing doesn't crash in either case (disposing a handle to a
            // global, or disposing a handle to a copy)
            v.Dispose();
            v2.Dispose();
        }
    }
}
