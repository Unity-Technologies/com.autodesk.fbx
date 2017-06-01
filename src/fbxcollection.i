// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxCollection;

%rename("%s") FbxCollection::Clear;
%rename("%s") FbxCollection::AddMember;
%rename("%s") FbxCollection::GetAnimLayerMember;
%rename("%s") FbxCollection::GetMemberCount() const;

%extend FbxCollection{
    FbxAnimLayer* GetAnimLayerMember(int pIndex = 0) const {
        return $self->GetMember<FbxAnimLayer>(pIndex);
    }
}

#endif

%include "fbxsdk/scene/fbxcollection.h"
