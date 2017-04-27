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

#if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() {
            CoverageTester.TestCoverage(typeof(FbxProperty), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyDouble3), this.GetType());
            CoverageTester.TestCoverage(typeof(FbxPropertyString), this.GetType());
        }
#endif

        [Test]
        public void TestEquality() {
            using(var manager = FbxManager.Create()) {
                var node = FbxNode.Create(manager, "node");
                var translation = node.LclTranslation;
                var rotation = node.LclRotation;

                EqualityTester<FbxPropertyDouble3>.TestEquality(translation, rotation);
            }
        }

        [Test]
        public void BasicTests ()
        {
            using(var manager = FbxManager.Create()) {
                // Easiest way to get a Double3 property: get a node and access its LclTranslation.
                var node = FbxNode.Create(manager, "node");
                var property = node.LclTranslation;

                Assert.AreEqual(Globals.FbxLocalTranslationDT, property.GetPropertyDataType());
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

                Assert.IsTrue(property.Set(5.0f));
                Assert.AreEqual(new FbxDouble3(5.0f), property.Get());

                // TODO: dispose will in the future destroy, which is illegal in this context;
                // modify the test then.
                property.Dispose();
                using(var prop2 = node.LclScaling) { }
            }

            using (var manager = FbxManager.Create()) {
                // How to get a String property? Create an FbxImplementation (a shader implementation).
                var impl = FbxImplementation.Create(manager, "name");
                var property = impl.RenderAPI;

                Assert.AreEqual(Globals.FbxStringDT, property.GetPropertyDataType());
                Assert.AreEqual("RenderAPI", property.GetName());
                Assert.AreEqual("RenderAPI", property.ToString());
                Assert.AreEqual("RenderAPI", property.GetHierarchicalName());
                Assert.AreEqual("RenderAPI", property.GetLabel(true));
                property.SetLabel("label");
                Assert.AreEqual("label", property.GetLabel());
                Assert.AreEqual(impl, property.GetFbxObject());

                property.Set("a value");
                Assert.AreEqual("a value", property.Get());

                Assert.IsTrue(property.Set(5.0f));
                Assert.AreEqual("5.000000", property.Get());

                property.Dispose();
                using(var prop2 = impl.RenderAPI) { }
            }

            using (var manager = FbxManager.Create()) {
                FbxProperty root, child;
                var obj = FbxObject.Create(manager, "obj");

                Assert.IsNotNull(FbxProperty.Create(obj, Globals.FbxStringDT, "a"));
                Assert.IsNotNull(FbxProperty.Create(obj, Globals.FbxStringDT, "b", "label"));
                Assert.IsNotNull(FbxProperty.Create(obj, Globals.FbxStringDT, "c", "label", false));
                bool didFind;
                Assert.IsNotNull(FbxProperty.Create(obj, Globals.FbxStringDT, "c", "label", true, out didFind));
                Assert.IsTrue(didFind);

                root = FbxProperty.Create(obj, Globals.FbxCompoundDT, "root");

                child = FbxProperty.Create(root, Globals.FbxStringDT, "a");
                Assert.IsNotNull(child);
                Assert.IsNotNull(FbxProperty.Create(root, Globals.FbxStringDT, "b", "label"));
                Assert.IsNotNull(FbxProperty.Create(root, Globals.FbxStringDT, "c", "label", false));
                Assert.IsNotNull(FbxProperty.Create(root, Globals.FbxStringDT, "c", "label", true, out didFind));
                Assert.IsTrue(didFind);

                child.Destroy();

                root.DestroyChildren();
                Assert.IsNotNull(FbxProperty.Create(root, Globals.FbxStringDT, "c", "label", true, out didFind));
                Assert.IsFalse(didFind);

                root.DestroyRecursively();
            }
        }
    }
}
