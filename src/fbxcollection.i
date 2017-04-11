#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxCollection;

%rename("%s") FbxCollection::Clear;

#endif

%include "fbxsdk/scene/fbxcollection.h"
