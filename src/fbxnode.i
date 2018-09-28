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
%rename("%s") FbxNode::SetShadingMode;
%rename("%s") FbxNode::GetShadingMode;
%rename("%s") FbxNode::EShadingMode;
%rename("%s") FbxNode::SetRotationOrder;
%rename("%s") FbxNode::SetTransformationInheritType;


%apply int* OUTPUT { FbxEuler::EOrder& pRotationOrder };
%rename("%s") FbxNode::GetRotationOrder;

/* Attributes */
%rename("%s") FbxNode::SetNodeAttribute;
%rename("%s") FbxNode::GetNodeAttribute();
%rename("%s") FbxNode::GetSkeleton;
%rename("%s") FbxNode::GetMesh;
%rename("%s") FbxNode::GetGeometry;
%rename("%s") FbxNode::GetCamera(); /* non-const to avoid warning */
%rename("%s") FbxNode::GetLight();
%rename("%s") FbxNode::GetNurbsCurve();


/* Materials */
%rename("%s") FbxNode::AddMaterial;
%rename("%s") FbxNode::GetMaterial;
%rename("%s") FbxNode::GetMaterialIndex;

/* Visibility */
%rename("%s") FbxNode::SetVisibility;
%rename("%s") FbxNode::GetVisibility;

/* Skinned Mesh */
%rename("%s") FbxNode::EvaluateGlobalTransform;
%rename("%s") FbxNode::EvaluateLocalTransform;
%rename("%s") FbxNode::EPivotSet;
%rename("%s") FbxNode::GetMesh;
%rename("%s") FbxNode::SetPreRotation;
%rename("%s") FbxNode::GetPreRotation;
%rename("%s") FbxNode::EPivotState;
%rename("%s") FbxNode::SetPivotState;
%rename("%s") FbxNode::SetRotationActive;
%rename("%s") FbxNode::GetRotationActive;
%rename("%s") FbxNode::GetPostRotation;
%rename("%s") FbxNode::SetPostRotation;
%rename("%s") FbxNode::GetRotationPivot;
%rename("%s") FbxNode::SetRotationPivot;
%rename("%s") FbxNode::GetRotationOffset;
%rename("%s") FbxNode::SetRotationOffset;
%rename("%s") FbxNode::GetScalingOffset;
%rename("%s") FbxNode::SetScalingOffset;
%rename("%s") FbxNode::GetScalingPivot;
%rename("%s") FbxNode::SetScalingPivot;
#endif

/* The properties need to be marked immutable. */
%fbximmutable(FbxNode::LclTranslation);
%fbximmutable(FbxNode::LclRotation);
%fbximmutable(FbxNode::LclScaling);
%fbximmutable(FbxNode::VisibilityInheritance);
%fbximmutable(FbxNode::InheritType);

%include "fbxsdk/scene/geometry/fbxnode.h"
