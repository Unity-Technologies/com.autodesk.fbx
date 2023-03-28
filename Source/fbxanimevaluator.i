// ***********************************************************************
// Copyright (c) 2023 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
// Unignore class
%rename("%s", %$isclass) FbxAnimEvaluator;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxAnimEvaluator::ValidateTime;

%include "fbxsdk/scene/animation/fbxanimevaluator.h"