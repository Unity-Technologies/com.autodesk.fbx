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
    public class FbxAxisSystemTest
    {

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxAxisSystem), this.GetType()); }
#endif

        /// <summary>
        /// Test the basics. Subclasses should override and add some calls
        /// e.g. to excercise all the constructors.
        /// </summary>
        [Test]
        public void TestBasics()
        {
            using (FbxAxisSystem.MayaZUp) { }
            using (FbxAxisSystem.MayaYUp) { }
            using (FbxAxisSystem.Max) { }
            using (FbxAxisSystem.Motionbuilder) { }
            using (FbxAxisSystem.OpenGL) { }
            using (FbxAxisSystem.DirectX) { }
            using (FbxAxisSystem.Lightwave) { }
            var axes = new FbxAxisSystem(FbxAxisSystem.Lightwave);
            Assert.That(axes.GetHashCode(), Is.LessThan(0));
            axes.Dispose();
        }
    }
}
