// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxObject;

%rename("%s") FbxObject::Create;

/* Destroy is marked abstract in C# FbxEmitter but non-virtual in C++
 * FbxObject. Mark it override in C#. */
%csmethodmodifiers FbxObject::Destroy "public override";
%rename("%s") FbxObject::Destroy;


%rename("%s") FbxObject::GetName;
%rename("%s") FbxObject::SetName;
%rename("%s") FbxObject::GetInitialName;
%rename("%s") FbxObject::SetInitialName;
%rename("%s") FbxObject::GetNameWithoutNameSpacePrefix;
%rename("%s") FbxObject::SetNameSpace;
%rename("%s") FbxObject::GetNameSpaceOnly;
%rename("%s") FbxObject::StripPrefix(const char*); // not the FbxString& one, and not RemovePrefix
%rename("%s") FbxObject::GetSelected;
%rename("%s") FbxObject::SetSelected;

%rename("%s") FbxObject::GetFbxManager;
%rename("%s") FbxObject::GetScene;

%rename("%s") FbxObject::GetRuntimeClassId;

/* Properties */
%rename("%s") FbxObject::GetFirstProperty() const;
%rename("%s") FbxObject::GetNextProperty(const FbxProperty& pProperty) const;
%rename("%s") FbxObject::FindProperty;
%rename("%s") FbxObject::FindPropertyHierarchical;
%rename("%s") FbxObject::GetClassRootProperty();

%rename("%s") FbxObject::ConnectSrcProperty(const FbxProperty& pProperty);
%rename("%s") FbxObject::IsConnectedSrcProperty(const FbxProperty& pProperty);
%rename("%s") FbxObject::DisconnectSrcProperty(const FbxProperty& pProperty);
%rename("%s") FbxObject::GetSrcPropertyCount() const;
%rename("%s") FbxObject::GetSrcProperty(int pIndex=0) const;
%rename("%s") FbxObject::FindSrcProperty(const char* pName,int pStartIndex=0) const;

%rename("%s") FbxObject::ConnectDstProperty(const FbxProperty& pProperty);
%rename("%s") FbxObject::IsConnectedDstProperty(const FbxProperty& pProperty);
%rename("%s") FbxObject::DisconnectDstProperty(const FbxProperty& pProperty);
%rename("%s") FbxObject::GetDstPropertyCount() const;
%rename("%s") FbxObject::GetDstProperty(int pIndex=0) const;
%rename("%s") FbxObject::FindDstProperty(const char* pName, int pStartIndex=0) const;

/* Object to Object Connection */
%rename("%s") FbxObject::ConnectSrcObject;
%rename("%s") FbxObject::IsConnectedSrcObject;
%rename("%s") FbxObject::DisconnectSrcObject;
%rename("%s") FbxObject::GetSrcObjectCount() const;
%rename("%s") FbxObject::GetSrcObject(int pIndex=0) const;
%rename("%s") FbxObject::FindSrcObject(const char* pName, int pStartIndex=0) const;

%rename("%s") FbxObject::ConnectDstObject;
%rename("%s") FbxObject::IsConnectedDstObject;
%rename("%s") FbxObject::DisconnectDstObject;
%rename("%s") FbxObject::GetDstObjectCount() const;
%rename("%s") FbxObject::GetDstObject(int pIndex=0) const;
%rename("%s") FbxObject::FindDstObject(const char* pName, int pStartIndex=0) const;

/* Shader implementation. */
%rename("%s") FbxObject::AddImplementation;
%rename("%s") FbxObject::RemoveImplementation;
%rename("%s") FbxObject::HasDefaultImplementation;
%rename("%s") FbxObject::GetDefaultImplementation;
%rename("%s") FbxObject::SetDefaultImplementation;

%extend FbxObject {
  %proxycode %{
  public override string ToString() {
    string name;
    try { name = GetName(); }
    catch (System.ArgumentNullException) { name = "(destroyed)"; }
    catch (System.NullReferenceException) { name = "(disposed)"; }
    return string.Format("{0}({1})", name, GetType().Name);
  }
  %}
  
}

// Animation
%rename("%s") FbxObject::GetSrcObjectCountAnimLayer; 

%include "UnityFbxSdkNative-fixed-headers/fbxobject.h"

%extend FbxObject {
    int GetSrcObjectCountAnimLayer()
    {
        return $self->GetSrcObjectCount<FbxAnimLayer>();
    }
}
