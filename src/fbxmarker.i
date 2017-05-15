// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxMarker;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxMarker::ELook;
%rename("%s") FbxMarker::EType;
%rename("%s") FbxMarker::SetType;
%rename("%s") FbxMarker::Reset;

#endif

%include "fbxmarkerimmutables.i";

%include "fbxsdk/scene/geometry/fbxmarker.h"
