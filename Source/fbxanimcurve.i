// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// Unignore class
%rename("%s", %$isclass) FbxAnimCurve;

// Create is funny: you *only* get the form with an FbxScene.
// The base class defined Create with the other arguments,
// make them throw (they return null in C++).
%ignore FbxAnimCurve::Create(FbxManager*, const char*);
%rename("%s") FbxAnimCurve::Create(FbxScene*, const char*);
%extend FbxAnimCurve { %proxycode %{
  public static new FbxAnimCurve Create(FbxManager pManager, string pName) {
    throw new System.NotImplementedException("FbxAnimCurve can only be created with a scene as argument.");
  }
  public static new FbxAnimCurve Create(FbxObject pContainer, string pName) {
    throw new System.NotImplementedException("FbxAnimCurve can only be created with a scene as argument.");
  }
%} }

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

// Unignore FbxAnimCurveDef. It's a static class.
%rename("%s", %$isclass) FbxAnimCurveDef;
%declare_static_class(FbxAnimCurveDef);

%rename("%s") FbxAnimCurveDef::EInterpolationType;
%rename("%s") FbxAnimCurveDef::ETangentMode;
%rename("%s") FbxAnimCurveDef::EWeightedMode;
%fbximmutable(FbxAnimCurveDef::sDEFAULT_WEIGHT);
%fbximmutable(FbxAnimCurveDef::sDEFAULT_VELOCITY);
%fbximmutable(FbxAnimCurveDef::sMIN_WEIGHT);
%fbximmutable(FbxAnimCurveDef::sMAX_WEIGHT);

%include "fbxsdk/scene/animation/fbxanimcurve.h"
