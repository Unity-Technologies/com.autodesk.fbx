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
%rename("%s") FbxNurbsCurve::EType;
%rename("%s") FbxNurbsCurve::GetKnotCount;
%rename("%s") FbxNurbsCurve::GetStep;
%rename("%s") FbxNurbsCurve::GetDimension;
%rename("%s") FbxNurbsCurve::GetSpanCount;
%rename("%s") FbxNurbsCurve::IsPolyline;
%rename("%s") FbxNurbsCurve::IsBezier;
%rename("%s") FbxNurbsCurve::GetKnotVectorAt;
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
   double GetKnotVectorAt( int pIndex ) const
   {
      if (pIndex < 0 || pIndex >= $self->GetKnotCount()){
          SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentOutOfRangeException, "Index out of range", "pIndex");
          return -1;
      }
      double* aKnotArray = $self->GetKnotVector();
      double aKnot = aKnotArray[pIndex];
      return aKnot;
   }
   void SetKnotVectorAt( int pIndex, double aKnot )
   {
      if (pIndex < 0 || pIndex >= $self->GetKnotCount()){
          SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentOutOfRangeException, "Index out of range", "pIndex");
          return;
      }
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
