%nodefaultdtor;                                 // Disable creation of default constructors

%apply bool & OUTPUT { bool & pExportResult };

%include "fbxsdk/fileio/fbxexporter.h"
