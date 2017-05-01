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
        public void TestConstructor()
        {
            // test constructor
            FbxStatus status = new FbxStatus ();
            Assert.IsNotNull (status);
        }

        [Test]
        [Ignore("Dispose fails with a MethodAccessException as there is no default destructor")]
        public void TestDispose()
        {
            FbxStatus status;
            status = new FbxStatus ();
            status.Dispose ();
            Assert.That (() => { status.GetType(); }, Throws.Exception.TypeOf<System.NullReferenceException>());

            using (status = new FbxStatus ()) {}
            Assert.That (() => { status.GetType(); }, Throws.Exception.TypeOf<System.NullReferenceException>());
        }
    }
}