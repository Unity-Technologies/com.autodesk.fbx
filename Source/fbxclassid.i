// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxClassId;
// %rename("%s", %$isclass) FbxClassIdInfo* ;

%rename("%s") FbxClassId::FbxClassId;
%rename("%s") FbxClassId::Create;
%rename("%s") FbxClassId::Is;
%rename("%s") FbxClassId::GetName;
%rename("%s") FbxClassId::IsValid;
%rename("%s") FbxClassId::GetParent;

// %rename("%s") FbxClassId::GetClassIdInfo;
/* Getting the matrix; C# would usually return it, not stuff an out reference.
 * If there's performance issues, we can %apply the OUTPUT instead of ignoring
 * this. */
%rename("GetClassIdInfo") FbxClassId::GetClassIdInt;
%extend FbxClassId {
  FbxObject* GetClassIdInt() {
    return (FbxObject*)$self->GetClassIdInfo();
  }
}

// also IS, getName, and others.
%include "fbxsdk/core/fbxclassid.h"
