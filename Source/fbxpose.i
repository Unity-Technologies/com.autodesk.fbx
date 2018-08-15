// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxPose;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxPose::Create;
%rename("%s") FbxPose::SetIsBindPose;
%rename("%s") FbxPose::IsBindPose;
%rename("%s") FbxPose::Add;
%rename("%s") FbxPose::GetNode;
%rename("%s") FbxPose::GetMatrix;
%rename("%s") FbxPose::GetCount;
#endif

%include "fbxsdk/scene/fbxpose.h"