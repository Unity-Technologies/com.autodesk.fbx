// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxIOFileHeaderInfo;
%rename("%s") FbxIOFileHeaderInfo::FbxIOFileHeaderInfo;
%rename("%s") FbxIOFileHeaderInfo::~FbxIOFileHeaderInfo;
%fbximmutable(FbxIOFileHeaderInfo::mCreator);
%fbximmutable(FbxIOFileHeaderInfo::mFileVersion);

%include "fbxsdk/fileio/fbx/fbxio.h"