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
%rename("%s") FbxCollection::RemoveMember;
%rename("%s") FbxCollection::GetMember;
%ignore FbxCollection::GetMember(const FbxCriteria&, int) const;
%ignore FbxCollection::GetMember(const FbxCriteria&) const;
%rename("%s") FbxCollection::IsMember;
%rename("%s") FbxCollection::GetAnimLayerMember;
%rename("%s") FbxCollection::FindMemberObject;
%rename("%s") FbxCollection::GetMemberObject;
%rename("%s") FbxCollection::GetMemberCount() const;

// %template(FindMemberFbxObject) FbxCollection::FindMember<FbxObject>;
// %template(FindMemberFbxObject) FbxCollection::GetMember<FbxObject>;

%extend FbxCollection{
    /*
     * GetMember returns an FbxObject, but we need to get an object of
     * class FbxAnimLayer so that we can call methods on it.
     * TODO: (UNI-19185) Make it possible to downcast from FbxObject.
     */
    FbxAnimLayer* GetAnimLayerMember(int pIndex = 0) const {
        return $self->GetMember<FbxAnimLayer>(pIndex);
    }

    FbxObject* GetMemberObject(int pIndex = 0) const 
    {
        return $self->GetMember<FbxObject>(pIndex);
    }
    FbxObject* FindMemberObject(const char* pName) const 
    {
        return $self->FindMember<FbxObject>(pName);
    }
}

#endif

%include "fbxsdk/scene/fbxcollection.h"
