// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
using NUnit.Framework;
using Unity.FbxSdk;

namespace Unity.FbxSdk.UnitTests
{
	public abstract class FbxConstraintTestBase<T> : Base<T> where T : FbxConstraint
	{
		protected virtual FbxConstraint.EType ConstraintType { get { return FbxConstraint.EType.eUnknown; }}

		[Test]
		public virtual void TestBasics(){
			T constraint = CreateObject ("constraint");

			TestGetter (constraint.Active);
			TestGetter (constraint.Lock);
			TestGetter (constraint.Weight);
			TestGetter (constraint.GetConstrainedObject ());
			TestGetter (constraint.GetConstraintSource (-1));
			TestGetter (constraint.GetConstraintSource (0));
			TestGetter (constraint.GetSourceWeight (FbxNode.Create (Manager, "Node")));
			Assert.That (() => constraint.GetSourceWeight (null), Throws.Exception.TypeOf<System.NullReferenceException>());
			Assert.That (constraint.GetConstraintSourceCount (), Is.EqualTo (0));
			Assert.That (constraint.GetConstraintType (), Is.EqualTo (ConstraintType));
		}
	}

	/// <summary>
	/// For testing functions that classes that derive from FbxConstraint share, but are not implemented in FbxConstraint.
	/// </summary>
	public abstract class FbxConstraintDescendantTestBase<T> : FbxConstraintTestBase<T> where T : FbxConstraint{
		static System.Reflection.MethodInfo s_AddConstraintSource;
		static System.Reflection.MethodInfo s_AddConstraintSourceDouble;
		static System.Reflection.MethodInfo s_SetConstrainedObject;

		static FbxConstraintDescendantTestBase() {
			s_AddConstraintSource = typeof(T).GetMethod("AddConstraintSource", new System.Type[] { typeof(FbxObject) });
			s_AddConstraintSourceDouble = typeof(T).GetMethod("AddConstraintSource", new System.Type[] { typeof(FbxObject), typeof(double) });
			s_SetConstrainedObject = typeof(T).GetMethod ("SetConstrainedObject", new System.Type[] { typeof(FbxObject) });

			#if ENABLE_COVERAGE_TEST
			// Register the calls we make through reflection.
			if(s_AddConstraintSource != null){
				var addConstraintSource = typeof(FbxConstraintDescendantTestBase<T>).GetMethod("AddConstraintSource");
				CoverageTester.RegisterReflectionCall(addConstraintSource, s_AddConstraintSource);
			}
			if(s_AddConstraintSourceDouble != null){
				var addConstraintSourceDouble = typeof(FbxConstraintDescendantTestBase<T>).GetMethod("AddConstraintSourceDouble");
				CoverageTester.RegisterReflectionCall(addConstraintSourceDouble, s_AddConstraintSourceDouble);
			}
			if(s_SetConstrainedObject != null){
				var setConstrainedObject = typeof(FbxConstraintDescendantTestBase<T>).GetMethod("SetConstrainedObject");
				CoverageTester.RegisterReflectionCall(setConstrainedObject, s_SetConstrainedObject);
			}
			#endif
		}

		public void AddConstraintSourceDouble(T instance, FbxObject obj, double weight){
			Invoker.Invoke (s_AddConstraintSourceDouble, instance, obj, weight);
		}

		public void AddConstraintSource(T instance, FbxObject obj){
			Invoker.Invoke (s_AddConstraintSource, instance, obj);
		}

		public void SetConstrainedObject(T instance, FbxObject obj){
			Invoker.Invoke (s_SetConstrainedObject, instance, obj);
		}

		[Test]
		public virtual void TestAddConstraintSource(){
			using (var constraint = CreateObject("constraint")) {
				Assert.That(() => AddConstraintSource (constraint, null), Throws.Exception.TypeOf<System.NullReferenceException>());
				Assert.That (constraint.GetConstraintSourceCount(), Is.EqualTo(0));

				var fbxNode = FbxNode.Create (Manager, "rootnode");

				AddConstraintSource (constraint, fbxNode);
				Assert.That (constraint.GetConstraintSource (0), Is.EqualTo (fbxNode));
				Assert.That (constraint.GetConstraintSourceCount (), Is.EqualTo (1));

				fbxNode = FbxNode.Create (Manager, "node2");
				AddConstraintSourceDouble (constraint, fbxNode, 2.0);
				Assert.That (constraint.GetConstraintSource (1), Is.EqualTo (fbxNode));
				Assert.That (constraint.GetConstraintSourceCount (), Is.EqualTo (2));
			}
		}

		[Test]
		public virtual void TestSetConstrainedObject(){
			if (ConstraintType == FbxConstraint.EType.eUnknown) {
				return;
			}

			using (var constraint = CreateObject("constraint")) {
				Assert.That(() => SetConstrainedObject (constraint, null), Throws.Exception.TypeOf<System.NullReferenceException>());

				var fbxNode = FbxNode.Create (Manager, "rootnode");

				SetConstrainedObject (constraint, fbxNode);
				Assert.That (constraint.GetConstrainedObject (), Is.EqualTo (fbxNode));
			}
		}
	}


	public class FbxConstraintTest : FbxConstraintTestBase<FbxConstraint>{}

	public class FbxConstraintAimTest : FbxConstraintDescendantTestBase<FbxConstraintAim>
	{
		protected override FbxConstraint.EType ConstraintType {
			get {
				return FbxConstraint.EType.eAim;
			}
		}

		[Test]
		public void TestGetters(){
			using (var constraint = FbxConstraintAim.Create (Manager, "aimConstraint")) {
				TestGetter (constraint.AffectX);
				TestGetter (constraint.AffectY);
				TestGetter (constraint.AffectZ);
				TestGetter (constraint.AimVector);
				TestGetter (constraint.RotationOffset);
				TestGetter (constraint.UpVector);
				TestGetter (constraint.WorldUpType);
				TestGetter (constraint.WorldUpVector);
			}
		}

		[Test]
		public void TestWorldUpObject(){
			using (var constraint = FbxConstraintAim.Create (Manager, "aimConstraint")) {
				Assert.That(() => constraint.SetWorldUpObject (null), Throws.Exception.TypeOf<System.NullReferenceException>());

				var fbxNode = FbxNode.Create (Manager, "rootnode");

				constraint.SetWorldUpObject (fbxNode);
				Assert.That (constraint.GetWorldUpObject (), Is.EqualTo (fbxNode));
			}
		}
	}

	public class FbxConstraintParentTest : FbxConstraintDescendantTestBase<FbxConstraintParent>{
		protected override FbxConstraint.EType ConstraintType {
			get {
				return FbxConstraint.EType.eParent;
			}
		}

		[Test]
		public void TestGetters(){
			using (var constraint = FbxConstraintParent.Create (Manager, "pConstraint")) {
				TestGetter (constraint.AffectRotationX);
				TestGetter (constraint.AffectRotationY);
				TestGetter (constraint.AffectRotationZ);
				TestGetter (constraint.AffectScalingX);
				TestGetter (constraint.AffectScalingY);
				TestGetter (constraint.AffectScalingZ);
				TestGetter (constraint.AffectTranslationX);
				TestGetter (constraint.AffectTranslationY);
				TestGetter (constraint.AffectTranslationZ);
			}
		}
	}

	public class FbxConstraintPositionTest : FbxConstraintDescendantTestBase<FbxConstraintPosition> {
		protected override FbxConstraint.EType ConstraintType {
			get {
				return FbxConstraint.EType.ePosition;
			}
		}

		[Test]
		public void TestGetters(){
			using (var constraint = FbxConstraintPosition.Create (Manager, "posConstraint")) {
				TestGetter (constraint.AffectX);
				TestGetter (constraint.AffectY);
				TestGetter (constraint.AffectZ);
				TestGetter (constraint.Translation);
			}
		}

		[Test]
		public override void TestAddConstraintSource(){
			// overriding implementation because FbxConstraintPosition also has a RemoveConstraintSource() function

			using (var constraint = FbxConstraintPosition.Create (Manager, "pConstraint")) {
				Assert.That(() => constraint.AddConstraintSource (null), Throws.Exception.TypeOf<System.NullReferenceException>());
				Assert.That (constraint.GetConstraintSourceCount(), Is.EqualTo(0));

				var fbxNode = FbxNode.Create (Manager, "rootnode");

				constraint.AddConstraintSource (fbxNode);
				Assert.That (constraint.GetConstraintSource (0), Is.EqualTo (fbxNode));
				Assert.That (constraint.GetConstraintSourceCount (), Is.EqualTo (1));

				var fbxNode2 = FbxNode.Create (Manager, "node2");
				constraint.AddConstraintSource (fbxNode2, 2);
				Assert.That (constraint.GetConstraintSource (1), Is.EqualTo (fbxNode2));
				Assert.That (constraint.GetConstraintSourceCount (), Is.EqualTo (2));

				Assert.That(() => constraint.RemoveConstraintSource (null), Throws.Exception.TypeOf<System.NullReferenceException>());

				constraint.RemoveConstraintSource (fbxNode);
				Assert.That (constraint.GetConstraintSourceCount (), Is.EqualTo (1));
				Assert.That (constraint.GetConstraintSource (0), Is.EqualTo (fbxNode2));
			}
		}
	}

	public class FbxConstraintRotationTest : FbxConstraintDescendantTestBase<FbxConstraintRotation>{
		protected override FbxConstraint.EType ConstraintType {
			get {
				return FbxConstraint.EType.eRotation;
			}
		}

		[Test]
		public void TestGetters(){
			using (var constraint = FbxConstraintRotation.Create (Manager, "rConstraint")) {
				TestGetter (constraint.AffectX);
				TestGetter (constraint.AffectY);
				TestGetter (constraint.AffectZ);
				TestGetter (constraint.Rotation);
			}
		}
	}

	public class FbxConstraintScaleTest : FbxConstraintDescendantTestBase<FbxConstraintScale>{
		protected override FbxConstraint.EType ConstraintType {
			get {
				return FbxConstraint.EType.eScale;
			}
		}

		[Test]
		public void TestGetters(){
			using (var constraint = FbxConstraintScale.Create (Manager, "sConstraint")) {
				TestGetter (constraint.AffectX);
				TestGetter (constraint.AffectY);
				TestGetter (constraint.AffectZ);
				TestGetter (constraint.Scaling);
			}
		}
	}
}