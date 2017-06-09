// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore class
%rename("%s", %$isclass) FbxBlendShapeChannel;

%rename("%s") FbxBlendShapeChannel::AddTargetShape;
%rename("%s") FbxBlendShapeChannel::RemoveTargetShape;
%rename("%s") FbxBlendShapeChannel::GetTargetShapeCount;
%rename("%s") FbxBlendShapeChannel::GetTargetShape(int pIndex) const;
%rename("%s") FbxBlendShapeChannel::GetTargetShapeIndex;
%rename("%s") FbxBlendShapeChannel::SetBlendShapeDeformer;
%rename("%s") FbxBlendShapeChannel::GetBlendShapeDeformer;

%fbximmutable(FbxBlendShapeChannel::DeformPercent);

%include "fbxsdk/scene/geometry/fbxblendshapechannel.h"