// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s") FbxImporter;

/*
 * Allow importing in blocking mode.
 * TODO: support non-blocking if there's demand for it.
 *
 * Non-blocking mode opens up the possibility of crashes from multi-threaded
 * use of the same FbxManager, or from funny garbage collection business.
 */
%rename("%s") FbxImporter::Import(FbxDocument*);

/* SetProgressCallback is implemented in fbxprogress.i */
%define_fbxprogress(FbxImporter);

/* Explicitly ignore it or else it pops up despite -fvirtual and default ignore. */
%ignore FbxImporter::Initialize(const char* pFileName, int pFileFormat=-1, FbxIOSettings * pIOSettings=NULL);

%ignore SetPassword;
%rename("%s") FbxImporter::IsFBX;
%rename("%s") FbxImporter::GetFileVersion;
%rename("%s") FbxImporter::GetAnimStackCount;
%rename("%s") FbxImporter::GetActiveAnimStackName;

%include "fbxsdk/fileio/fbximporter.h"
