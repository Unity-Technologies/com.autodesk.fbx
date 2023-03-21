// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s", %$isclass) FbxMesh;

%rename("%s") FbxMesh::Create;

%rename("%s") FbxMesh::GetPolygonVertex;
%rename("%s") FbxMesh::GetPolygonCount;
%rename("%s") FbxMesh::GetPolygonSize;
%rename("%s") FbxMesh::GetPolygonVertexCount;
%rename("%s") FbxMesh::GetPolygonVertices;

%apply FbxVector4& OUTPUT { FbxVector4& pNormal };
%rename("%s") FbxMesh::GetPolygonVertexNormal(int pPolyIndex, int pVertexIndex, FbxVector4& pNormal) const;

#endif

/*
 * AddPolygon crashes if you haven't called BeginPolygon. We add a check.
 *
 * We make the normal wrappers private, with an 'unchecked' suffix. We only
 * expose the version with all the arguments, because we'll always be passing
 * them. Then we have public methods with the normal name and default arguments.
 * The public methods check that the operation is legal, and throw
 * BadBracketingException if not.
 *
 * While we're at it, we check for negative vertex indices.
 *
 * For performance we may eventually want to provide extensions that do the
 * check and call into C++ just once for an entire triangle or quad.
 */
%extend FbxMesh { %proxycode %{
  [System.SerializableAttribute]
  public class BadBracketingException : System.NotSupportedException {
    public    BadBracketingException() : base() { }
    public    BadBracketingException(string message, System.Exception innerException) : base("Improper bracketing of Begin/Add/EndPolygon: " + message, innerException) { }
    public    BadBracketingException(string message) : base("Improper bracketing of Begin/Add/EndPolygon: " + message) { }
    protected BadBracketingException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
  }

  bool m_isAddingPolygon = false;
%} }

%ignore FbxMesh::BeginPolygon;
%rename ("BeginPolygonUnchecked") FbxMesh::BeginPolygon(int pMaterial, int pTexture, int pGroup, bool pLegacy);
%csmethodmodifiers FbxMesh::BeginPolygon "private";
%extend FbxMesh { %proxycode %{
  public void BeginPolygon(int pMaterial=-1, int pTexture=-1, int pGroup=-1, bool pLegacy=true) {
    if (m_isAddingPolygon) { throw new BadBracketingException("BeginPolygon while already building a polygon"); }
    BeginPolygonUnchecked(pMaterial, pTexture, pGroup, pLegacy);
    m_isAddingPolygon = true;
  } %} }

%ignore FbxMesh::AddPolygon;
%rename ("AddPolygonUnchecked") FbxMesh::AddPolygon(int pIndex, int pTextureUVIndex);
%csmethodmodifiers FbxMesh::AddPolygon "private";
%extend FbxMesh { %proxycode %{
  public void AddPolygon(int pIndex, int pTextureUVIndex = -1) {
    if (!m_isAddingPolygon) { throw new BadBracketingException("AddPolygon without matching BeginPolygon"); }
    if (pIndex < 0) { throw new System.ArgumentOutOfRangeException("pIndex"); }
    AddPolygonUnchecked(pIndex, pTextureUVIndex);
  } %} }

%ignore FbxMesh::EndPolygon;
%rename ("EndPolygonUnchecked") FbxMesh::EndPolygon();
%csmethodmodifiers FbxMesh::EndPolygon "private";
%extend FbxMesh { %proxycode %{
  public void EndPolygon() {
    if (!m_isAddingPolygon) { throw new BadBracketingException("EndPolygon without matching BeginPolygon"); }
    m_isAddingPolygon = false;
    EndPolygonUnchecked();
  } %} }

%include "fbxsdk/scene/geometry/fbxmesh.h"
