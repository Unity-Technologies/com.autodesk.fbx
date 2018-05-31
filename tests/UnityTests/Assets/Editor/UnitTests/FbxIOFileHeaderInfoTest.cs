using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.Formats.FbxSdk;

namespace UnityEngine.Formats.FbxSdk.UnitTests
{
    public class FbxIOFileHeaderInfoTest : TestBase<FbxIOFileHeaderInfo>
    {

        [Test]
        public void TestBasics ()
        {
            var fileHeaderInfo = new FbxIOFileHeaderInfo ();
            TestGetter (fileHeaderInfo.mCreator);
            TestGetter (fileHeaderInfo.mFileVersion);
            fileHeaderInfo.Dispose ();
        }
    }
}