// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// This file contains Tests for FbxLayerElement and all its derived classes.

using NUnit.Framework;
using System.Collections;
using Autodesk.Fbx;

/*
 * Convenience class for testing all derived classes of FbxLayerElement.
 *
 * FbxLayerElement itself has no public constructor or Create function, so we can
 * only test its functions from its derived classes (which do have Create functions).
 */
namespace Autodesk.Fbx.UnitTests
{
    public abstract class FbxLayerElementTestBase<T> where T: Autodesk.Fbx.FbxLayerElement
    {
        // Note: Create function is only present in derived classes (those which are actually used).
        //       Included it here so that we can test FbxLayerElement functions, and because they
        //       are all of the same format (avoid copy pasting into each derived class test class).
        // T.Create(FbxLayerContainer, string)
        static System.Reflection.MethodInfo s_createFromLayerContainerAndName;

        static System.Reflection.MethodInfo s_getDirectArray;
        static System.Reflection.MethodInfo s_getIndexArray;

        static FbxLayerElementTestBase() {
            s_createFromLayerContainerAndName = typeof(T).GetMethod("Create", new System.Type[] {typeof(FbxLayerContainer), typeof(string)});

            s_getDirectArray = typeof(T).GetMethod ("GetDirectArray");
            s_getIndexArray = typeof(T).GetMethod ("GetIndexArray");

            #if ENABLE_COVERAGE_TEST
            // Register the calls we make through reflection.

            // We use reflection in CreateObject(FbxLayerContainer, string)
            if (s_createFromLayerContainerAndName != null) {
                var createFromLayerContainerAndName = typeof(FbxLayerElementTestBase<T>).GetMethod("CreateObject", new System.Type[] {typeof(FbxLayerContainer), typeof(string)});
                CoverageTester.RegisterReflectionCall(createFromLayerContainerAndName, s_createFromLayerContainerAndName);
            }

            if(s_getDirectArray != null){
                var getDirectArray = typeof(FbxLayerElementTestBase<T>).GetMethod("GetDirectArray");
                CoverageTester.RegisterReflectionCall(getDirectArray, s_getDirectArray);
            }

            if(s_getIndexArray != null){
                var getIndexArray = typeof(FbxLayerElementTestBase<T>).GetMethod("GetIndexArray");
                CoverageTester.RegisterReflectionCall(getIndexArray, s_getIndexArray);
            }
            #endif
        }

        protected FbxManager m_fbxManager;

        protected FbxLayerContainer LayerContainer {
            get;
            private set;
        }

        /* Create an object with the default manager. */
        public T CreateObject (string name = "") {
            return CreateObject(LayerContainer, name);
        }

        #if ENABLE_COVERAGE_TEST
        [Test]
        public void TestCoverage() { CoverageTester.TestCoverage(typeof(T), this.GetType()); }
        #endif

        /* Test all the equality functions we can find. */
        [Test]
        public virtual void TestEquality() {
            var a = CreateObject("a");
            var b = CreateObject("b");
            var acopy = a; // TODO: copy the proxy
            EqualityTester<T>.TestEquality(a, b, acopy);
        }

        /* Create an object with another layer container. Default implementation uses
         * reflection to call T.Create(...); override if reflection is wrong. */
        public virtual T CreateObject (FbxLayerContainer layerContainer, string name = "") {
            return Invoker.InvokeStatic<T>(s_createFromLayerContainerAndName, layerContainer, name);
        }

        public virtual FbxLayerElementArray GetDirectArray(T layerElement)
        {
            return Invoker.Invoke<FbxLayerElementArray> (s_getDirectArray, layerElement);
        }

        public virtual FbxLayerElementArrayTemplateInt GetIndexArray(T layerElement)
        {
            return Invoker.Invoke<FbxLayerElementArrayTemplateInt> (s_getIndexArray, layerElement);
        }

        [SetUp]
        public virtual void Init ()
        {
            m_fbxManager = FbxManager.Create ();
            LayerContainer = FbxLayerContainer.Create (m_fbxManager, "layer container");
        }

        [TearDown]
        public virtual void Term ()
        {
            try {
                m_fbxManager.Destroy ();
            }
            catch (System.ArgumentNullException) {
            }
        }

        [Test]
        public void TestCreate()
        {
            var obj = CreateObject("MyObject");
            Assert.IsInstanceOf<T> (obj);

            // test null container
            Assert.That (() => { CreateObject((FbxLayerContainer)null, "MyObject"); }, Throws.Exception.TypeOf<System.ArgumentNullException>());

            // test null name
            CreateObject((string)null);

            // test zombie
            var layerContainer = FbxLayerContainer.Create(m_fbxManager, "");
            layerContainer.Destroy();
            Assert.That (() => { CreateObject(layerContainer, "MyObject"); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        public void TestDispose ()
        {
            var layerElement = CreateObject ("element");
            layerElement.Dispose ();
            Assert.That (() => { layerElement.SetMappingMode (FbxLayerElement.EMappingMode.eAllSame); }, Throws.Exception.TypeOf<System.ArgumentNullException>());

            T element;
            using (element = CreateObject ("element2")) {
                element.SetMappingMode (FbxLayerElement.EMappingMode.eAllSame); // should be fine
            }
            Assert.That (() => { element.SetMappingMode (FbxLayerElement.EMappingMode.eAllSame); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        public void TestSetMappingMode ()
        {
            var layerElement = CreateObject ("element");
            layerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);
            Assert.AreEqual (layerElement.GetMappingMode (), FbxLayerElement.EMappingMode.eByControlPoint);
        }

        [Test]
        public void TestSetReferenceMode ()
        {
            var layerElement = CreateObject ("element");
            layerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);
            Assert.AreEqual (layerElement.GetReferenceMode (), FbxLayerElement.EReferenceMode.eDirect);
        }

        [Test]
        public void TestGetDirectArray() {
            var layerElement = CreateObject ("element");
            // make sure this doesn't crash
            GetDirectArray (layerElement);
        }

        [Test]
        public void TestGetIndexArray() {
            var layerElement = CreateObject ("element");
            // make sure this doesn't crash
            GetIndexArray (layerElement);
        }
    }

    /*
     * Tests for the classes derived from the FbxLayerElementTemplate classes.
     */
    public class FbxLayerElementUVTest : FbxLayerElementTestBase<FbxLayerElementUV>
    {}

    public class FbxLayerElementVertexColorTest : FbxLayerElementTestBase<FbxLayerElementVertexColor>
    {}

    public class FbxLayerElementNormalTest : FbxLayerElementTestBase<FbxLayerElementNormal>
    {}

    public class FbxLayerElementBinormalTest : FbxLayerElementTestBase<FbxLayerElementBinormal>
    {}

    public class FbxLayerElementTangentTest : FbxLayerElementTestBase<FbxLayerElementTangent>
    {}

    public class FbxLayerElementMaterialTest : FbxLayerElementTestBase<FbxLayerElementMaterial>
    {}
}
