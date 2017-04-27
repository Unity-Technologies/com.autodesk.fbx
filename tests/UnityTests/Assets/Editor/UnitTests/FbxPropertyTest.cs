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
        // tests that should work for any subclass of FbxProperty
        private void GenericPropertyTests<T>(T property, FbxNode node, string propertyName) where T:FbxProperty{
            Assert.AreEqual(propertyName, property.GetName());
            Assert.AreEqual(propertyName, property.ToString());
            Assert.AreEqual(propertyName, property.GetHierarchicalName());
            Assert.AreEqual(propertyName, property.GetLabel(true));
            property.SetLabel("label");
            Assert.AreEqual("label", property.GetLabel());
            Assert.AreEqual(node, property.GetFbxObject());
            Assert.AreEqual(property.GetFbxObject(), node); // test it both ways just in case equals is busted
        }

        [Test]
        public void BasicTests ()
        {
            // Easiest way to get a Double3 property: get a node and access its LclTranslation.
            using(var manager = FbxManager.Create()) {
                var node = FbxNode.Create(manager, "node");
                var property = node.LclTranslation;
                GenericPropertyTests<FbxPropertyDouble3> (property, node, "Lcl Translation");

                var dbl3 = property.Get();
                Assert.AreEqual(new FbxDouble3(), dbl3);
                property.Set(new FbxDouble3(1,2,3));
                dbl3 = property.Get();
                Assert.AreEqual(new FbxDouble3(1, 2, 3), dbl3);

                // TODO: dispose will in the future destroy, which is illegal in this context;
                // modify the test then.
                property.Dispose();
                using(var prop2 = node.LclScaling) { }

                // Run the same tests for FbxPropertyT<FbxBool> used by VisibilityInheritance
                var visibilityInheritance = node.VisibilityInheritance;
                GenericPropertyTests<FbxPropertyBool> (visibilityInheritance, node, "Visibility Inheritance");

                var fbxBool = visibilityInheritance.Get();
                Assert.AreEqual(true, fbxBool); // VisibilityInheritance is true by default
                visibilityInheritance.Set(false);
                fbxBool = visibilityInheritance.Get();
                Assert.AreEqual(false, fbxBool);

                // TODO: dispose will in the future destroy, which is illegal in this context;
                // modify the test then.
                visibilityInheritance.Dispose();
                using(var prop2 = node.VisibilityInheritance) { }
            }
        }

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() {
            CoverageTester.TestCoverage(typeof(FbxProperty), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyDouble3), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyBool), this.GetType());
        }
#endif
    }
}
