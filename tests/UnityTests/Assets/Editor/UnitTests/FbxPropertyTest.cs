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
    public class FbxPropertyTest
    {
        [Test]
        public void BasicTests ()
        {
            // Easiest way to get a Double3 property: get a node and access its LclTranslation.
            using(var manager = FbxManager.Create()) {
                var node = FbxNode.Create(manager, "node");
                var property = node.LclTranslation;
                Assert.AreEqual("Lcl Translation", property.GetName());
                Assert.AreEqual("Lcl Translation", property.ToString());
                Assert.AreEqual("Lcl Translation", property.GetHierarchicalName());
                Assert.AreEqual("Lcl Translation", property.GetLabel(true));
                property.SetLabel("label");
                Assert.AreEqual("label", property.GetLabel());
                Assert.AreEqual(node, property.GetFbxObject());

                var dbl3 = property.Get();
                Assert.AreEqual(new FbxDouble3(), dbl3);
                property.Set(new FbxDouble3(1,2,3));
                dbl3 = property.Get();
                Assert.AreEqual(new FbxDouble3(1, 2, 3), dbl3);

                // TODO: dispose will in the future destroy, which is illegal in this context;
                // modify the test then.
                property.Dispose();
                using(var prop2 = node.LclScaling) { }
            }
        }

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() {
            CoverageTester.TestCoverage(typeof(FbxProperty), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyDouble3), this.GetType());
        }
#endif
    }
}
