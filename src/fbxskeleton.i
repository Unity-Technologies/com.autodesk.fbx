// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%reveal_all_start;

%typemap(csclassmodifiers) FbxSkeleton::EType "public new enum";

%fbximmutable(FbxSkeleton::sSize);
%fbximmutable(FbxSkeleton::sLimbLength);
%fbximmutable(FbxSkeleton::sDefaultSize);
%fbximmutable(FbxSkeleton::sDefaultLimbLength);
%fbximmutable(FbxSkeleton::Size);
%fbximmutable(FbxSkeleton::LimbLength);

%include "fbxsdk/scene/geometry/fbxskeleton.h"

/****************************************************************************
 * We end reveal-all mode now. This must be at the end of the file.
 ****************************************************************************/
%reveal_all_end;
