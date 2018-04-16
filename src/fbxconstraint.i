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

// FbxConstraintParent

%ignore FbxConstraintParent::ConstraintSources;
%ignore FbxConstraintParent::ConstrainedObject;

%fbximmutable(FbxConstraintParent::AffectTranslationX);
%fbximmutable(FbxConstraintParent::AffectTranslationY);
%fbximmutable(FbxConstraintParent::AffectTranslationZ);
%fbximmutable(FbxConstraintParent::AffectRotationX);
%fbximmutable(FbxConstraintParent::AffectRotationY);
%fbximmutable(FbxConstraintParent::AffectRotationZ);
%fbximmutable(FbxConstraintParent::AffectScalingX);
%fbximmutable(FbxConstraintParent::AffectScalingY);
%fbximmutable(FbxConstraintParent::AffectScalingZ);

%include "fbxsdk/scene/constraint/fbxconstraintparent.h"

// FbxConstraintPosition

%ignore FbxConstraintPosition::ConstraintSources;
%ignore FbxConstraintPosition::ConstrainedObject;

%fbximmutable(FbxConstraintPosition::Translation);
%fbximmutable(FbxConstraintPosition::AffectX);
%fbximmutable(FbxConstraintPosition::AffectY);
%fbximmutable(FbxConstraintPosition::AffectZ);

%include "fbxsdk/scene/constraint/fbxconstraintposition.h"

// FbxConstraintRotation

%ignore FbxConstraintRotation::ConstraintSources;
%ignore FbxConstraintRotation::ConstrainedObject;

%fbximmutable(FbxConstraintRotation::Rotation);
%fbximmutable(FbxConstraintRotation::AffectX);
%fbximmutable(FbxConstraintRotation::AffectY);
%fbximmutable(FbxConstraintRotation::AffectZ);

%include "fbxsdk/scene/constraint/fbxconstraintrotation.h"

// FbxConstraintScale

%ignore FbxConstraintScale::ConstraintSources;
%ignore FbxConstraintScale::ConstrainedObject;

%fbximmutable(FbxConstraintScale::Scaling);
%fbximmutable(FbxConstraintScale::AffectX);
%fbximmutable(FbxConstraintScale::AffectY);
%fbximmutable(FbxConstraintScale::AffectZ);

%include "fbxsdk/scene/constraint/fbxconstraintscale.h"

/****************************************************************************
 * We end reveal-all mode now. This must be at the end of the file.
 ****************************************************************************/
%reveal_all_end;