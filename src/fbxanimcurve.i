// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore class
%rename("%s", %$isclass) FbxAnimCurve;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxAnimCurve::KeyModifyBegin;
%rename("%s") FbxAnimCurve::KeyModifyEnd;
%rename("%s") FbxAnimCurve::KeyGetCount;

%apply int* INOUT {int* pLast};

%rename("%s") FbxAnimCurve::KeyAdd(FbxTime pTime, int* pLast=((void *) 0));
%rename("%s") FbxAnimCurve::KeySet(int pKeyIndex,
        FbxTime pTime,
        float pValue,
        FbxAnimCurveDef::EInterpolationType pInterpolation=FbxAnimCurveDef::eInterpolationCubic,
		FbxAnimCurveDef::ETangentMode pTangentMode=FbxAnimCurveDef::eTangentAuto,
        float pData0=0.0,
        float pData1=0.0,
        FbxAnimCurveDef::EWeightedMode pTangentWeightMode=FbxAnimCurveDef::eWeightedNone,
        float pWeight0=FbxAnimCurveDef::sDEFAULT_WEIGHT,
        float pWeight1=FbxAnimCurveDef::sDEFAULT_WEIGHT,
        float pVelocity0=FbxAnimCurveDef::sDEFAULT_VELOCITY,
        float pVelocity1=FbxAnimCurveDef::sDEFAULT_VELOCITY 
    );
%rename("%s") FbxAnimCurve::KeyGetValue;

// Unignore FbxAnimCurveDef
%rename("%s", %$isclass) FbxAnimCurveDef;

%rename("%s") FbxAnimCurveDef::EInterpolationType;
%rename("%s") FbxAnimCurveDef::ETangentMode;
%rename("%s") FbxAnimCurveDef::EWeightedMode;
%rename("%s") FbxAnimCurveDef::sDEFAULT_WEIGHT;
%rename("%s") FbxAnimCurveDef::sDEFAULT_VELOCITY;

%include "fbxsdk/scene/animation/fbxanimcurve.h"