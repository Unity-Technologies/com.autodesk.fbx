// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxTransform;
%declare_static_class(FbxTransform);
%rename("%s") FbxTransform::EInheritType;

%include "fbxsdk/core/math/fbxtransforms.h"