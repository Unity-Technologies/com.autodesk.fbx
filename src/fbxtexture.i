// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/* Mark all the constants and properties immutable. 
 * This file is auto-generated. */
%include "fbxtextureimmutables.i"

/* These are "helper" functions for getting/setting the Translation, Rotation,
 * and Scaling properties -- but with a weird interface.
 * Let's let our users use the actually easier interface. */
%rename("$ignore") FbxTexture::GetDefaultT;
%rename("$ignore") FbxTexture::GetDefaultR;
%rename("$ignore") FbxTexture::GetDefaultS;
%rename("$ignore") FbxTexture::SetDefaultT;
%rename("$ignore") FbxTexture::SetDefaultR;
%rename("$ignore") FbxTexture::SetDefaultS;

/* Ignore the class id. */
%ignore FbxTexture::ClassId;
%ignore FbxFileTexture::ClassId;

/* Ignore ETextureUse6 and EUnifiedMappingType, which are documented as
 * internal (despite being public and having public properties). */
%ignore FbxTexture::ETextureUse6;
%ignore FbxTexture::TextureTypeUse;

%ignore FbxTexture::EUnifiedMappingMode;
%ignore FbxTexture::CurrentMappingType;

/* The textures include some property templates that wrap enums. We need to
 * rename them before including the header, then template them after including
 * the header. Because swig. */
%rename("FbxPropertyEBlendMode") FbxPropertyT<FbxTexture::EBlendMode>;
%rename("FbxPropertyEWrapMode") FbxPropertyT<FbxTexture::EWrapMode>;

/* Include everything in the header files, even if normally we're being selective.
 * Go back to being selective after.
 * Let the coverage tests sort us out to make sure we test it all. */
%rename("%s") "";
%include "fbxsdk/scene/shading/fbxtexture.h"
%include "fbxsdk/scene/shading/fbxfiletexture.h"
/* Remember to build the templates we previously renamed *before* ignoring things. */
%template("FbxPropertyEBlendMode") FbxPropertyT<FbxTexture::EBlendMode>;
%template("FbxPropertyEWrapMode") FbxPropertyT<FbxTexture::EWrapMode>;
#ifdef IGNORE_ALL_INCLUDE_SOME
%ignore "";
#endif

