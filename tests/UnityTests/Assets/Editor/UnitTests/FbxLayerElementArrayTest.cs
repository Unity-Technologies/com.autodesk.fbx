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
    public class FbxLayerElementArrayTest
    {

        [Test]
        public void TestSetCount ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxDouble);

            layerElementArray.SetCount (1);
            Assert.AreEqual (layerElementArray.GetCount (), 1);

            // test invalid
            layerElementArray.SetCount (-1);
        }

        [Test]
        public void TestAddInt ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxInt);

            layerElementArray.Add (0);
            layerElementArray.Add (-1);
        }

        [Test]
        public void TestAddFbxColor ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.Add (new FbxColor ());
            layerElementArray.Add (new FbxColor (1, 0, 0));
        }

        [Test]
        public void TestAddFbxVector2 ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.Add (new FbxVector2 ());
            layerElementArray.Add (new FbxVector2 (1, 0));
        }

        [Test]
        public void TestAddFbxVector4 ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.Add (new FbxVector4 ());
            layerElementArray.Add (new FbxVector4 (1, 0, 0));
        }

        [Test]
        public void TestSetAtInt ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.SetAt (0, 1);

            // test invalid index
            layerElementArray.SetAt (-1, 1);

            // test negative int
            layerElementArray.SetAt (1, -1);
        }

        [Test]
        public void TestSetAtFbxColor ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.SetAt (0, new FbxColor ());

            // test invalid index
            layerElementArray.SetAt (-1, new FbxColor ());
        }

        [Test]
        public void TestSetAtFbxVector2 ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.SetAt (0, new FbxVector2 ());

            // test invalid index
            layerElementArray.SetAt (-1, new FbxVector2 ());
        }

        [Test]
        public void TestSetAtFbxVector4 ()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);

            layerElementArray.SetAt (0, new FbxVector4 ());

            // test invalid index
            layerElementArray.SetAt (-1, new FbxVector4 ());
        }

        [Test]
        public void TestDispose()
        {
            var layerElementArray = new FbxLayerElementArray (EFbxType.eFbxBlob);
            layerElementArray.Dispose ();
            Assert.That (() => {
                layerElementArray.SetCount (1);
            }, Throws.Exception.TypeOf<System.NullReferenceException> ());

            FbxLayerElementArray elementArray;
            using (elementArray = new FbxLayerElementArray (EFbxType.eFbxBlob)) {}
            Assert.That (() => {
                elementArray.SetCount (1);
            }, Throws.Exception.TypeOf<System.NullReferenceException> ());
        }

        #if ENABLE_COVERAGE_TEST
        [Test]
        public virtual void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxLayerElementArray), this.GetType()); }
        #endif
    }

    public abstract class FbxLayerElementArrayTemplateTestBase<T,U> : FbxLayerElementArrayTest where T : FbxSdk.FbxLayerElementArray {

        static System.Reflection.ConstructorInfo s_constructor;
        static System.Reflection.MethodInfo s_getAt;

        static FbxLayerElementArrayTemplateTestBase() {
            s_constructor = typeof(T).GetConstructor (new[] { typeof(EFbxType) });

            s_getAt = typeof(T).GetMethod("GetAt", new System.Type[] { typeof(int) });

            #if ENABLE_COVERAGE_TEST
            // Register the calls we make through reflection.

            // We use reflection in CreateObject(FbxLayerContainer, string)
            if (s_constructor != null) {
                var constructor = typeof(FbxLayerElementArrayTemplateTestBase<T,U>).GetMethod("CreateObject", new System.Type[] { typeof(EFbxType) });
                CoverageTester.RegisterReflectionCall(constructor, s_constructor);
            }

            if(s_getAt != null){
                var getAt = typeof(FbxLayerElementArrayTemplateTestBase<T,U>).GetMethod("GetAt");
                CoverageTester.RegisterReflectionCall(getAt, s_getAt);
            }
            #endif
        }

        /* Create an object with the default manager. */
        public T CreateObject (EFbxType type = EFbxType.eFbxBlob) {
            return Invoker.InvokeConstructor<T>(s_constructor, type);
        }

        public U GetAt(T layerElementArray, int index){
            return Invoker.Invoke<U> (s_getAt, layerElementArray, index);
        }

        #if ENABLE_COVERAGE_TEST
        [Test]
        public override void TestCoverage() { CoverageTester.TestCoverage(typeof(T), this.GetType()); }
        #endif

        [Test]
        public void TestGetAt()
        {
            var layerElementArrayTemplate = CreateObject ();

            // make sure doesn't crash
            GetAt (layerElementArrayTemplate, 0);
            GetAt (layerElementArrayTemplate, int.MinValue);
            GetAt (layerElementArrayTemplate, int.MaxValue);
        }
    }

    public class FbxLayerElementArrayTemplateFbxColorTest : 
        FbxLayerElementArrayTemplateTestBase<FbxLayerElementArrayTemplateFbxColor,FbxColor> {}

    public class FbxLayerElementArrayTemplateFbxSurfaceMaterialTest :
        FbxLayerElementArrayTemplateTestBase<FbxLayerElementArrayTemplateFbxSurfaceMaterial,FbxSurfaceMaterial> {}

    [Ignore("Calling GetAt() causes a crash")]
    public class FbxLayerElementArrayTemplateFbxVector2Test : 
        FbxLayerElementArrayTemplateTestBase<FbxLayerElementArrayTemplateFbxVector2,FbxVector2> {}

    public class FbxLayerElementArrayTemplateFbxVector4Test : 
        FbxLayerElementArrayTemplateTestBase<FbxLayerElementArrayTemplateFbxVector4,FbxVector4> {}

    public class FbxLayerElementArrayTemplateIntTest : 
        FbxLayerElementArrayTemplateTestBase<FbxLayerElementArrayTemplateInt,int> {}
}
