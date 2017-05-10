// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxAnimCurveNode;

/* Get a bunch of global constants */
%rename("%s", regextarget=1) "^FBXSDK_CURVENODE_.*$";

%rename("%s") FbxAnimCurveNode::Create;
%rename("%s") FbxAnimCurveNode::CreateTypedCurveNode;

%rename("%s") FbxAnimCurveNode::IsComposite;
%rename("%s") FbxAnimCurveNode::IsAnimated;

/* TODO: handle the output argument correctly (should be an 'out' variable) */
%rename("%s") FbxAnimCurveNode::GetAnimationInterval;

%rename("%s") FbxAnimCurveNode::GetChannelsCount;
%rename("%s") FbxAnimCurveNode::GetChannelIndex;
%rename("%s") FbxAnimCurveNode::GetChannelName;

%rename("%s") FbxAnimCurveNode::CreateCurve;
%rename("%s") FbxAnimCurveNode::GetCurveCount;
%rename("%s") FbxAnimCurveNode::GetCurve;


/* These functions are templated so they need tender loving care. */
%rename("AddChannel") FbxAnimCurveNode::AddChannel;
%rename("SetChannelValue") FbxAnimCurveNode::SetChannelValue;
%rename("GetChannelValue") FbxAnimCurveNode::GetChannelValue;

%include "fbxsdk/scene/animation/fbxanimcurvenode.h"

%template(AddChannelValue) FbxAnimCurveNode::AddChannel<float>;
%template(SetChannelValue) FbxAnimCurveNode::SetChannelValue<float>;
%template(GetChannelValue) FbxAnimCurveNode::GetChannelValue<float>;
