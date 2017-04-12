/*
 * This is initial work on defining the math types:
 * - matrix (FbxMatrix)
 * - affine matrix (FbxAMatrix)
 * - vector (FbxVector4)
 */

class FbxDouble4x4 { };
class FbxDouble4 { };

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
%rename("%s") FbxVector4;
%ignore FbxVector4::FbxVector4(const double pValue[4]);
%ignore FbxVector4::FbxVector4(const FbxDouble3&);
%rename("%s") FbxVector4::~FbxVector4();
#endif

%include "fbxsdk/core/math/fbxvector4.h"


#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxAMatrix;
%rename("%s") FbxAMatrix::~FbxAMatrix();
%rename("%s") FbxMatrix::Get;
%rename("%s") FbxMatrix::Set;
#endif

%include "fbxsdk_csharp-fixed-headers/fbxaffinematrix.h"
