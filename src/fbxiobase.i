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
%include "fbxsdk/fileio/fbxiosettingspath.h"

%include "fbxsdk/fileio/fbxiobase.h"

