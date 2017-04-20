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

    public class FbxObjectTest : Base<FbxObject>
    {
        [Test]
        public void TestEquality()
        {
            FbxObject a = FbxObject.Create(Manager, "a");
            FbxObject b = FbxObject.Create(Manager, "b");

            Assert.IsFalse(a == b);
            Assert.IsTrue(a == a);
            Assert.IsFalse(a == null);
            Assert.IsFalse(null == b);
            Assert.IsTrue((FbxObject)null == (FbxObject)null);

            Assert.IsTrue(a != b);
            Assert.IsFalse(a != a);
            Assert.IsTrue(a != null);
            Assert.IsTrue(null != b);
            Assert.IsFalse((FbxObject)null != (FbxObject)null);

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals(a as FbxEmitter));
            Assert.IsTrue((a as FbxEmitter).Equals(a));
            Assert.IsTrue((a as FbxEmitter).Equals(a as FbxEmitter));
            Assert.IsTrue((a as object).Equals(a as object));

            Assert.IsFalse(a.Equals(b));
            Assert.IsFalse(a.Equals(b as FbxEmitter));
            Assert.IsFalse((a as FbxEmitter).Equals(b));
            Assert.IsFalse((a as FbxEmitter).Equals(b as FbxEmitter));
            Assert.IsFalse((a as object).Equals(b as object));
        }

        [Test]
        public void TestUTF8()
        {
            // make sure japanese survives the round-trip.
            string katakana = "片仮名";
            FbxObject obj = FbxObject.Create(Manager, katakana);
            Assert.AreEqual(katakana, obj.GetName());
        }

        [Test]
        public void TestFindClass ()
        {
            FbxClassId classId = Manager.FindClass ("FbxObject");

            Assert.AreEqual (classId.GetName (), "FbxObject");
        }

        [Test]
        public void TestFbxManager ()
        {
            using (FbxObject obj = FbxObject.Create (Manager, "")) {
                FbxManager fbxManager2 = obj.GetFbxManager();
                Assert.IsNotNull(fbxManager2);
            }
        }
    }
}
