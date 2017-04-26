// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxImplementation;
%rename("%s") FbxImplementation::Create;
%rename("%s") FbxImplementation::Language;
%rename("%s") FbxImplementation::LanguageVersion;
%rename("%s") FbxImplementation::RenderAPI;
%rename("%s") FbxImplementation::RenderAPIVersion;
%rename("%s") FbxImplementation::RootBindingName;
%rename("%s") FbxImplementation::AddNewTable;
%rename("%s") FbxImplementation::GetRootTable() const;

/* add these constants to FbxImplementation? */
%rename("%s") FBXSDK_SHADING_LANGUAGE_HLSL;
%rename("%s") FBXSDK_SHADING_LANGUAGE_GLSL;
%rename("%s") FBXSDK_SHADING_LANGUAGE_CGFX;
%rename("%s") FBXSDK_SHADING_LANGUAGE_SFX;
%rename("%s") FBXSDK_SHADING_LANGUAGE_MRSL;
%rename("%s") FBXSDK_RENDERING_API_DIRECTX;
%rename("%s") FBXSDK_RENDERING_API_OPENGL;
%rename("%s") FBXSDK_RENDERING_API_MENTALRAY;
%rename("%s") FBXSDK_RENDERING_API_PREVIEW;
#endif

%include "fbxsdk/scene/shading/fbximplementation.h"
%include "fbxsdk/scene/shading/fbxshadingconventions.h"
