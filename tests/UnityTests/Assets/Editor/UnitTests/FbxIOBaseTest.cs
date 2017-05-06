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

    public class FbxIOBaseTest<T> : Base<T> where T: FbxIOBase
    {
        [Test]
        public virtual void TestBasics()
        {
            using (var iobase = CreateObject()) { iobase.Initialize("/no/such/file.fbx"); }
            using (var iobase = CreateObject()) { iobase.Initialize("/no/such/file.fbx", -1); }
            using (var iobase = CreateObject()) { iobase.Initialize("/no/such/file.fbx", -1, FbxIOSettings.Create(Manager, "")); }
            using (var iobase = CreateObject()) { iobase.Initialize("/no/such/file.fbx", -1, null); }
        }
    }

    public class FbxIOBaseTestClass : FbxIOBaseTest<FbxIOBase> { }
}
