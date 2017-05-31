// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore class
%rename("%s") FbxIOBase;

#ifndef SWIG_GENERATING_TYPEDEFS
%apply FbxIOSettings * MAYBENULL { FbxIOSettings * pIOSettings };
#endif

%rename("%s") FbxIOBase::Initialize(const char *pFileName, int pFileFormat=-1, FbxIOSettings* pIOSettings=NULL);
%rename("%s") FbxIOBase::GetStatus;
%rename("%s") FbxIOBase::GetFileName;

/* Also include global constants that should seemingly be a part of FbxIOBase */
%rename("%s") IOSROOT;
%rename("%s") EXP_FBX_EMBEDDED;
%rename("%s") EXP_FBX_PASSWORD;
%rename("%s") EXP_FBX_PASSWORD_ENABLE;
%rename("%s") EXP_FBX_MATERIAL;
%rename("%s") EXP_FBX_TEXTURE;
%rename("%s") EXP_FBX_ANIMATION;
%rename("%s") EXP_FBX_GLOBAL_SETTINGS;

%rename("%s") IMP_FBX_PASSWORD;
%rename("%s") IMP_FBX_PASSWORD_ENABLE;
%rename("%s") IMP_FBX_MATERIAL;
%rename("%s") IMP_FBX_TEXTURE;
%rename("%s") IMP_FBX_ANIMATION;
%rename("%s") IMP_FBX_GLOBAL_SETTINGS;
%rename("%s") IMP_FBX_EXTRACT_EMBEDDED_DATA;

%include "fbxsdk/fileio/fbxiosettingspath.h"

%include "fbxsdk/fileio/fbxiobase.h"

