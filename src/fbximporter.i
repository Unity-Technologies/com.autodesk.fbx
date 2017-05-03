// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s") FbxImporter;

// explicitly unignored the following:
%rename("%s") FbxImporter::Import;

/* Explicitly ignore it or else it pops up despite -fvirtual and default ignore. */
%ignore FbxImporter::Initialize(const char* pFileName, int pFileFormat=-1, FbxIOSettings * pIOSettings=NULL);

%ignore SetPassword;

%include "fbxsdk/fileio/fbximporter.h"
