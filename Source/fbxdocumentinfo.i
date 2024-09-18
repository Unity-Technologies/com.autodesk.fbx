// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxDocumentInfo;

%rename("%s") FbxDocumentInfo::Create;
%rename("%s") FbxDocumentInfo::Destroy;
%rename("%s") FbxDocumentInfo::Clear;

%rename("%s") FbxDocumentInfo::mTitle;
%rename("%s") FbxDocumentInfo::mSubject;
%rename("%s") FbxDocumentInfo::mAuthor;
%rename("%s") FbxDocumentInfo::mKeywords;
%rename("%s") FbxDocumentInfo::mRevision;
%rename("%s") FbxDocumentInfo::mComment;
%rename("%s") FbxDocumentInfo::mDocumentUrl;
%rename("%s") FbxDocumentInfo::mSrcDocumentUrl;

%include "fbxdocumentinfoimmutables.i"

/* TODO when we have FbxDateTime. */
%ignore FbxDocumentInfo::Original_DateTime_GMT;
%ignore FbxDocumentInfo::LastSaved_DateTime_GMT;

%include "fbxsdk/scene/fbxdocumentinfo.h"
