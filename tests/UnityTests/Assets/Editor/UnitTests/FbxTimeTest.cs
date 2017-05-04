// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

using NUnit.Framework;
using System.Collections;
using FbxSdk;

namespace UnitTests
{
    public class FbxTimeTest
    {

        #if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxTime), this.GetType()); }
        #endif

        [Test]
        public void TestBasics ()
        {
            // just make sure it doesn't crash
            new FbxTime();
            new FbxTime (1);

            // test dispose
            DisposeTester.TestDispose(new FbxTime());
            using (new FbxTime (1)) {}
        }
    }

    public class FbxTimeSpanTest
    {
        #if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxTimeSpan), this.GetType()); }
        #endif

        [Test]
        public void TestBasics ()
        {
            // just make sure it doesn't crash
            new FbxTimeSpan();
            new FbxTimeSpan (new FbxTime(1), new FbxTime(2));

            // test dispose
            DisposeTester.TestDispose(new FbxTimeSpan());
            using (new FbxTimeSpan (new FbxTime(1), new FbxTime(2))) { }

            Assert.That (() => { new FbxTimeSpan(null, null); }, Throws.Exception.TypeOf<System.ArgumentNullException>());

            // test Set
            FbxTimeSpan timeSpan = new FbxTimeSpan();
            timeSpan.Set (new FbxTime (2), new FbxTime (3));
            Assert.That (() => { timeSpan.Set(null, null); }, Throws.Exception.TypeOf<System.ArgumentNullException>());

        }
    }
}
