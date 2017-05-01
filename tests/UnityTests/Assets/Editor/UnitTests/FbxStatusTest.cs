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
    public class FbxStatusTest
    {
        #if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxStatus), this.GetType()); }
        #endif

        [Test]
        public void TestBasics()
        {
            // test constructor
            FbxStatus status = new FbxStatus ();
            Assert.IsNotNull (status);

            // test dispose
            Assert.That (() => { status.Dispose(); }, Throws.Exception.TypeOf<System.MethodAccessException>());
        }
    }
}