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
    [Ignore("Create function returns null")]
    public class FbxAnimCurveBaseTest : Base<FbxAnimCurveBase>
    {
        [Test]
        public override void TestCreate ()
        {
            // have to override the TestCreate as FbxAnimCurveBase doesn't have a create function that takes an object

            var obj = CreateObject("MyObject");
            Assert.IsInstanceOf<FbxAnimCurve> (obj);
            Assert.AreEqual(Manager, obj.GetFbxManager());

            using(var manager2 = FbxManager.Create()) {
                var obj2 = CreateObject(manager2, "MyOtherObject");
                Assert.AreEqual(manager2, obj2.GetFbxManager());
                Assert.AreNotEqual(Manager, obj2.GetFbxManager());
            }

            // Test with a null manager or container. Should throw.
            Assert.That (() => { CreateObject((FbxManager)null, "MyObject"); }, Throws.Exception.TypeOf<System.NullReferenceException>());

            // Test with a null string. Should work.
            Assert.IsNotNull(CreateObject((string)null));

            // Test with a destroyed manager. Should throw.
            var mgr = FbxManager.Create();
            mgr.Destroy();
            Assert.That (() => { CreateObject(mgr, "MyObject"); }, Throws.Exception.TypeOf<System.ArgumentNullException>());

            // Test with a disposed manager. Should throw.
            mgr = FbxManager.Create();
            mgr.Dispose();
            Assert.That (() => { CreateObject(mgr, "MyObject"); }, Throws.Exception.TypeOf<System.NullReferenceException>());
        }

        // FbxAnimCurveBase doesn't have a create function that takes an object,
        // so had to remove any calls to CreateObject(FbxObject, name)
        public override void DoTestDisposeDestroy (bool canDestroyNonRecursive)
        {
            FbxAnimCurveBase a;

            // Test destroying just yourself.
            a = CreateObject ("a");
            a.Destroy ();
            Assert.That(() => a.GetName(), Throws.Exception.TypeOf<System.ArgumentNullException>());

            // Test destroying just yourself, explicitly non-recursive.
            a = CreateObject ("a");
            a.Destroy (false);
            Assert.That(() => a.GetName(), Throws.Exception.TypeOf<System.ArgumentNullException>());

            // Test destroying recursively.
            a = CreateObject ("a");
            a.Destroy(true);
            Assert.That(() => a.GetName(), Throws.Exception.TypeOf<System.ArgumentNullException>());

            // Test disposing. TODO: how to test that a was actually destroyed?
            a = CreateObject("a");
            a.Dispose();
            Assert.That(() => a.GetName(), Throws.Exception.TypeOf<System.NullReferenceException>());

            // Test that the using statement works.
            using (a = CreateObject ("a")) {
                a.GetName (); // works here, throws outside using
            }
            Assert.That(() => a.GetName(), Throws.Exception.TypeOf<System.NullReferenceException>());

            // Test that if we try to use an object after Destroy()ing its
            // manager, the object was destroyed as well.
            a = CreateObject("a");
            Assert.IsNotNull (a);
            Manager.Destroy();
            Assert.That (() => { a.GetName (); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }
    }

    public class FbxAnimCurveTest : Base<FbxAnimCurve>
    {
        [Test]
        public override void TestCreate ()
        {
            // have to override the TestCreate as FbxAnimCurve doesn't have a create function that takes an object,
            // but it does have a Create function that takes an FbxScene

            var obj = CreateObject("MyObject");
            Assert.IsInstanceOf<FbxAnimCurve> (obj);
            Assert.AreEqual(Manager, obj.GetFbxManager());

            using(var manager2 = FbxManager.Create()) {
                var obj2 = CreateObject(manager2, "MyOtherObject");
                Assert.AreEqual(manager2, obj2.GetFbxManager());
                Assert.AreNotEqual(Manager, obj2.GetFbxManager());
            }

            var obj3 = FbxAnimCurve.Create(FbxScene.Create(Manager, "scene"), "MySubObject");
            Assert.AreEqual(Manager, obj3.GetFbxManager());

            // Test with a null manager or container. Should throw.
            Assert.That (() => { CreateObject((FbxManager)null, "MyObject"); }, Throws.Exception.TypeOf<System.NullReferenceException>());
            Assert.That (() => { FbxAnimCurve.Create((FbxScene)null, "MyObject"); }, Throws.Exception.TypeOf<System.NullReferenceException>());

            // Test with a null string. Should work.
            Assert.IsNotNull(CreateObject((string)null));

            // Test with a destroyed manager. Should throw.
            var mgr = FbxManager.Create();
            mgr.Destroy();
            Assert.That (() => { CreateObject(mgr, "MyObject"); }, Throws.Exception.TypeOf<System.ArgumentNullException>());

            // Test with a disposed manager. Should throw.
            mgr = FbxManager.Create();
            mgr.Dispose();
            Assert.That (() => { CreateObject(mgr, "MyObject"); }, Throws.Exception.TypeOf<System.NullReferenceException>());
        }

        // FbxAnimCurve doesn't have a create function that takes an object,
        // so had to remove any calls to CreateObject(FbxObject, name)
        public override void DoTestDisposeDestroy (bool canDestroyNonRecursive)
        {
            FbxAnimCurve a;

            // Test destroying just yourself.
            a = CreateObject ("a");
            a.Destroy ();
            Assert.That(() => a.GetName(), Throws.Exception.TypeOf<System.ArgumentNullException>());

            // Test destroying just yourself, explicitly non-recursive.
            a = CreateObject ("a");
            a.Destroy (false);
            Assert.That(() => a.GetName(), Throws.Exception.TypeOf<System.ArgumentNullException>());

            // Test destroying recursively.
            a = CreateObject ("a");
            a.Destroy(true);
            Assert.That(() => a.GetName(), Throws.Exception.TypeOf<System.ArgumentNullException>());

            // Test disposing. TODO: how to test that a was actually destroyed?
            a = CreateObject("a");
            a.Dispose();
            Assert.That(() => a.GetName(), Throws.Exception.TypeOf<System.NullReferenceException>());

            // Test that the using statement works.
            using (a = CreateObject ("a")) {
                a.GetName (); // works here, throws outside using
            }
            Assert.That(() => a.GetName(), Throws.Exception.TypeOf<System.NullReferenceException>());

            // Test that if we try to use an object after Destroy()ing its
            // manager, the object was destroyed as well.
            a = CreateObject("a");
            Assert.IsNotNull (a);
            Manager.Destroy();
            Assert.That (() => { a.GetName (); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
        }

        [Test]
        public void TestBasics ()
        {
            using (FbxAnimCurve curve = CreateObject ("curve")) {
                // test KeyModifyBegin (make sure it doesn't crash)
                curve.KeyModifyBegin ();

                // test KeyAdd
                int last = 0;
                int index = curve.KeyAdd (new FbxTime (5), ref last);
                Assert.GreaterOrEqual (index, 0);

                // test KeyAdd null FbxTime
                Assert.That (() => { curve.KeyAdd(null); }, Throws.Exception.TypeOf<System.ArgumentNullException>());
                Assert.That (() => { curve.KeyAdd(null, ref last); }, Throws.Exception.TypeOf<System.ArgumentNullException>());

                // test KeySet
                curve.KeySet(index, new FbxTime(), 5);
                Assert.AreEqual (5, curve.KeyGetValue (index));

                // make sure none of the variations crash
                curve.KeySet (index, new FbxTime (), 5, FbxAnimCurveDef.EInterpolationType.eInterpolationConstant
                );
                curve.KeySet (index, new FbxTime (), 0, 
                    FbxAnimCurveDef.EInterpolationType.eInterpolationCubic,
                    FbxAnimCurveDef.ETangentMode.eTangentAuto
                );
                curve.KeySet (index, new FbxTime (), 0,
                    FbxAnimCurveDef.EInterpolationType.eInterpolationCubic,
                    FbxAnimCurveDef.ETangentMode.eTangentAuto, 4
                );
                curve.KeySet (index, new FbxTime (), 0,
                    FbxAnimCurveDef.EInterpolationType.eInterpolationCubic,
                    FbxAnimCurveDef.ETangentMode.eTangentAuto,
                    0, 3);
                curve.KeySet (index, new FbxTime (), 0,
                    FbxAnimCurveDef.EInterpolationType.eInterpolationCubic,
                    FbxAnimCurveDef.ETangentMode.eTangentAuto,
                    0, 0, FbxAnimCurveDef.EWeightedMode.eWeightedAll);
                curve.KeySet (index, new FbxTime (), 0,
                    FbxAnimCurveDef.EInterpolationType.eInterpolationCubic,
                    FbxAnimCurveDef.ETangentMode.eTangentAuto,
                    0, 0, FbxAnimCurveDef.EWeightedMode.eWeightedAll, 0);
                curve.KeySet (index, new FbxTime (), 0,
                    FbxAnimCurveDef.EInterpolationType.eInterpolationCubic,
                    FbxAnimCurveDef.ETangentMode.eTangentAuto,
                    0, 0, FbxAnimCurveDef.EWeightedMode.eWeightedAll, 0, 0);
                curve.KeySet (index, new FbxTime (), 0,
                    FbxAnimCurveDef.EInterpolationType.eInterpolationCubic,
                    FbxAnimCurveDef.ETangentMode.eTangentAuto,
                    0, 0, FbxAnimCurveDef.EWeightedMode.eWeightedAll, 0, 0, 0);
                curve.KeySet (index, new FbxTime (), 0,
                    FbxAnimCurveDef.EInterpolationType.eInterpolationCubic,
                    FbxAnimCurveDef.ETangentMode.eTangentAuto,
                    0, 0, FbxAnimCurveDef.EWeightedMode.eWeightedAll, 0, 0, 0, 0);

                // test KeyModifyEnd (make sure it doesn't crash)
                curve.KeyModifyEnd ();
            }
        }

        [Test]
        public void TestKeyAddBeforeKeyModifyBegin()
        {
            using (FbxAnimCurve curve = CreateObject ("curve")) {
                curve.KeyAdd (new FbxTime ());
                curve.KeyModifyBegin ();
            }
        }

        [Test]
        public void TestKeyModifyEndBeforeKeyModifyBegin()
        {
            using (FbxAnimCurve curve = CreateObject ("curve")) {
                curve.KeyModifyEnd ();
                curve.KeyModifyBegin ();
            }
        }
    }
}