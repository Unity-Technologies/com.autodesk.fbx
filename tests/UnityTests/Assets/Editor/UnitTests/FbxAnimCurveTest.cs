// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

using NUnit.Framework;
using System.Collections.Generic;
using FbxSdk;

namespace UnitTests
{
    public class FbxAnimCurveTest : Base<FbxAnimCurve>
    {
        Dictionary<FbxManager, FbxScene> m_scenes = new Dictionary<FbxManager, FbxScene>();

        public override FbxAnimCurve CreateObject(FbxManager mgr, string name = "") {
            if (mgr == null) { throw new System.NullReferenceException(); }

            /* Creating in a manager doesn't work for AnimCurves, but for the benefit of
               testing, just fudge it by creating a scene for the manager. */
            FbxScene scene;
            if (!m_scenes.TryGetValue(mgr, out scene)) {
                scene = FbxScene.Create(mgr, "__testscene");
                m_scenes.Add(mgr, scene);
            }
            return FbxAnimCurve.Create(scene, name);
        }

        public override FbxAnimCurve CreateObject(FbxObject container, string name = "") {
            if (container == null) { throw new System.NullReferenceException(); }

            if (container is FbxScene) {
                /* Probably should have cast to a scene already... but ok. */
                return FbxAnimCurve.Create((FbxScene)container, name);
            } else {
                /* This create call doesn't do what you want. Use the manager's scene instead. */
                return CreateObject(container.GetFbxManager(), name);
            }
        }

        [Test]
        public void TestBasics ()
        {
            var scene = FbxScene.Create(Manager, "scene");
            using (FbxAnimCurve curve = FbxAnimCurve.Create(scene, "curve")) {
                // test KeyModifyBegin (make sure it doesn't crash)
                curve.KeyModifyBegin ();

                // test KeyAdd
                int last = 0;
                int index = curve.KeyAdd (FbxTime.FromFrame (5), ref last);
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

            // Also test that the AnimCurveBase can't be created.
            Assert.That(() => FbxAnimCurveBase.Create(Manager, ""), Throws.Exception.TypeOf<System.NotImplementedException>());
            Assert.That(() => FbxAnimCurveBase.Create(FbxObject.Create(Manager, ""), ""), Throws.Exception.TypeOf<System.NotImplementedException>());
        }

        [Test]
        public override void TestDisposeDestroy()
        {
            // Because we can't build a recursive structure of anim curves,
            // this test is much simpler than the usual FbxObject test.
            var curve = CreateObject("a");
            DisposeTester.TestDispose(curve);
            using (CreateObject("b"));

            curve = CreateObject("c");
            curve.Destroy();
            Assert.That(() => curve.GetName(), Throws.Exception.TypeOf<System.ArgumentNullException>());

            // we can't destroy recursively, but we still get the flag
            curve = CreateObject("d");
            curve.Destroy(false);
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

    public class FbxAnimCurveDefTest /* testing a static class, so we can't derive from TestBase */
    {
#if ENABLE_COVERAGE_TEST
        [Test]
        public virtual void TestCoverage() { CoverageTester.TestCoverage(typeof(FbxAnimCurveDef), this.GetType()); }
#endif

        [Test]
        public void TestBasics()
        {
            Assert.AreEqual(0.0, FbxAnimCurveDef.sDEFAULT_VELOCITY);
            Assert.AreNotEqual(0.0, FbxAnimCurveDef.sDEFAULT_WEIGHT);
            Assert.That(FbxAnimCurveDef.sMIN_WEIGHT, Is.GreaterThan(0.0f));
            Assert.That(FbxAnimCurveDef.sMAX_WEIGHT, Is.LessThan(1.0f));
        }
    }
}
