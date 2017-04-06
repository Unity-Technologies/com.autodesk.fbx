#ifdef IGNORE_ALL_INCLUDE_SOME

// Unignore class
%rename("%s") FbxExporter;
%rename("%s") FbxExporter::Export(FbxDocument *pDocument);

#else

%ignore FbxExporter::SetProgressCallback;
%apply bool & OUTPUT { bool & pExportResult };
%nodefaultdtor;                                 // Disable creation of default constructors

#endif

%include "fbxsdk/fileio/fbxexporter.h"
