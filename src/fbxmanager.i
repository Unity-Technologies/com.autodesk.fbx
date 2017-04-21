// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME

// Unignore class
%rename("%s") FbxManager;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxManager::Create; 
%rename("%s") FbxManager::Destroy; 
%rename("%s") FbxManager::FindClass;
%rename("%s") FbxManager::SetIOSettings;
%rename("%s") FbxManager::GetIOSettings;
%rename("%s") FbxManager::GetVersion;
%rename("%s") FbxManager::GetFileFormatVersion;

#endif

%nodefaultctor FbxManager;                      // Disable the default constructor for class FbxManager.

%apply int & OUTPUT { int & pMajor };
%apply int & OUTPUT { int & pMinor };
%apply int & OUTPUT { int & pRevision };

%include "fbxsdk/core/fbxmanager.h"
