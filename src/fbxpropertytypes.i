// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore enum
%rename("%s") EFbxType;

/*
 * The color is an optimized type since it's almost exactly like FbxVector4.
 * See optimized/FbxDoubleTemplates.cs and optimization.i
 *
 * We want IsValid() to exactly match the FBX definition; the rest is all
 * trivial.
 **/
%{
bool IsValidColor(const FbxColor& c) {
  return c.IsValid();
}
%}
%rename("%s") IsValidColor(const FbxColor&);
%csmethodmodifiers IsValidColor "internal";
bool IsValidColor(const FbxColor& c);

%rename("%s") FbxTypeOf(const FbxColor&);
%rename("%s") FbxTypeOf(const FbxVector2&);
%rename("%s") FbxTypeOf(const FbxVector4&);
%rename("%s") FbxTypeOf(const FbxInt&);
%include "fbxsdk_csharp-fixed-headers/fbxpropertytypes.h"
