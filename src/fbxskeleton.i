// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxSkeleton;

%typemap(csclassmodifiers) FbxSkeleton::EType "public new enum";
%rename("%s") FbxSkeleton::EType;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxSkeleton::Create;
%rename("%s") FbxSkeleton::SetSkeletonType;
%rename("%s") FbxSkeleton::GetSkeletonType;
%fbximmutable(FbxSkeleton::Size);
#endif

%include "fbxsdk/scene/geometry/fbxskeleton.h"
