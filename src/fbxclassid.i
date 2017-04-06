#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxClassId;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxClassId::GetName;
#else
/* Ignore the constructors for class Id. */
%ignore FbxClassId::FbxClassId;
%ignore FbxClassId::Override;
#endif

%include "fbxsdk/core/fbxclassid.h"
