// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore class... but nothing in it.
%rename("%s", %$isclass) FbxAnimCurveBase;

// FbxAnimCurveBase is abstract, so you can't create it.
// Throw exceptions if we try.
%ignore FbxAnimCurveBase::Create(FbxManager*, const char*);
%ignore FbxAnimCurveBase::Create(FbxObject*, const char*);
%extend FbxAnimCurveBase { %proxycode %{
  public static new FbxAnimCurveBase Create(FbxManager pManager, string pName) {
    throw new System.NotImplementedException("FbxAnimCurveBase is abstract; create FbxAnimCurve instead");
  }
  public static new FbxAnimCurveBase Create(FbxObject pContainer, string pName) {
    throw new System.NotImplementedException("FbxAnimCurveBase is abstract; create FbxAnimCurve instead");
  }
%} }

%rename("%s") FbxAnimCurveBase::KeyGetTime;
%rename("%s") FbxAnimCurveBase::KeyGetCount;

%include "fbxsdk/scene/animation/fbxanimcurvebase.h"
