// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxDocument;

%rename("%s") FbxDocument::GetDocumentInfo;
%rename("%s") FbxDocument::SetDocumentInfo;

#endif

%include "fbxsdk/scene/fbxdocument.h"


