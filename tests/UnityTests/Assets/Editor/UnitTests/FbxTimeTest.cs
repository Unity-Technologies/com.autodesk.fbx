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
        }

        [Test]
        [Ignore("Dispose throws a MethodAccessException as FbxTime has no public default destructor")]
        public void TestDispose()
        {
            FbxTime time;
            time = new FbxTime ();
            time.Dispose ();
            Assert.That (() => { time.ToString(); }, Throws.Exception.TypeOf<System.ArgumentNullException>());

            using (time = new FbxTime (1)) {}
            Assert.That (() => { time.ToString(); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
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

            Assert.That (() => { new FbxTimeSpan(null, null); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        [Ignore("Dispose throws a MethodAccessException as FbxTime has no public default destructor")]
        public void TestDispose()
        {
            FbxTimeSpan time;
            time = new FbxTimeSpan ();
            time.Dispose ();
            Assert.That (() => { time.ToString(); }, Throws.Exception.TypeOf<System.ArgumentNullException>());

            using (time = new FbxTimeSpan (new FbxTime(1), new FbxTime(2))) {}
            Assert.That (() => { time.ToString(); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }
    }
}
