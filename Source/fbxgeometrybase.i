// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxGeometryBase;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxGeometryBase::InitControlPoints;
%rename("%s") FbxGeometryBase::GetControlPointsCount;
%rename("%s") FbxGeometryBase::GetControlPointAt;
%rename("%s") FbxGeometryBase::CreateElementNormal;
%rename("%s") FbxGeometryBase::CreateElementTangent;

#endif

/* Prevent a crash when setting a negative index. */
%ignore FbxGeometryBase::SetControlPointAt;
%rename("SetControlPointAt") FbxGeometryBase::SetControlPointChecked;
%extend FbxGeometryBase {
  void SetControlPointChecked(const FbxVector4& pCtrlPoint, int pIndex)
  {
    if (pIndex < 0) {
      SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentOutOfRangeException, "", "pIndex");
      return;
    }
    $self->SetControlPointAt(pCtrlPoint, pIndex);
  }
}

/* Prevent a crash when getting a negative index. */
%ignore FbxGeometryBase::GetControlPointAt;
%rename("GetControlPointAt") FbxGeometryBase::GetControlPointChecked;
%extend FbxGeometryBase {
  FbxVector4 GetControlPointChecked(int pIndex)
  {
    if (pIndex < 0) {
    // Out of bounds returns FbxVector4(0,0,0). FBX code crashes with
    // index < 0. Don't crash and return the documented value
      return FbxVector4(0,0,0);
    }
    return $self->GetControlPointAt(pIndex);
  }
}

%include "fbxsdk/scene/geometry/fbxgeometrybase.h"
