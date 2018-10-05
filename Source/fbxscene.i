// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxScene;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxScene::GetNodeCount;
%rename("%s") FbxScene::GetSceneInfo;
%rename("%s") FbxScene::SetSceneInfo;
%rename("%s") FbxScene::GetRootNode;
%rename("%s") FbxScene::GetGlobalSettings();
%rename("%s") FbxScene::AddPose;
%rename("%s") FbxScene::GetPose;
%rename("%s") FbxScene::SetCurrentAnimationStack;
%rename("%s") FbxScene::GetCurrentAnimationStack;
#endif

%include "fbxsdk/scene/fbxscene.h"

