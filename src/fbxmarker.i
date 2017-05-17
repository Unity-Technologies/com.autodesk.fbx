// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%reveal_all_start;

%include "fbxmarkerimmutables.i";

/* EType is in NodeAttribute as well, so we need to mark it new. */
%typemap(csclassmodifiers) FbxMarker::EType "public new enum";

/* GetType is reserved in C#; renamed to MarkerType. */
%rename("GetMarkerType") FbxMarker::GetType;
%rename("SetMarkerType") FbxMarker::SetType;

/* Marked as obsolete in the header file (but not deprecated) */
%ignore FbxMarker::GetDefaultColor;
%ignore FbxMarker::SetDefaultColor;


%include "fbxsdk/scene/geometry/fbxmarker.h"

/****************************************************************************
 * We end reveal-all mode now. This must be at the end of the file.
 ****************************************************************************/
%reveal_all_end;
