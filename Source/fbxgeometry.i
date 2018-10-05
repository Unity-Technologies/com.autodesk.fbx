// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxGeometry;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxGeometry::AddDeformer;
%rename("%s") FbxGeometry::GetDeformer;
%rename("%s") FbxGeometry::GetDeformerCount;
%rename("%s") FbxGeometry::GetBlendShapeDeformer;
#endif

%extend FbxGeometry {
    FbxBlendShape* GetBlendShapeDeformer(int pIndex, FbxStatus* pStatus=NULL) const
    {
        FbxDeformer* deformer = $self->GetDeformer(pIndex, FbxDeformer::eBlendShape, pStatus);
        return FbxCast<FbxBlendShape>(deformer);
    }
}
%include "fbxsdk/scene/geometry/fbxgeometry.h"
