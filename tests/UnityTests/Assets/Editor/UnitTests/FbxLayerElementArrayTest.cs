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
    public class FbxLayerElementArrayTest
    {

        [Test]
        public void TestSetCount ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxDouble);

            layerElementArray.SetCount (1);
            Assert.AreEqual (layerElementArray.GetCount (), 1);

            // test invalid
            layerElementArray.SetCount (-1);
        }

        [Test]
        public void TestAddInt ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxInt);

            layerElementArray.Add (0);
            layerElementArray.Add (-1);
        }

        [Test]
        public void TestAddFbxColor ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.Add (new FbxColor ());
            layerElementArray.Add (new FbxColor (1, 0, 0));

            Assert.That (() => {
                layerElementArray.Add ((FbxColor)null);
            }, Throws.Exception.TypeOf<System.ArgumentNullException> ());
        }

        [Test]
        public void TestAddFbxVector2 ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.Add (new FbxVector2 ());
            layerElementArray.Add (new FbxVector2 (1, 0));
        }

        [Test]
        public void TestAddFbxVector4 ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.Add (new FbxVector4 ());
            layerElementArray.Add (new FbxVector4 (1, 0, 0));
        }

        [Test]
        public void TestSetAtInt ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.SetAt (0, 1);

            // test invalid index
            layerElementArray.SetAt (-1, 1);

            // test negative int
            layerElementArray.SetAt (1, -1);
        }

        [Test]
        public void TestSetAtFbxColor ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.SetAt (0, new FbxColor ());

            // test invalid index
            layerElementArray.SetAt (-1, new FbxColor ());

            // test null
            Assert.That (() => {
                layerElementArray.SetAt (0, (FbxColor)null);
            }, Throws.Exception.TypeOf<System.ArgumentNullException> ());
        }

        [Test]
        public void TestSetAtFbxVector2 ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.SetAt (0, new FbxVector2 ());

            // test invalid index
            layerElementArray.SetAt (-1, new FbxVector2 ());
        }

        [Test]
        public void TestSetAtFbxVector4 ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.SetAt (0, new FbxVector4 ());

            // test invalid index
            layerElementArray.SetAt (-1, new FbxVector4 ());
        }

        [Test]
        public void TestDispose()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);
            layerElementArray.Dispose ();
            Assert.That (() => {
                layerElementArray.SetCount (1);
            }, Throws.Exception.TypeOf<System.NullReferenceException> ());

            FbxLayerElementArray elementArray;
            using (elementArray = new FbxLayerElementArray (EFbxType.eFbxBlob)) {}
            Assert.That (() => {
                elementArray.SetCount (1);
            }, Throws.Exception.TypeOf<System.NullReferenceException> ());
        }

        #if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxLayerElementArray), this.GetType()); }
        #endif
    }
}
