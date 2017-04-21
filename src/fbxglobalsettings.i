// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxGlobalSettings;
%rename("%s") FbxGlobalSettings::Create;
%rename("%s") FbxGlobalSettings::Destroy;
%rename("%s") FbxGlobalSettings::SetAxisSystem;
%rename("%s") FbxGlobalSettings::GetAxisSystem;
%rename("%s") FbxGlobalSettings::SetSystemUnit;
%rename("%s") FbxGlobalSettings::GetSystemUnit;
#endif

%include "fbxsdk/fileio/fbxglobalsettings.h"
