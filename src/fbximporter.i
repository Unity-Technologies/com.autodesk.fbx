#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxImporter;

// explicitly unignored the following:
%rename("%s") FbxImporter::Import;

#else
%ignore SetPassword;
#endif

%include "fbxsdk/fileio/fbximporter.h"

