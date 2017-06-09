// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore class
%rename("%s", %$isclass) FbxBlendShape;

%rename("%s") FbxBlendShape::AddBlendShapeChannel;
%rename("%s") FbxBlendShape::RemoveBlendShapeChannel;
%rename("%s") FbxBlendShape::GetBlendShapeChannel(int pIndex) const;
%rename("%s") FbxBlendShape::GetBlendShapeChannelCount;
%rename("%s") FbxBlendShape::GetGeometry;
%rename("%s") FbxBlendShape::SetGeometry;

%include "fbxsdk/scene/geometry/fbxblendshape.h"