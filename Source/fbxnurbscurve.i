// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s", %$isclass) FbxNurbsCurve;
// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxNurbsCurve::Create(FbxManager* pManager, const char* pName);
%rename("%s") FbxNurbsCurve::Create(FbxObject* pContainer, const char* pName);

/* Attributes */
%rename("%s") FbxNurbsCurve::EDimension;
%rename("%s") FbxNurbsCurve::GetKnotCount;
%rename("%s") FbxNurbsCurve::GetStep;
%rename("%s") FbxNurbsCurve::GetDimension;
%rename("%s") FbxNurbsCurve::GetSpanCount;
%rename("%s") FbxNurbsCurve::IsPolyline;
%rename("%s") FbxNurbsCurve::IsBezier;
%rename("%s") FbxNurbsCurve::GetKnotVector;
%rename("%s") FbxNurbsCurve::GetKnotVectorAtIndex;
%rename("%s") FbxNurbsCurve::GetOrder;
%rename("%s") FbxNurbsCurve::IsRational;

%rename("%s") FbxNurbsCurve::Allocate;
%rename("%s") FbxNurbsCurve::InitControlPoints;
%rename("%s") FbxNurbsCurve::SetOrder;
%rename("%s") FbxNurbsCurve::SetStep;
%rename("%s") FbxNurbsCurve::SetDimension;

%rename("%s") FbxNurbsCurve::SetKnotVectorAt;

#endif

%extend FbxNurbsCurve {
   double GetKnotVectorAtIndex( int pIndex ) const
   {
      double* aKnotArray = $self->GetKnotVector();
      double aKnot = aKnotArray[pIndex];
      return aKnot;
   }
   void SetKnotVectorAt( int pIndex, double aKnot )
   {
      double* aKnotArray = $self->GetKnotVector();
      aKnotArray[pIndex] = aKnot;
   }
}


/* The properties need to be marked immutable. 
%fbximmutable(FbxNode::LclTranslation);
%fbximmutable(FbxNode::LclRotation);
%fbximmutable(FbxNode::LclScaling);
%fbximmutable(FbxNode::VisibilityInheritance);
%fbximmutable(FbxNode::InheritType);*/

%include "fbxsdk/scene/geometry/fbxnurbscurve.h"
