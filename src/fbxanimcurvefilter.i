// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
// Unignore class
%rename("%s", %$isclass) FbxAnimCurveFilterUnroll;
%rename("%s") FbxAnimCurveFilterUnroll::FbxAnimCurveFilterUnroll();
%rename("%s") FbxAnimCurveFilterUnroll::~FbxAnimCurveFilterUnroll;

%rename("%s") FbxAnimCurveFilterUnroll::NeedApply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus = ((void *)0));
%rename("%s") FbxAnimCurveFilterUnroll::Apply(FbxAnimCurveNode& pCurveNode, FbxStatus* pStatus = ((void *)0));
%rename("%s") FbxAnimCurveFilterUnroll::Reset;

%include "fbxsdk/scene/animation/fbxanimcurvefilters.h"
