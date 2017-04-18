// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxProperty;

%apply bool *OUTPUT { bool *pWasFound }
%rename("%s") FbxProperty::Create(FbxObject* pObject, const FbxDataType& pDataType, const char* pName, const char* pLabel="", bool pCheckForDup=true, bool* pWasFound=NULL);

%rename("%s") FbxProperty::Destroy;
%rename("%s") FbxProperty::GetName; // TODO: use GetNameAsCStr instead?
#endif

%include "fbxsdk/core/fbxproperty.h"
%include "fbxdatatypes.i"
