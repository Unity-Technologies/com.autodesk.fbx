#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxIOBase;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxIOBase::FbxIOBase;
%rename("%s") FbxIOBase::~FbxIOBase;
%rename("%s") FbxIOBase::Initialize;

%rename("%s") IOSROOT;
#endif

// TODO: global constants that should be part of FbxIOBase
%include "fbxsdk/fileio/fbxiosettingspath.h"

%include "fbxsdk/fileio/fbxiobase.h"

