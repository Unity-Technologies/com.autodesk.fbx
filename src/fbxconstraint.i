// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%reveal_all_start;

// FbxConstraint
%fbximmutable(FbxConstraint::Weight);
%fbximmutable(FbxConstraint::Active);
%fbximmutable(FbxConstraint::Lock);

%include "fbxsdk/scene/constraint/fbxconstraint.h"


// FbxConstraintAim

%ignore FbxConstraintAim::AimAtObjects;
%ignore FbxConstraintAim::ConstrainedObject;
%ignore FbxConstraintAim::WorldUpObject;

%fbximmutable(FbxConstraintAim::RotationOffset);
%fbximmutable(FbxConstraintAim::WorldUpType);
%fbximmutable(FbxConstraintAim::WorldUpVector);
%fbximmutable(FbxConstraintAim::UpVector);
%fbximmutable(FbxConstraintAim::AimVector);
%fbximmutable(FbxConstraintAim::AffectX);
%fbximmutable(FbxConstraintAim::AffectY);
%fbximmutable(FbxConstraintAim::AffectZ);

%include "fbxsdk/scene/constraint/fbxconstraintaim.h"

/****************************************************************************
 * We end reveal-all mode now. This must be at the end of the file.
 ****************************************************************************/
%reveal_all_end;