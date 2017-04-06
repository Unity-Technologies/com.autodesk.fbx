#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxImporter;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxImporter::Import;

#else
%ignore SetPassword;
#endif

%include "fbxsdk/fileio/fbximporter.h"

