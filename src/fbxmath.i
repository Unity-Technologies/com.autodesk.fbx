// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
%reveal_all_start;

%{
  static inline int32_t GetHashCode(const double *data, int len) {
    uint64_t hash = 0;
    union {
      uint64_t u;
      double d;
    };
    for(int i = 0; i < len; ++i) {
      hash = (hash << 5) | (hash >> 59);
      d = data[i];
      hash ^= u;
    }
    return int32_t(hash) ^ int32_t(hash >> 32);
  }
%}

/******************************
 * FbxMatrix (not affine matrix)
 ******************************/
%apply FbxVector4& OUTPUT { FbxVector4& pTranslation, FbxVector4& pRotation, FbxVector4& pShearing, FbxVector4& pScaling }
%apply double& OUTPUT { double& pSign }

%ignore FbxMatrix::operator double* ();
%ignore FbxMatrix::operator const double* () const;
%ignore FbxMatrix::Double44;

%ignore FbxMatrix::operator=;
%ignore FbxMatrix::operator+=;
%ignore FbxMatrix::operator-=;
%ignore FbxMatrix::operator*=;
%ignore FbxMatrix::operator/=;

%define_unary_operator(FbxMatrix, -, Negate);
// Strangely, you can't scale a matrix in FBX SDK (not with public API -- it's hidden)
//%define_commutative_binary_operator(FbxMatrix, *, Scale, double);
//%define_binary_operator(FbxMatrix, /, InvScale, double);
%define_binary_operator(FbxMatrix, +, Add, FbxMatrix);
%define_binary_operator(FbxMatrix, -, Sub, FbxMatrix);
%define_binary_operator(FbxMatrix, *, Mul, FbxMatrix);
%define_equality_from_operator(FbxMatrix);
// This case is handled by the Double4x4 operator==.
%ignore FbxMatrix::operator==(const FbxAMatrix&) const;
%extend FbxMatrix {
  %csmethodmodifiers GetHashCode "public override";
  int GetHashCode() { return GetHashCode(*$self, 16); }
}

%include "fbxsdk_csharp-fixed-headers/fbxmatrix.h"

/******************************
 * FbxAMatrix (guaranteed affine)
 ******************************/
%ignore FbxAMatrix::operator double* ();
%ignore FbxAMatrix::operator const double* () const;
%ignore FbxAMatrix::Double44;

%ignore FbxAMatrix::operator=;
%ignore FbxAMatrix::operator+=;
%ignore FbxAMatrix::operator-=;
%ignore FbxAMatrix::operator*=;
%ignore FbxAMatrix::operator/=;

%define_unary_operator(FbxAMatrix, -, Negate);
%define_commutative_binary_operator(FbxAMatrix, *, Scale, double);
%define_binary_operator(FbxAMatrix, /, InvScale, double);
%define_binary_operator(FbxAMatrix, *, Mul, FbxAMatrix);
%define_equality_from_operator(FbxAMatrix);
// This case is handled by the Double4x4 operator==.
%ignore FbxAMatrix::operator==(const FbxMatrix&) const;
%extend FbxAMatrix {
  %csmethodmodifiers GetHashCode "public override";
  int GetHashCode() { return GetHashCode(*$self, 16); }
}

%include "fbxsdk_csharp-fixed-headers/fbxaffinematrix.h"

/******************************
 * End of file. Turn reveal back off.
 ******************************/
%reveal_all_end;

%rename("%s", %$isclass) FbxEuler;
%rename("%s") FbxEuler::EOrder;

%include "fbxsdk/core/math/fbxmath.h"