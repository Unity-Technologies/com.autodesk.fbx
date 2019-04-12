// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxConnection;
%rename("%s") FbxConnection::EType;
%declare_static_class(FbxConnection);

%include "fbxsdk/core/fbxconnectionpoint.h"
