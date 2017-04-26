// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
/* There's very little on this class; everything interesting is in FbxObject. */
%rename("%s") FbxSurfaceMaterial;
%rename("%s") FbxSurfaceMaterial::Create;
#endif

%include "fbxsdk/scene/shading/fbxsurfacematerial.h"
