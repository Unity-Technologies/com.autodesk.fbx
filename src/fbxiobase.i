// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxIOBase;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxIOBase::Initialize(const char *pFileName, int pFileFormat=-1, FbxIOSettings* pIOSettings=NULL);

%rename("%s") IOSROOT;
#endif

// TODO: global constants that should be part of FbxIOBase
%include "fbxsdk/fileio/fbxiosettingspath.h"

%include "fbxsdk/fileio/fbxiobase.h"

