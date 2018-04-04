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

	public class FbxConstraintTest : FbxConstraintTestBase<FbxConstraint>{}

	public class FbxConstraintAimTest : FbxConstraintTestBase<FbxConstraintAim>
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
		public void TestAddConstraintSource(){
			using (var constraint = FbxConstraintAim.Create (Manager, "aimConstraint")) {
				Assert.That(() => constraint.AddConstraintSource (null), Throws.Exception.TypeOf<System.NullReferenceException>());
				Assert.That (constraint.GetConstraintSourceCount(), Is.EqualTo(0));

				var fbxNode = FbxNode.Create (Manager, "rootnode");

				constraint.AddConstraintSource (fbxNode);
				Assert.That (constraint.GetConstraintSource (0), Is.EqualTo (fbxNode));
				Assert.That (constraint.GetConstraintSourceCount (), Is.EqualTo (1));

				fbxNode = FbxNode.Create (Manager, "node2");
				constraint.AddConstraintSource (fbxNode, 2);
				Assert.That (constraint.GetConstraintSource (1), Is.EqualTo (fbxNode));
				Assert.That (constraint.GetConstraintSourceCount (), Is.EqualTo (2));
			}
		}

		[Test]
		public void TestSetConstrainedObject(){
			using (var constraint = FbxConstraintAim.Create (Manager, "aimConstraint")) {
				Assert.That(() => constraint.SetConstrainedObject (null), Throws.Exception.TypeOf<System.NullReferenceException>());

				var fbxNode = FbxNode.Create (Manager, "rootnode");

				constraint.SetConstrainedObject (fbxNode);
				Assert.That (constraint.GetConstrainedObject (), Is.EqualTo (fbxNode));
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
}