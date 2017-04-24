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
%rename("%s") FbxAxisSystem::ConvertScene(FbxScene* pScene) const;

/* Get a bunch of constants. */
%rename("%s") FbxAxisSystem::MayaZUp;
%rename("%s") FbxAxisSystem::MayaYUp;
%rename("%s") FbxAxisSystem::Max;
%rename("%s") FbxAxisSystem::Motionbuilder;
%rename("%s") FbxAxisSystem::OpenGL;
%rename("%s") FbxAxisSystem::DirectX;
%rename("%s") FbxAxisSystem::Lightwave;

/* Get a couple of enums. But *not* the up/front vector, which we handle specially. */
%rename("%s") FbxAxisSystem::ECoordSystem;
%rename("%s") FbxAxisSystem::EPreDefinedAxisSystem;

/* Get two extended functions, and the non-extended GetCoorSystem. */
%rename("%s") FbxAxisSystem::GetUpVector();
%rename("%s") FbxAxisSystem::GetFrontVector();
%rename("%s") FbxAxisSystem::GetCoorSystem() const;

/* For equality we need this extended method, the rest is handled below. */
%rename("%s") FbxAxisSystem::GetHashCode;
#endif

/* The EUpVector and EFrontVector enums are funny: they actually have negative values,
 * but those aren't declared in the C++. Instead, when you get them you get a sign; and
 * when you set them you're supposed to multiply by the sign. That's weird even in C++, and
 * feels like unsafe code in C#, so let's clean it up.
*/
%ignore FbxAxisSystem::GetUpVector(int& pSign) const;
%ignore FbxAxisSystem::GetFrontVector(int& pSign) const;
%typemap(cstype) FbxAxisSystem::EUpVector "EUpVector";
%typemap(cstype) FbxAxisSystem::EFrontVector "EFrontVector";
%typemap(imtype) FbxAxisSystem::EUpVector "int";
%typemap(imtype) FbxAxisSystem::EFrontVector "int";
%typemap(ctype) FbxAxisSystem::EUpVector "int";
%typemap(ctype) FbxAxisSystem::EFrontVector "int";
%typemap(csout) FbxAxisSystem::EUpVector { return (EUpVector)$imcall; }
%typemap(csout) FbxAxisSystem::EFrontVector { return (EFrontVector)$imcall; }
%ignore FbxAxisSystem::EUpVector;
%ignore FbxAxisSystem::EFrontVector;
%extend FbxAxisSystem {
  %proxycode %{
  public enum EUpVector {
    eXAxis = 1, eXAxisDown = -1,
    eYAxis = 2, eYAxisDown = -2,
    eZAxis = 3, eZAxisDown = -3,
  };
  public enum EFrontVector {
    eParityOdd = 1, eParityOddNegative = -1,
    eParityEven = 2, eParityEvenNegative = -2,
  };
  %}
  EUpVector GetUpVector() {
    int sign;
    FbxAxisSystem::EUpVector up = $self->GetUpVector(sign);
    return (FbxAxisSystem::EUpVector) (sign * (int)up);
  }
  EFrontVector GetFrontVector() {
    int sign;
    FbxAxisSystem::EFrontVector up = $self->GetFrontVector(sign);
    return (FbxAxisSystem::EFrontVector) (sign * (int)up);
  }
}

/* Getting the matrix; C# would usually return it, not stuff an out reference.
 * If there's performance issues, we can %apply the OUTPUT instead of ignoring
 * this. */
%ignore FbxAxisSystem::GetMatrix(FbxAMatrix&);
%extend FbxAxisSystem {
  FbxAMatrix GetMatrix() {
    FbxAMatrix mx;
    $self->GetMatrix(mx);
    return mx;
  }
}

/* This is documented as being legacy, so don't include it (even though it's not deprecated). */
%ignore FbxAxisSystem::ConvertScene(FbxScene* pScene, FbxNode* pFbxRoot) const;


/* Handle equality. */
%rename("Equals") FbxAxisSystem::operator==;
%ignore FbxAxisSystem::operator!=;
%ignore FbxAxisSystem::operator=;
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
