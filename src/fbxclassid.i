// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxClassId;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxClassId::GetName;

/* Ignore the constructors for class Id. */
%ignore FbxClassId::FbxClassId;

#else
/* Ignore the constructors for class Id. */
%ignore FbxClassId::FbxClassId;
%ignore FbxClassId::Override;
#endif

%include "fbxsdk/core/fbxclassid.h"
