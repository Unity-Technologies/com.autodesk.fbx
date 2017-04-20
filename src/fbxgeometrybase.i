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
%rename("%s") FbxGeometryBase::GetControlPointAt;
%rename("%s") FbxGeometryBase::SetControlPointAt(const FbxVector4& pCtrlPoint, int pIndex);
#endif

%include "fbxsdk/scene/geometry/fbxgeometrybase.h"
