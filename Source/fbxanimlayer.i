// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
// Unignore class
%rename("%s", %$isclass) FbxAnimLayer;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxAnimLayer::Create;

%include "fbxsdk/scene/animation/fbxanimlayer.h"