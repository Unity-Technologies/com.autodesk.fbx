// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s", %$isclass) FbxTime;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxTime::FbxTime;
%rename("%s") FbxTime::~FbxTime;

// Unignore class
%rename("%s", %$isclass) FbxTimeSpan;

%rename("%s") FbxTimeSpan::FbxTimeSpan;
%rename("%s") FbxTimeSpan::~FbxTimeSpan;
%rename("%s") FbxTimeSpan::Set;
#endif

%include "fbxsdk/core/base/fbxtime.h"
