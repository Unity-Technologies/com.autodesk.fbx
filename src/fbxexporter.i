#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxExporter;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxExporter::Export;

#else
%ignore FbxExporter::SetProgressCallback;
#endif

%nodefaultdtor;                                 // Disable creation of default constructors
%apply bool & OUTPUT { bool & pExportResult };

%include "fbxsdk/fileio/fbxexporter.h"
