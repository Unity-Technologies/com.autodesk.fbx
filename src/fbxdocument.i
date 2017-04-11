#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxDocument;

%rename("%s") FbxDocument::GetDocumentInfo;
%rename("%s") FbxDocument::SetDocumentInfo;

#endif

%include "fbxsdk/scene/fbxdocument.h"


