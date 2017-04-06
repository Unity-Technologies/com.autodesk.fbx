#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore chosen class 'FbxObject'
%rename("%s") FbxObject;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxObject::Create(FbxManager* pManager, const char* pName); // named method
%rename("%s") FbxObject::Destroy;
%rename("%s") FbxObject::GetName;
%rename("%s") FbxObject::GetNameWithoutNameSpacePrefix;
%rename("%s") FbxObject::SetNameSpace;
%rename("%s") FbxObject::GetNameSpaceOnly;
%rename("%s") FbxObject::StripPrefix(const char*); // not the FbxString& one, and not RemovePrefix
#endif

%include "fbxsdk/core/fbxobject.h"
