// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s", %$isclass) FbxIOPluginRegistry;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxIOPluginRegistry::FindWriterIDByDescription;

// make sure FindWriterIDByDescription() doesn't crash if we pass it a null string
%null_arg_check(const char* pDesc);
#endif

%include "fbxsdk/fileio/fbxiopluginregistry.h"