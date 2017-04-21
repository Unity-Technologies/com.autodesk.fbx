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

// add some bounds checking to SetControlPointAt
%csmethodmodifiers FbxGeometryBase::SetControlPointAt "private";
%rename(SetControlPointAt_private) FbxGeometryBase::SetControlPointAt(const FbxVector4& pCtrlPoint, int pIndex);
%rename("%s") FbxGeometryBase::SetControlPointAt_private;

%extend FbxGeometryBase {
  %proxycode %{
  public virtual void SetControlPointAt(FbxVector4 pCtrlPoint, int pIndex)
  {
    if(pIndex < 0 || pIndex >= this.GetControlPointsCount()){
        throw new System.IndexOutOfRangeException();
    }
    else{
        SetControlPointAt_private(pCtrlPoint, pIndex);
    }
  }
  %}
}

#endif

%include "fbxsdk/scene/geometry/fbxgeometrybase.h"
