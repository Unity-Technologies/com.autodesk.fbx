// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore chosen class 'FbxObject'
%rename("%s") FbxObject;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxObject::Create;
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
#endif

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

%include "fbxsdk/core/fbxobject.h"
