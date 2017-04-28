// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
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
            FbxTime time = new FbxTime();
            time = new FbxTime (1);

            // TODO: fix so calling Dispose() doesn't throw an exception
            Assert.That (() => { time.Dispose(); }, Throws.Exception.TypeOf<System.MethodAccessException>());
        }
    }
}