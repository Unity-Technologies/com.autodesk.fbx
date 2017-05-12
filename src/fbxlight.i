// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxLight;
%rename("%s") FbxLight::EType;
%rename("%s") FbxLight::Create;

/* Properties */
%fbximmutable(FbxLight::LightType);
%fbximmutable(FbxLight::Color);
%fbximmutable(FbxLight::Intensity);
%fbximmutable(FbxLight::FileName);
%fbximmutable(FbxLight::DrawGroundProjection);
%fbximmutable(FbxLight::DrawVolumetricLight);
%fbximmutable(FbxLight::DrawFrontFacingVolumetricLight);
%fbximmutable(FbxLight::InnerAngle);
%fbximmutable(FbxLight::OuterAngle);

%include "fbxsdk/scene/geometry/fbxlight.h"