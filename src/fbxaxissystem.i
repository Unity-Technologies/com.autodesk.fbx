// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxAxisSystem;
%clearnodefaultdtor FbxAxisSystem;
%rename("%s") FbxAxisSystem::~FbxAxisSystem();
%ignore FbxAxisSystem::FbxAxisSystem(EUpVector pUpVector, EFrontVector pFrontVector, ECoordSystem pCoorSystem);
%ignore FbxAxisSystem::FbxAxisSystem(const EPreDefinedAxisSystem);
%ignore FbxAxisSystem::FbxAxisSystem();
%rename("%s") FbxAxisSystem::MayaZUp;
%rename("%s") FbxAxisSystem::MayaYUp;
%rename("%s") FbxAxisSystem::Max;
%rename("%s") FbxAxisSystem::Motionbuilder;
%rename("%s") FbxAxisSystem::OpenGL;
%rename("%s") FbxAxisSystem::DirectX;
%rename("%s") FbxAxisSystem::Lightwave;
%rename("%s") FbxAxisSystem::GetHashCode;
#endif

%rename("Equals") FbxAxisSystem::operator==;
%define_generic_equality_functions(FbxAxisSystem);

%extend FbxAxisSystem {
  %csmethodmodifiers GetHashCode "public override";
  int GetHashCode() const {
    int upSign;
    FbxAxisSystem::EUpVector up;
    int frontSign;
    FbxAxisSystem::EFrontVector front;
    FbxAxisSystem::ECoordSystem side;
    up = $self->GetUpVector(upSign);
    front = $self->GetFrontVector(frontSign);
    side = $self->GetCoorSystem();

    /* All this data is only 48 choices, so it fits in 6 bits.
     * Set the high bit so that mod by different primes gives different answers
     * (e.g. to support cuckoo hashing) */
    uint32_t u;
    u = (upSign > 0) ? 0 : 1;
    u = (u << 2) | ((up == FbxAxisSystem::eXAxis) ? 0 : ((up == FbxAxisSystem::eYAxis) ? 1 : 2));
    u = (u << 1) | ((frontSign > 0) ? 0 : 1);
    u = (u << 1) | ((front == FbxAxisSystem::eParityEven) ? 0 : 1);
    u = (u << 1) | ((side == FbxAxisSystem::eRightHanded) ? 0 : 1);
    u |= 1u<<31;
    return (int)u;
  }
}


%include "fbxsdk/scene/fbxaxissystem.h"
