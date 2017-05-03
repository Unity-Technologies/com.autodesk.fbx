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

#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxMatrix;
%ignore FbxMatrix::FbxMatrix(const FbxVector4& pT, const FbxQuaternion& pQ, const FbxVector4& pS);
%rename("%s") FbxMatrix::~FbxMatrix();
%rename("%s") FbxMatrix::Get;
%rename("%s") FbxMatrix::Set;
%apply FbxVector4& OUTPUT { FbxVector4& pTranslation, FbxVector4& pRotation, FbxVector4& pShearing, FbxVector4& pScaling }
%apply double& OUTPUT { double& pSign }
%rename("%s") FbxMatrix::GetElements(FbxVector4& pTranslation, FbxVector4& pRotation, FbxVector4& pShearing, FbxVector4& pScaling, double& pSign) const;
%rename("%s") FbxMatrix::SetRow;
%rename("%s") FbxMatrix::SetColumn;
#endif

%include "fbxsdk_csharp-fixed-headers/fbxmatrix.h"


#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxAMatrix;
%rename("%s") FbxAMatrix::~FbxAMatrix();
%rename("%s") FbxAMatrix::Get;
#endif

%include "fbxsdk_csharp-fixed-headers/fbxaffinematrix.h"
