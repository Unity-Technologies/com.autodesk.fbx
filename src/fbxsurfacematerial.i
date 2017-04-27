// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
/* There's very little on this class; everything interesting is in FbxObject or in phong and lambert. */
%rename("%s") FbxSurfaceMaterial;
%rename("%s") FbxSurfaceMaterial::Create;

%rename("%s", %$isclass) FbxSurfaceLambert;
%rename("%s") FbxSurfaceLambert::Create;
%fbxproperty(FbxSurfaceLambert::Emissive);
%fbxproperty(FbxSurfaceLambert::EmissiveFactor);
%fbxproperty(FbxSurfaceLambert::Ambient);
%fbxproperty(FbxSurfaceLambert::AmbientFactor);
%fbxproperty(FbxSurfaceLambert::Diffuse);
%fbxproperty(FbxSurfaceLambert::DiffuseFactor);
%fbxproperty(FbxSurfaceLambert::NormalMap);
%fbxproperty(FbxSurfaceLambert::Bump);
%fbxproperty(FbxSurfaceLambert::BumpFactor);
%fbxproperty(FbxSurfaceLambert::TransparentColor);
%fbxproperty(FbxSurfaceLambert::TransparencyFactor);
%fbxproperty(FbxSurfaceLambert::DisplacementColor);
%fbxproperty(FbxSurfaceLambert::DisplacementFactor);
%fbxproperty(FbxSurfaceLambert::VectorDisplacementColor);
%fbxproperty(FbxSurfaceLambert::VectorDisplacementFactor);

%rename("%s", %$isclass) FbxSurfacePhong;
%rename("%s") FbxSurfacePhong::Create;
%fbxproperty(FbxSurfacePhong::Specular);
%fbxproperty(FbxSurfacePhong::SpecularFactor);
%fbxproperty(FbxSurfacePhong::Shininess);
%fbxproperty(FbxSurfacePhong::Reflection);
%fbxproperty(FbxSurfacePhong::ReflectionFactor);
#endif

%include "fbxsdk/scene/shading/fbxsurfacematerial.h"
%include "fbxsdk/scene/shading/fbxsurfacelambert.h"
%include "fbxsdk/scene/shading/fbxsurfacephong.h"
