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
    public class FbxSystemUnitTest
    {

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxSystemUnit), this.GetType()); }
#endif

        [Test]
        public void TestEquality()
        {
            EqualityTester<FbxSystemUnit>.TestEquality(FbxSystemUnit.m, FbxSystemUnit.Yard);
        }

        /// <summary>
        /// Test the basics. Subclasses should override and add some calls
        /// e.g. to excercise all the constructors.
        /// </summary>
        [Test]
        public void TestBasics()
        {
            // Call all the functions. Test that a few of them actually work
            // (rather than merely not crashing).
            using (FbxSystemUnit.mm) { }
            using (FbxSystemUnit.cm) { }
            using (FbxSystemUnit.dm) { }
            using (FbxSystemUnit.m) { }
            using (FbxSystemUnit.km) { }
            using (FbxSystemUnit.Inch) { }
            using (FbxSystemUnit.Foot) { }
            using (FbxSystemUnit.Yard) { }

            var units = new FbxSystemUnit(0.1);
            Assert.AreEqual(0.1, units.GetScaleFactor());
            Assert.AreEqual(1, units.GetMultiplier(), 1);
            Assert.AreEqual("mm", units.GetScaleFactorAsString());
            Assert.AreEqual(FbxSystemUnit.mm, units);
            Assert.AreNotEqual(FbxSystemUnit.km, units);
            units.GetHashCode();
            units.ToString();
            units.Dispose();

            units = new FbxSystemUnit(0.1378123891, 324823);
            units.ToString();
            Assert.AreEqual("custom unit", units.GetScaleFactorAsString(pAbbreviated: false));
            Assert.AreNotEqual(units, FbxSystemUnit.mm);
        }
    }
}
