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
    public class FbxCollectionTest : Base<FbxCollection>
    {

        public static void GenericTests<T>(T fbxCollection, FbxManager manager) where T : FbxCollection
        {
            // TODO: FbxScene has a member count of 3 instead of one (even after clearing), is this normal?
            int initialMemberCount = fbxCollection.GetMemberCount ();

            // test AddMember
            FbxObject obj = FbxObject.Create (manager, "");
            bool result = fbxCollection.AddMember (obj);
            Assert.IsTrue (result);
            Assert.AreEqual(initialMemberCount+1, fbxCollection.GetMemberCount());

            // test Clear
            fbxCollection.Clear ();
            Assert.AreEqual (initialMemberCount, fbxCollection.GetMemberCount());
        }

        [Test]
        public void TestBasics ()
        {
            GenericTests (CreateObject ("fbx collection"), Manager);
        }
    }
}
