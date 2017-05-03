// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s", %$isclass) FbxSurfaceMaterial;
%rename("%s") FbxSurfaceMaterial::Create;

%rename("%s", %$isclass) FbxSurfaceLambert;
%rename("%s") FbxSurfaceLambert::Create;

%rename("%s", %$isclass) FbxSurfacePhong;
%rename("%s") FbxSurfacePhong::Create;
#endif

%include "fbxsurfaceimmutables.i";
%fbximmutable(FbxSurfaceMaterial::sMultiLayerDefault);

%include "fbxsdk/scene/shading/fbxsurfacematerial.h"
%include "fbxsdk/scene/shading/fbxsurfacelambert.h"
%include "fbxsdk/scene/shading/fbxsurfacephong.h"
