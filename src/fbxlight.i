// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxLight;
%rename("%s") FbxLight::EType;
%rename("%s") FbxLight::EAreaLightShape;
%rename("%s") FbxLight::EDecayType;
%rename("%s") FbxLight::Create;
%rename("%s") FbxLight::SetShadowTexture;
%rename("%s") FbxLight::GetShadowTexture;

/* Mark all the constants and properties immutable.
 * This file is auto-generated. */
%include "fbxlightimmutables.i"

%include "fbxsdk/scene/geometry/fbxlight.h"