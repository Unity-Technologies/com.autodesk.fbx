%nodefaultdtor;                                 // Disable creation of default constructors

%apply bool & OUTPUT { bool & pExportResult };
%ignore FbxExporter::SetProgressCallback;

%include "fbxsdk/fileio/fbxexporter.h"
