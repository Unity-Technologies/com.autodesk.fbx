// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// ignore everything we don't need
%ignore LockAccessStatus;
%ignore FbxLayerElementArrayReadLock;
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

%rename("$ignore", regextarget=1, fullname=1) "FbxLayerElementMaterial::.*";
%rename("%s") FbxLayerElementMaterial::Create;

%rename("$ignore", regextarget=1, fullname=1) "FbxLayer::.*";
%rename("%s") FbxLayer::SetNormals;
%rename("%s") FbxLayer::SetBinormals;
%rename("%s") FbxLayer::SetVertexColors;
%rename("%s") FbxLayer::SetUVs;
%rename("%s") FbxLayer::SetTangents;
%rename("%s") FbxLayer::SetMaterials;
%rename("%s") FbxLayer::GetNormals() const;
%rename("%s") FbxLayer::GetBinormals() const;
%rename("%s") FbxLayer::GetVertexColors() const;
%rename("%s") FbxLayer::GetUVs(FbxLayerElement::EType pTypeIdentifier=FbxLayerElement::eTextureDiffuse) const;
%rename("%s") FbxLayer::GetTangents() const;
%rename("%s") FbxLayer::GetMaterials() const;
%define_pointer_equality_functions(FbxLayer);

%rename("$ignore", regextarget=1, fullname=1) "FbxLayerElementTemplate::.*";
%rename("%s") FbxLayerElementTemplate::GetDirectArray() const;
%rename("%s") FbxLayerElementTemplate::GetIndexArray() const;

// don't ignore enum items (will only show up in C# if we unignore the enum itself)
%rename("$ignore", "not" %$isenumitem, regextarget=1, fullname=1) "FbxLayerElement::.*";
// unignore enums that we need
%rename("%s") FbxLayerElement::EType;
%rename("%s") FbxLayerElement::EMappingMode;
%rename("%s") FbxLayerElement::EReferenceMode;

%rename("%s") FbxLayerElement::GetMappingMode;
%rename("%s") FbxLayerElement::GetReferenceMode;
%rename("%s") FbxLayerElement::SetMappingMode;
%rename("%s") FbxLayerElement::SetReferenceMode;

// Ignore everything we don't need from FbxLayerElementArray.
// Have to do this so that we can expose the templates for method Add()
%ignore FbxLayerElementArray::ELockMode;
%ignore FbxLayerElementArray::ClearStatus;
%ignore FbxLayerElementArray::GetStatus;
%ignore FbxLayerElementArray::IsWriteLocked;
%ignore FbxLayerElementArray::GetReadLockCount;
%ignore FbxLayerElementArray::IsInUse;
%ignore FbxLayerElementArray::ReadLock;
%ignore FbxLayerElementArray::ReadUnlock;
%ignore FbxLayerElementArray::WriteLock;
%ignore FbxLayerElementArray::WriteUnlock;
%ignore FbxLayerElementArray::ReadWriteLock;
%ignore FbxLayerElementArray::ReadWriteUnlock;
%ignore FbxLayerElementArray::GetLocked;
%ignore FbxLayerElementArray::Release;
%ignore FbxLayerElementArray::GetStride;
%ignore FbxLayerElementArray::Clear;
%ignore FbxLayerElementArray::Resize;
%ignore FbxLayerElementArray::AddMultiple;
%ignore FbxLayerElementArray::Add(const void* pItem, EFbxType pValueType);
%ignore FbxLayerElementArray::InsertAt;
%ignore FbxLayerElementArray::SetLast;
%ignore FbxLayerElementArray::RemoveAt;
%ignore FbxLayerElementArray::RemoveLast;
%ignore FbxLayerElementArray::RemoveIt;
%ignore FbxLayerElementArray::GetAt;
%ignore FbxLayerElementArray::SetAt(int pIndex, const void* pItem, EFbxType pValueType);
%ignore FbxLayerElementArray::GetFirst;
%ignore FbxLayerElementArray::GetLast;
%ignore FbxLayerElementArray::Find;
%ignore FbxLayerElementArray::FindAfter;
%ignore FbxLayerElementArray::FindBefore;
%ignore FbxLayerElementArray::IsEqual;
%ignore FbxLayerElementArray::CopyTo;
%ignore FbxLayerElementArray::GetDataPtr;
%ignore FbxLayerElementArray::GetReference;
%ignore FbxLayerElementArray::GetReferenceTo;
%ignore FbxLayerElementArray::SetStatus;
%ignore FbxLayerElementArray::SetImplementation;
%ignore FbxLayerElementArray::GetImplementation;
%ignore FbxLayerElementArray::ConvertDataType;
%ignore FbxLayerElementArray::mDataType;

%rename("$ignore", "not" %$isconstructor, "not" %$isdestructor, regextarget=1, fullname=1) "FbxLayerElementArrayTemplate::.*";
%ignore FbxLayerElementArrayTemplate::FbxLayerElementArrayTemplate(EFbxType pDataType);
%rename("%s") FbxLayerElementArrayTemplate::~FbxLayerElementArrayTemplate;

%rename("%sUnchecked") FbxLayerElementArrayTemplate::GetAt;
%csmethodmodifiers FbxLayerElementArrayTemplate::GetAt "private";
%extend FbxLayerElementArrayTemplate { %proxycode %{
   public $typemap(cstype, T) GetAt(int pIndex) { 
      if (pIndex < 0 || pIndex >= GetCount()) { 
        throw new System.IndexOutOfRangeException();
      }
      return GetAtUnchecked(pIndex);
    }
%} }

// add some default constructors to help avoid making mistakes
// with setting the wrong FbxType
%add_default_template_constructor(FbxLayerElementArrayTemplate<int>, eFbxInt);
%add_default_template_constructor(FbxLayerElementArrayTemplate<FbxColor>, eFbxDouble4);
%add_default_template_constructor(FbxLayerElementArrayTemplate<FbxVector2>, eFbxDouble2);
%add_default_template_constructor(FbxLayerElementArrayTemplate<FbxVector4>, eFbxDouble4);
%add_default_template_constructor(FbxLayerElementArrayTemplate<FbxSurfaceMaterial*>, eFbxReference);


%include "UnityFbxSdkNative-fixed-headers/fbxlayer.h"

%template(Add) FbxLayerElementArray::Add<int>;
%template(Add) FbxLayerElementArray::Add<FbxColor>;
%template(Add) FbxLayerElementArray::Add<FbxVector2>;
%template(Add) FbxLayerElementArray::Add<FbxVector4>;

%template(SetAt) FbxLayerElementArray::SetAt<int>;
%template(SetAt) FbxLayerElementArray::SetAt<FbxColor>;
%template(SetAt) FbxLayerElementArray::SetAt<FbxVector2>;
%template(SetAt) FbxLayerElementArray::SetAt<FbxVector4>;

// needed for FbxLayerElementTemplate templates (more specifically for the GetArray() functions)
%template(FbxLayerElementArrayTemplateFbxColor) FbxLayerElementArrayTemplate<FbxColor>;
%template(FbxLayerElementArrayTemplateFbxVector2) FbxLayerElementArrayTemplate<FbxVector2>;
%template(FbxLayerElementArrayTemplateFbxVector4) FbxLayerElementArrayTemplate<FbxVector4>;
%template(FbxLayerElementArrayTemplateInt) FbxLayerElementArrayTemplate<int>;
