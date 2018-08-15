// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxLayerContainer;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxLayerContainer::CreateLayer;
%rename("%s") FbxLayerContainer::GetLayer(int pIndex);
%rename("%s") FbxLayerContainer::GetLayerCount;
#endif

%include "fbxsdk/scene/geometry/fbxlayercontainer.h"
