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
    public class FbxGeometryBaseTest : Base<FbxGeometryBase>
    {

        [Test]
        public void TestInitControlPoints ()
        {
            using (FbxGeometryBase geometryBase = CreateObject ("geometry base")) {
                geometryBase.InitControlPoints (24);
                Assert.AreEqual (geometryBase.GetControlPointsCount (), 24);
            }
        }

        [Test]
        public void TestInitNegativeControlPoints ()
        {
            using (FbxGeometryBase geometryBase = CreateObject ("geometry base")) {
                geometryBase.InitControlPoints (-1);
            }
        }

        [Test]
        public void TestSetControlPointAt ()
        {
            using (FbxGeometryBase geometryBase = CreateObject ("geometry base")) {
                geometryBase.InitControlPoints (5);
                FbxVector4 vector = new FbxVector4 ();
                geometryBase.SetControlPointAt (vector, 0);
                Assert.AreEqual (vector, geometryBase.GetControlPointAt (0));
            }
        }

        [Test]
        public void TestSetControlPointAtWithoutInit ()
        {
            using (FbxGeometryBase geometryBase = CreateObject ("geometry base")) {
                FbxVector4 vector = new FbxVector4 ();
                geometryBase.SetControlPointAt (vector, 0);
            }
        }

        [Test]
        public void TestSetControlPointAtInvalidFbxVector4 ()
        {
            using (FbxGeometryBase geometryBase = CreateObject ("geometry base")) {
                geometryBase.InitControlPoints (5);
                FbxVector4 vector = new FbxVector4 ();
                vector.Dispose ();
                Assert.That (() => { geometryBase.SetControlPointAt (vector, 0); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
            }
        }

        [Test]
        public void TestSetControlPointAtNullFbxVector4 ()
        {
            using (FbxGeometryBase geometryBase = CreateObject ("geometry base")) {
                geometryBase.InitControlPoints (5);
                Assert.That (() => { geometryBase.SetControlPointAt (null, 0); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
            }
        }

        [Test]
        [Ignore("Calling SetControlPointAt with a negative index crashes Unity")]
        public void TestSetControlPointAtInvalidIndex ()
        {
            using (FbxGeometryBase geometryBase = CreateObject ("geometry base")) {
                geometryBase.InitControlPoints (5);
                FbxVector4 vector = new FbxVector4 ();
                geometryBase.SetControlPointAt (vector, -1);
            }
        }

    }
}