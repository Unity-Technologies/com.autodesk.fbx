// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxNode;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxNode::Create(FbxManager* pManager, const char* pName);
%rename("%s") FbxNode::Create(FbxObject* pContainer, const char* pName);
%rename("%s") FbxNode::GetParent();
%rename("%s") FbxNode::AddChild(FbxNode*);
%rename("%s") FbxNode::RemoveChild(FbxNode*);
%rename("%s") FbxNode::GetChild(int);
%rename("%s") FbxNode::GetChildCount(bool pRecursive = false) const;
%rename("%s") FbxNode::FindChild;
%rename("%s") FbxNode::SetNodeAttribute;
%rename("%s") FbxNode::SetShadingMode;
%rename("%s") FbxNode::EShadingMode;
%rename("%s") FbxNode::eWireFrame;
#endif

%include "fbxsdk/scene/geometry/fbxnode.h"
