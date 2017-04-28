// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s", %$isclass) FbxSurfaceMaterial;
%rename("%s") FbxSurfaceMaterial::Create;
%fbximmutable(FbxSurfaceMaterial::ShadingModel);
//%fbximmutable(FbxSurfaceMaterial::MultiLayer); // wait for FbxPropertyT<bool>

%rename("%s", %$isclass) FbxSurfaceLambert;
%rename("%s") FbxSurfaceLambert::Create;
%fbximmutable(FbxSurfaceLambert::Emissive);
%fbximmutable(FbxSurfaceLambert::EmissiveFactor);
%fbximmutable(FbxSurfaceLambert::Ambient);
%fbximmutable(FbxSurfaceLambert::AmbientFactor);
%fbximmutable(FbxSurfaceLambert::Diffuse);
%fbximmutable(FbxSurfaceLambert::DiffuseFactor);
%fbximmutable(FbxSurfaceLambert::NormalMap);
%fbximmutable(FbxSurfaceLambert::Bump);
%fbximmutable(FbxSurfaceLambert::BumpFactor);
%fbximmutable(FbxSurfaceLambert::TransparentColor);
%fbximmutable(FbxSurfaceLambert::TransparencyFactor);
%fbximmutable(FbxSurfaceLambert::DisplacementColor);
%fbximmutable(FbxSurfaceLambert::DisplacementFactor);
%fbximmutable(FbxSurfaceLambert::VectorDisplacementColor);
%fbximmutable(FbxSurfaceLambert::VectorDisplacementFactor);

%rename("%s", %$isclass) FbxSurfacePhong;
%rename("%s") FbxSurfacePhong::Create;
%fbximmutable(FbxSurfacePhong::Specular);
%fbximmutable(FbxSurfacePhong::SpecularFactor);
%fbximmutable(FbxSurfacePhong::Shininess);
%fbximmutable(FbxSurfacePhong::Reflection);
%fbximmutable(FbxSurfacePhong::ReflectionFactor);
#endif

%include "fbxsdk/scene/shading/fbxsurfacematerial.h"
%include "fbxsdk/scene/shading/fbxsurfacelambert.h"
%include "fbxsdk/scene/shading/fbxsurfacephong.h"
