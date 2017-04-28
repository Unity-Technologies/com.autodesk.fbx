// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxSkin;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxSkin::Create;
%rename("%s") FbxSkin::AddCluster;
%rename("%s") FbxSkin::GetCluster(int pIndex) const;
#endif

%include "fbxsdk/scene/geometry/fbxskin.h"