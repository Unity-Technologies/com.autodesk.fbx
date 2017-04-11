#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxDocumentInfo;

%rename("%s") FbxDocumentInfo::mTitle;
%rename("%s") FbxDocumentInfo::mSubject;
%rename("%s") FbxDocumentInfo::mAuthor;
%rename("%s") FbxDocumentInfo::mKeywords;
%rename("%s") FbxDocumentInfo::mRevision;
%rename("%s") FbxDocumentInfo::mComment;

#endif

%include "fbxsdk/scene/fbxdocumentinfo.h"


