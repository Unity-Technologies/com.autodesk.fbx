// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxClassId;

%rename("%s") FbxClassId::FbxClassId;
%rename("%s") FbxClassId::Create;
%rename("%s") FbxClassId::Is;
%rename("%s") FbxClassId::GetName;
%rename("%s") FbxClassId::IsValid;
%rename("%s") FbxClassId::GetParent;


// also IS, getName, and others.
%include "fbxsdk/core/fbxclassid.h"
