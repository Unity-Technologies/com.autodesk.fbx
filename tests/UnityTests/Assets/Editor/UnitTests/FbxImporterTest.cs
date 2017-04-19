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
    public class FbxImporterTest : Base<FbxImporter>
    {
        [Test]
        public void TestImport1 ()
        {
            using (FbxImporter newImporter = CreateObject("MyImporter"))
            {
                Assert.IsFalse (newImporter.Import (null));
            }
        }

        [Test]
        public void TestImport2 ()
        {
            using (FbxImporter newImporter = CreateObject("MyImporter"))
            {
                Assert.IsFalse (newImporter.Import (null, false));

                // don't ask
                Assert.IsTrue (newImporter.Import (null, true));
            }
        }
    }
}
