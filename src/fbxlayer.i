// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// ignore everything we don't need
%ignore LockAccessStatus;
%ignore FbxLayerElementArray;
%ignore FbxLayerElementArrayReadLock;
%ignore FbxLayerElementArrayTemplate;
%ignore FbxLayerElementMaterial;
%ignore LayerElementArrayProxy;
%ignore FbxLayerElementPolygonGroup;
%ignore FbxLayerElementUserData;
%ignore FbxLayerElementSmoothing;
%ignore FbxLayerElementCrease;
%ignore FbxLayerElementHole;
%ignore FbxLayerElementVisibility;
%ignore FbxLayerElementTexture;
%ignore FBXSDK_LAYER_ELEMENT_CREATE_DECLARE;
%ignore FBXSDK_FOR_EACH_TEXTURE;
%ignore FBXSDK_FOR_EACH_NON_TEXTURE;
%ignore FBXSDK_TEXTURE_INDEX;
%ignore FBXSDK_TEXTURE_TYPE;
%ignore FBXSDK_NON_TEXTURE_INDEX;
%ignore FBXSDK_NON_TEXTURE_TYPE;
%ignore FbxRefPtr;
%ignore FbxLayerElementArrayPtr;
%ignore FbxSurfaceMaterialPtr;
%ignore FbxTexturePtr;
%ignore FbxGeometryElement;
%ignore FbxGeometryElementNormal;
%ignore FbxGeometryElementBinormal;
%ignore FbxGeometryElementTangent;
%ignore FbxGeometryElementMaterial;
%ignore FbxGeometryElementPolygonGroup;
%ignore FbxGeometryElementUV;
%ignore FbxGeometryElementVertexColor;
%ignore FbxGeometryElementUserData;
%ignore FbxGeometryElementSmoothing;
%ignore FbxGeometryElementCrease;
%ignore FbxGeometryElementHole;
%ignore FbxGeometryElementVisibility;
%ignore FbxTypeOf;
%ignore RemapIndexArrayTo;
%ignore FbxGetDirectArray; 

// This statement ignores everything in FbxLayerElementNormal, except the class itself.
// Using this so that we can then unignore what we need from the class, instead of
// having to ignore each function individually.
// Also makes it easy to see which functions are actually being wrapped.
%rename("$ignore", regextarget=1, fullname=1) "FbxLayerElementNormal::.*";
%rename("%s") FbxLayerElementNormal::Create;

%rename("$ignore", regextarget=1, fullname=1) "FbxLayerElementBinormal::.*";
%rename("%s") FbxLayerElementBinormal::Create;

%rename("$ignore", regextarget=1, fullname=1) "FbxLayerElementTangent::.*";
%rename("%s") FbxLayerElementTangent::Create;

%rename("$ignore", regextarget=1, fullname=1) "FbxLayerElementUV::.*";
%rename("%s") FbxLayerElementUV::Create;

%rename("$ignore", regextarget=1, fullname=1) "FbxLayerElementVertexColor::.*";
%rename("%s") FbxLayerElementVertexColor::Create;

%rename("$ignore", regextarget=1, fullname=1) "FbxLayer::.*";
%rename("%s") FbxLayer::SetNormals;
%rename("%s") FbxLayer::SetBinormals;
%rename("%s") FbxLayer::SetVertexColors;
%rename("%s") FbxLayer::SetUVs;
%rename("%s") FbxLayer::SetTangents;

%rename("$ignore", regextarget=1, fullname=1) "FbxLayerElementTemplate::.*";

// don't ignore enum items (will only show up in C# if we unignore the enum itself)
%rename("$ignore", "not" %$isenumitem, regextarget=1, fullname=1) "FbxLayerElement::.*";
// unignore enums that we need
%rename("%s") FbxLayerElement::EType;
%rename("%s") FbxLayerElement::EMappingMode;
%rename("%s") FbxLayerElement::EReferenceMode;

%rename("%s") FbxLayerElement::SetMappingMode;
%rename("%s") FbxLayerElement::SetReferenceMode;

%include "fbxsdk_csharp-fixed-headers/fbxlayer.h"
