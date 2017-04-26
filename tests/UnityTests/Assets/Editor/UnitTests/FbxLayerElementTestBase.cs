// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.  
//
// Licensed under the ##LICENSENAME##. 
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// This file contains Tests for FbxLayerElement and all its derived classes.

using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using FbxSdk;

/*
 * Convenience class for testing all derived classes of FbxLayerElement.
 *
 * FbxLayerElement itself has no public constructor or Create function, so we can
 * only test its functions from its derived classes (which do have Create functions).
 */
namespace UnitTests
{
    public abstract class FbxLayerElementTestBase<T> where T: FbxSdk.FbxLayerElement
    {
        // Note: Create function is only present in derived classes (those which are actually used).
        //       Included it here so that we can test FbxLayerElement functions, and because they
        //       are all of the same format (avoid copy pasting into each derived class test class).
        // T.Create(FbxLayerContainer, string)
        static System.Reflection.MethodInfo s_createFromLayerContainerAndName;

        static FbxLayerElementTestBase() {
            s_createFromLayerContainerAndName = typeof(T).GetMethod("Create", new System.Type[] {typeof(FbxLayerContainer), typeof(string)});

            #if ENABLE_COVERAGE_TEST
            // Register the calls we make through reflection.

            // We use reflection in CreateObject(FbxLayerContainer, string)
            if (s_createFromLayerContainerAndName != null) {
                var createFromLayerContainerAndName = typeof(FbxLayerElementTestBase<T>).GetMethod("CreateObject", new System.Type[] {typeof(FbxLayerContainer), typeof(string)});
                CoverageTester.RegisterReflectionCall(createFromLayerContainerAndName, s_createFromLayerContainerAndName);
            }

            // Make sure to have the equality tester register its methods right now.
            EqualityTester<T>.RegisterCoverage();
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
            EqualityTester<T>.TestEquality(CreateObject("a"), CreateObject("b"));
        }

        /* Create an object with another layer container. Default implementation uses
         * reflection to call T.Create(...); override if reflection is wrong. */
        public virtual T CreateObject (FbxLayerContainer layerContainer, string name = "") {
            return Invoker.InvokeStatic<T>(s_createFromLayerContainerAndName, layerContainer, name);
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
            Assert.That (() => { CreateObject((FbxLayerContainer)null, "MyObject"); }, Throws.Exception.TypeOf<System.NullReferenceException>());

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
            Assert.That (() => { layerElement.SetMappingMode (FbxLayerElement.EMappingMode.eAllSame); }, Throws.Exception.TypeOf<System.NullReferenceException>());

            T element;
            using (element = CreateObject ("element2")) {
                element.SetMappingMode (FbxLayerElement.EMappingMode.eAllSame); // should be fine
            }
            Assert.That (() => { element.SetMappingMode (FbxLayerElement.EMappingMode.eAllSame); }, Throws.Exception.TypeOf<System.NullReferenceException>());
        }

        [Test]
        public void TestSetMappingMode ()
        {
            var layerElement = CreateObject ("element");
            layerElement.SetMappingMode (FbxLayerElement.EMappingMode.eByControlPoint);
        }

        [Test]
        public void TestSetReferenceMode ()
        {
            var layerElement = CreateObject ("element");
            layerElement.SetReferenceMode (FbxLayerElement.EReferenceMode.eDirect);
        }
    }

    /*
     * Convenience classes for testing functions of FbxLayerElementTemplate and its derived classes.
     * 
     * FbxLayerElementTemplate derives from FbxLayerElement, so we also derive from FbxLayerElementTestBase.
     * FbxLayerElementTemplate has no public constructors or Create function, therefore
     * this class is abstract and must be inherited from for the tests to run.
     * 
     * Had to create a test class for each template (FbxVector2, FbxVector4, and FbxColor)
     * as the template type is part of the class name.
     */
    public abstract class FbxLayerElementTemplateFbxVector4Test<T> : FbxLayerElementTestBase<T>
        where T: FbxSdk.FbxLayerElementTemplateFbxVector4
    {
            [Test]
            public void TestGetDirectArray() {
                var layerElement = CreateObject ("element");
                // make sure this doesn't crash
                var directArray = layerElement.GetDirectArray ();
            }

            [Test]
            public void TestGetIndexArray() {
                var layerElement = CreateObject ("element");
                // make sure this doesn't crash
                var directArray = layerElement.GetIndexArray ();
            }
    }

    public abstract class FbxLayerElementTemplateFbxColorTest<T> : FbxLayerElementTestBase<T>
        where T: FbxSdk.FbxLayerElementTemplateFbxColor
    {
        [Test]
        public void TestGetDirectArray() {
            var layerElement = CreateObject ("element");
            // make sure this doesn't crash
            var directArray = layerElement.GetDirectArray ();
        }

        [Test]
        public void TestGetIndexArray() {
            var layerElement = CreateObject ("element");
            // make sure this doesn't crash
            var directArray = layerElement.GetIndexArray ();
        }
    }

    public abstract class FbxLayerElementTemplateFbxVector2Test<T> : FbxLayerElementTestBase<T>
        where T: FbxSdk.FbxLayerElementTemplateFbxVector2
    {
        [Test]
        public void TestGetDirectArray() {
            var layerElement = CreateObject ("element");
            // make sure this doesn't crash
            var directArray = layerElement.GetDirectArray ();
        }

        [Test]
        public void TestGetIndexArray() {
            var layerElement = CreateObject ("element");
            // make sure this doesn't crash
            var directArray = layerElement.GetIndexArray ();
        }
    }

    /*
     * Tests for the classes derived from the FbxLayerElementTemplate classes.
     */
    public class FbxLayerElementUVTest : FbxLayerElementTemplateFbxVector2Test<FbxLayerElementUV>
    {}

    public class FbxLayerElementVertexColorTest : FbxLayerElementTemplateFbxColorTest<FbxLayerElementVertexColor>
    {}

    public class FbxLayerElementNormalTest : FbxLayerElementTemplateFbxVector4Test<FbxLayerElementNormal>
    {}

    public class FbxLayerElementBinormalTest : FbxLayerElementTemplateFbxVector4Test<FbxLayerElementBinormal>
    {}

    public class FbxLayerElementTangentTest : FbxLayerElementTemplateFbxVector4Test<FbxLayerElementTangent>
    {}
}