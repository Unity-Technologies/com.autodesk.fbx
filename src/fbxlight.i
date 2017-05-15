// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%reveal_all_start;

/* EType is in NodeAttribute as well, so we need to mark it new. */
%typemap(csclassmodifiers) FbxLight::EType "public new enum";

/* Color is in NodeAttribute as well, so mark it new.
 * They actually test equal, so they're the same property, and it's just a bit
 * of waste to have both. We expose this one really just so we can run unit
 * tests on it to make sure there's no bug potential here. */
%csmethodmodifiers FbxLight::Color "public new";

/* Mark all the constants and properties immutable.
 * This file is auto-generated. */
%include "fbxlightimmutables.i"

%include "fbxsdk/scene/geometry/fbxlight.h"

/****************************************************************************
 * We end reveal-all mode now. This must be at the end of the file.
 ****************************************************************************/
%reveal_all_end;
