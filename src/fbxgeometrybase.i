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

#endif

/* Prevent a crash when setting a negative index. */
%ignore FbxGeometryBase::SetControlPointAt;
%rename("SetControlPointAt") FbxGeometryBase::SetControlPointChecked;
%extend FbxGeometryBase {
  void SetControlPointChecked(const FbxVector4& pCtrlPoint, int pIndex)
  {
    if (pIndex < 0) {
      SWIG_CSharpSetPendingException(SWIG_CSharpIndexOutOfRangeException, "");
      return;
    }
    $self->SetControlPointAt(pCtrlPoint, pIndex);
  }
}

%include "fbxsdk/scene/geometry/fbxgeometrybase.h"
