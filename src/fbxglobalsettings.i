// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore class
%rename("%s", %$isclass) FbxGlobalSettings;
%rename("%s") FbxGlobalSettings::Create;
%rename("%s") FbxGlobalSettings::Destroy;
%rename("%s") FbxGlobalSettings::SetAxisSystem;
%rename("%s") FbxGlobalSettings::GetAxisSystem;
%rename("%s") FbxGlobalSettings::SetSystemUnit;
%rename("%s") FbxGlobalSettings::GetSystemUnit;
%rename("%s") FbxGlobalSettings::SetDefaultCamera;
%rename("%s") FbxGlobalSettings::GetDefaultCamera;

%include "fbxsdk/fileio/fbxglobalsettings.h"
