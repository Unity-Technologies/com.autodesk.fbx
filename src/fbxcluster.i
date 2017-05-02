// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s", %$isclass) FbxCluster;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxCluster::Create;
%rename("%s") FbxCluster::SetLink;
%rename("%s") FbxCluster::GetLink() const;
%rename("%s") FbxCluster::ELinkMode;
%rename("%s") FbxCluster::SetLinkMode;
%rename("%s") FbxCluster::GetLinkMode;
%rename("%s") FbxCluster::SetTransformMatrix;
%rename("%s") FbxCluster::GetTransformMatrix;
%rename("%s") FbxCluster::SetTransformLinkMatrix;
%rename("%s") FbxCluster::GetTransformLinkMatrix;
%rename("%s") FbxCluster::AddControlPointIndex;
#endif

%include "fbxsdk/scene/geometry/fbxcluster.h"