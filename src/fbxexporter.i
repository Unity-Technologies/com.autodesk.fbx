// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME

// Unignore class
%rename("%s") FbxExporter;


%rename("%s") FbxExporter::Export(FbxDocument *pDocument);

#else

%ignore FbxExporter::SetProgressCallback;
%apply bool & OUTPUT { bool & pExportResult };
%nodefaultdtor;                                 // Disable creation of default constructors

#endif

#ifndef SWIG_GENERATING_TYPEDEFS
// TODO: should we be more specific, test each function in turn for whether it can
// actually take null?
%apply FbxIOSettings * MAYBENULL { FbxIOSettings * };
%apply FbxDocument * MAYBENULL { FbxDocument *pDocument };
#endif

%include "fbxsdk/fileio/fbxexporter.h"
