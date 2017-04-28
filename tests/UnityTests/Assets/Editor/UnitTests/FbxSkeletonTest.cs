using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using FbxSdk;

namespace UnitTests
{
    public class FbxSkeletonTest : Base<FbxSkeleton>
    {

        [Test]
        public void TestSetSkeletonType ()
        {
            var skeleton = CreateObject ("skeleton");
            skeleton.SetSkeletonType(FbxSkeleton.EType.eLimb);
            Assert.AreEqual (skeleton.GetSkeletonType (), FbxSkeleton.EType.eLimb);
        }
    }
}