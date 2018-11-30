// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// FbxTransform
%rename("%s", %$isclass) FbxTransform;
%declare_static_class(FbxTransform);
%rename("%s") FbxTransform::EInheritType;


// FbxLimits
// This is a simple class that you can use independently of anything.
// Not likely to be important to optimize but it would be easy to reimplement
// it in C# and avoid the extra allocation.
%rename("%s", %$isclass) FbxLimits;
%rename("%s") FbxLimits::FbxLimits;
%rename("%s") FbxLimits::~FbxLimits;

// Optimization potential: grab the mMask directly to get/set the active status
// all in one P/Invoke (rather than 7).
//
// Note: Not exposing GetMinActive/GetMaxActive because then we need to deal
// with ref arguments.
%rename("%s") FbxLimits::GetActive;
%rename("%s") FbxLimits::GetMinXActive;
%rename("%s") FbxLimits::GetMinYActive;
%rename("%s") FbxLimits::GetMinZActive;
%rename("%s") FbxLimits::GetMaxXActive;
%rename("%s") FbxLimits::GetMaxYActive;
%rename("%s") FbxLimits::GetMaxZActive;
%rename("%s") FbxLimits::GetMin;
%rename("%s") FbxLimits::GetMax;

%rename("%s") FbxLimits::SetActive;
%rename("%s") FbxLimits::SetMinActive;
%rename("%s") FbxLimits::SetMinXActive;
%rename("%s") FbxLimits::SetMinYActive;
%rename("%s") FbxLimits::SetMinZActive;
%rename("%s") FbxLimits::SetMaxActive;
%rename("%s") FbxLimits::SetMaxXActive;
%rename("%s") FbxLimits::SetMaxYActive;
%rename("%s") FbxLimits::SetMaxZActive;
%rename("%s") FbxLimits::SetMin;
%rename("%s") FbxLimits::SetMax;

%rename("%s") FbxLimits::GetAnyMinMaxActive;
%rename("%s") FbxLimits::Apply;

%include "fbxsdk/core/math/fbxtransforms.h"
