// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxCamera;

/* Enums */
%rename("%s") FbxCamera::EProjectionType;
%rename("%s") FbxCamera::EApertureMode;
%rename("%s") FbxCamera::EAspectRatioMode;

/* Functions */
%rename("%s") FbxCamera::Create;
%rename("%s") FbxCamera::SetAspect;
%rename("%s") FbxCamera::GetAspectRatioMode;
%rename("%s") FbxCamera::SetApertureWidth;
%rename("%s") FbxCamera::GetApertureWidth;
%rename("%s") FbxCamera::SetApertureHeight;
%rename("%s") FbxCamera::GetApertureHeight;
%rename("%s") FbxCamera::SetApertureMode;
%rename("%s") FbxCamera::GetApertureMode;
%rename("%s") FbxCamera::ComputeFocalLength;
%rename("%s") FbxCamera::SetNearPlane;
%rename("%s") FbxCamera::GetNearPlane;
%rename("%s") FbxCamera::SetFarPlane;
%rename("%s") FbxCamera::GetFarPlane;

/* Properties */
%fbximmutable(FbxCamera::ProjectionType);
%fbximmutable(FbxCamera::FilmAspectRatio);
%fbximmutable(FbxCamera::FocalLength);

%include "fbxsdk/scene/geometry/fbxcamera.h"

/* Fbx Global Camera Settings*/
%rename("%s") FBXSDK_CAMERA_PERSPECTIVE;

%include "fbxsdk/fileio/fbxglobalcamerasettings.h"