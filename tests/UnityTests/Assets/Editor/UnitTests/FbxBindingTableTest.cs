// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using FbxSdk;
using System.Collections.Generic;

namespace UnitTests
{
    public class FbxBindingTableTest : Base<FbxBindingTable>
    {
#if ENABLE_COVERAGE_TEST
        [Test]
        public override void TestCoverage() {
            // This test is also responsible for FbxBindingTableBase and FbxBindingTableEntry
            base.TestCoverage();
            CoverageTester.TestCoverage(typeof(FbxBindingTableBase), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxBindingTableEntry), this.GetType());
        }
#endif

        [Test]
        public void TestBasics() {
            var table = FbxBindingTable.Create(Manager, "table");

            // Call the getters, make sure they get.
            GetSetProperty(table.DescAbsoluteURL, "file:///dev/null");
            GetSetProperty(table.DescRelativeURL, "shader.glsl");
            GetSetProperty(table.DescTAG, "user");

            var entry = table.AddNewEntry(); // don't crash (nothing to test yet)
            entry.Dispose();
        }

        void GetSetProperty(FbxPropertyString prop, string value) {
            prop.Set(value);
            Assert.AreEqual(value, prop.Get());
        }
    }
}
