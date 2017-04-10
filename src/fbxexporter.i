#ifdef IGNORE_ALL_INCLUDE_SOME

// Unignore class
%rename("%s") FbxExporter;

%apply FbxDocument * MAYBENULL { FbxDocument *pDocument };
%rename("%s") FbxExporter::Export(FbxDocument *pDocument);

// TODO: should we be more specific, test each function in turn for whether it can
// actually take null?
%apply FbxIOSettings * MAYBENULL { FbxIOSettings * };

#else

%ignore FbxExporter::SetProgressCallback;
%apply bool & OUTPUT { bool & pExportResult };
%nodefaultdtor;                                 // Disable creation of default constructors

#endif

%include "fbxsdk/fileio/fbxexporter.h"
