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
// Note: Have to add pValue too even though we don't need it so this doesn't
// also match argument pName in Create (which already handles null values)
%typemap(check, canthrow=1) (const char* pName, bool pValue) %{
  if(!$1){
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "$1_basetype is null", "$1_name");
    return $null;
  }
%}

%include "fbxsdk/fileio/fbxiosettings.h"
