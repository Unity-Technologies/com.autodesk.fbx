// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxLayer;
%nodefaultctor FbxLayer;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxLayer::SetNormals;
%rename("%s") FbxLayer::SetBinormals;
%rename("%s") FbxLayer::SetVertexColor;
%rename("%s") FbxLayer::SetUVs;
%rename("%s") FbxLayer::SetTangents;
#endif

%include "fbxsdk_csharp-fixed-headers/fbxlayer.h"
