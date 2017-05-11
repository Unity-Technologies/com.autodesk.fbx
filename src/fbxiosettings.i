// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore class
%rename("%s", %$isclass) FbxIOSettings;
%rename("%s") FbxIOSettings::SetBoolProp;

// make sure SetBoolProp() doesn't crash if we pass it a null string
%typemap(check, canthrow=1) const char* pName %{
  if($1 == null){
    SWIG_CSharpSetPendingException(SWIG_CSharpNullReferenceException, "$1_basetype $1_name is null");
    return $null;
  }
%}

%include "fbxsdk/fileio/fbxiosettings.h"
