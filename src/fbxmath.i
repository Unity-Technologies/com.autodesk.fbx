// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/*
 * This is initial work on defining the math types:
 * - matrix (FbxMatrix)
 * - affine matrix (FbxAMatrix)
 * Vector4 is handled specially because it's performance-critical.
 */
%declare_hand_optimized_type(FbxVector4, FbxSharpDouble4, FbxVector4);

#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxMatrix;
%ignore FbxMatrix::FbxMatrix(const FbxVector4& pT, const FbxQuaternion& pQ, const FbxVector4& pS);
%rename("%s") FbxMatrix::~FbxMatrix();
%rename("%s") FbxMatrix::Get;
%rename("%s") FbxMatrix::Set;
#endif

%include "fbxsdk_csharp-fixed-headers/fbxmatrix.h"


#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxAMatrix;
%rename("%s") FbxAMatrix::~FbxAMatrix();
%rename("%s") FbxAMatrix::Get;
#endif

%include "fbxsdk_csharp-fixed-headers/fbxaffinematrix.h"

#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxVector2;
#endif

%include "fbxsdk/core/math/fbxvector2.h"