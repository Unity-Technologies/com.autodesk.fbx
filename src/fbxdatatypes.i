// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxDataType;

%rename("%s") FbxDataType::Create(const char* pName, const EFbxType pType);
%rename("%s") FbxDataType::Destroy;
%rename("%s") EFbxType;
#endif

%include "fbxsdk/core/fbxdatatypes.h"
%include "fbxsdk_csharp-fixed-headers/fbxpropertytypes.h"
