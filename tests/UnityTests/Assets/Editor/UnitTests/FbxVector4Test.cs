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
            // make sure the constructors compile and don't crash
            new FbxVector4();
            new FbxVector4(new FbxVector4());
            new FbxVector4(1, 2, 3, 4);
            new FbxVector4(1, 2, 3);
        }

        [Test]
        public void TestUsing ()
        {
            /* make sure that the using form compiles and doesn't crash */
            using (new FbxVector4()) { }

            // Make sure we can explicitly dispose as well.
            new FbxVector4().Dispose();
        }

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxVector4), this.GetType()); }
#endif
    }
}
