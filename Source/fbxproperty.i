// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/*
 * Template classes (like FbxPropertyT) and %ignore "" don't mix well: the
 * templates can't be found again.
 *
 * In fbxsdk.i this file is included before the %ignore "".
 *
 * We still want to default to ignoring functions and classes, but we need to
 * do that per class.
 */
%rename("$ignore", regextarget=1, fullname=1) "FbxProperty::.*";
%rename("$ignore", regextarget=1, fullname=1) "FbxPropertyT::.*";

/***************************************************************************
 * Define equality and hash code.
 ***************************************************************************/
%define_equality_from_operator(FbxProperty);
%ignore FbxProperty::operator==(int) const;
%extend FbxProperty { %proxycode %{
  public override int GetHashCode() {
    uint hash = (uint) GetName().GetHashCode();
    hash = (hash << 11) | (hash >> 21);
    hash ^= (uint) GetPropertyDataType().GetHashCode();
    var obj = GetFbxObject();
    if (obj != null) {
      hash = (hash << 11) | (hash >> 21);
      hash ^= (uint) obj.GetHashCode();
    }
    return (int) hash;
  } %} }

/***************************************************************************
 * Create/destroy
 ***************************************************************************/

/* Create has an optional output when looking for duplicates. */
%apply bool *OUTPUT { bool* pWasFound };

%rename("%s") FbxProperty::Create;
%rename("%s") FbxProperty::Destroy;
%rename("%s") FbxProperty::DestroyChildren;
%rename("%s") FbxProperty::DestroyRecursively;

/***************************************************************************
 * Property metadata
 ***************************************************************************/
%rename("%s") FbxProperty::GetPropertyDataType;
%rename("%s") FbxProperty::IsValid;

%ignore FbxProperty::GetName() const; // returns FbxString
%rename("GetName") FbxProperty::GetNameAsCStr() const; // returns const char*, faster to convert
%define_tostring(FbxProperty, GetName());

%rename("%s") FbxProperty::GetHierarchicalName;
%rename("%s") FbxProperty::GetLabel;
%rename("%s") FbxProperty::SetLabel;

%rename("%s") FbxProperty::GetFbxObject;

/***************************************************************************
 * Flags.
 ***************************************************************************/
%rename("%s") FbxProperty::ModifyFlag;
%rename("%s") FbxProperty::GetFlag;
%rename("%s") FbxProperty::GetFlags;
%rename("%s") FbxProperty::GetFlagInheritType;
%rename("%s") FbxProperty::SetFlagInheritType;
%rename("%s") FbxProperty::ModifiedFlag;

/*
 * We also need to take in fbxpropertydef so we get the FbxPropertyFlags enums.
 *
 * FbxPropertyFlags and FbxPropertyValue are internal classes; we don't really
 * want them. But we do want the enums in FbxPropertyFlags.
 *
 * Since what we're exposing of FbxPropertyFlags is only the enums, we declare
 * it as a static class.
 */
%ignore FbxPropertyValue;
%rename("$ignore", fullname=1, regextarget=1, %$not %$isenum, %$not %$isenumitem) "FbxPropertyFlags::.*";
%declare_static_class(FbxPropertyFlags);
%include "fbxsdk/core/fbxpropertydef.h"

/***************************************************************************
 * Connecting to objects
 ***************************************************************************/
%rename("%s") FbxProperty::ConnectSrcObject;
%rename("%s") FbxProperty::IsConnectedSrcObject;
%rename("%s") FbxProperty::DisconnectSrcObject;

// TODO: support the versions of these functions that take FbxCriteria
%rename("%s") FbxProperty::DisconnectAllSrcObject();
%rename("%s") FbxProperty::GetSrcObjectCount() const;
%rename("%s") FbxProperty::GetSrcObject(const int pIndex=0) const;
%rename("FindSrcObjectInternal") FbxProperty::FindSrcObject(const char* pName, const int pStartIndex) const;
%csmethodmodifiers FbxProperty::FindSrcObject "private";
%extend FbxProperty { %proxycode %{
  public FbxObject FindSrcObject(string pName, int pStartIndex = 0) {
    if (pName == null) { throw new System.ArgumentNullException("pName"); }
    return FindSrcObjectInternal(pName, pStartIndex);
  }
%} }


%rename("%s") FbxProperty::ConnectDstObject;
%rename("%s") FbxProperty::IsConnectedDstObject;
%rename("%s") FbxProperty::DisconnectDstObject;

// TODO: support the versions of these functions that take FbxCriteria
%rename("%s") FbxProperty::DisconnectAllDstObject();
%rename("%s") FbxProperty::GetDstObjectCount() const;
%rename("%s") FbxProperty::GetDstObject(const int pIndex=0) const;
%rename("FindDstObjectInternal") FbxProperty::FindDstObject(const char* pName, const int pStartIndex) const;
%csmethodmodifiers FbxProperty::FindDstObject "private";
%extend FbxProperty { %proxycode %{
  public FbxObject FindDstObject(string pName, int pStartIndex = 0) {
    if (pName == null) { throw new System.ArgumentNullException("pName"); }
    return FindDstObjectInternal(pName, pStartIndex);
  }
%} }

/***************************************************************************
 * Get/set. See also the %template after the %include.
 ***************************************************************************/
%rename("Set") FbxProperty::Set; // Actually just for Set<float>
%rename("%s") FbxProperty::GetFloat; // Actually just for Get<float>
%rename("%s") FbxProperty::GetBool;
%rename("%s") FbxProperty::GetDouble;
%rename("%s") FbxProperty::GetFbxDouble3;
%rename("%s") FbxProperty::GetString;
%rename("%s") FbxProperty::GetFbxColor;
%rename("%s") FbxProperty::GetInt;
%rename("%s") FbxProperty::GetCurve;
%rename("%s") FbxProperty::GetCurveNode;
%rename("%s") FbxProperty::GetInt;
%rename("%s") FbxProperty::GetFbxColor;

%extend FbxProperty {
    float GetFloat () const { return $self->Get<float>(); }
    FbxBool GetBool () const { return $self->Get<FbxBool>(); }
    FbxDouble GetDouble () const { return $self->Get<FbxDouble>(); }
    FbxDouble3 GetFbxDouble3 () const { return $self->Get<FbxDouble3>(); }
    FbxString GetString () const { return $self->Get<FbxString>(); }
    FbxColor GetFbxColor () const { return $self->Get<FbxColor>(); }
    int GetInt() const { return $self->Get<int>(); }
}

%rename("%s") FbxPropertyT::Get;
%rename("%s") FbxPropertyT::Set;
%rename("%s") FbxPropertyT::EvaluateValue;

/***************************************************************************
 * There's no assignment operator for FbxProperty and FbxPropertyT.
 * All the member variables that are properties need to be marked
 * %fbximmutable.
 *
 * In case we forget, we will emit a custom warning 999 (and an 844, and a C#
 * compile error).
 ***************************************************************************/
%define SWIGWARN_FBXSHARP_MUTABLE_PROPERTY_MSG "999:FbxProperty variable should be marked %fbximmutable" %enddef
%typemap("csvarin",warning=SWIGWARN_FBXSHARP_MUTABLE_PROPERTY_MSG) const FbxPropertyT& {#error this should have been marked %fbximmutable}

/***************************************************************************/
%include "fbxsdk/core/fbxproperty.h"
/***************************************************************************/

%template(Set) FbxProperty::Set<FbxColor>;
%template(Set) FbxProperty::Set<float>;
%template(Set) FbxProperty::Set<FbxString>;
%template(EvaluateValue) FbxProperty::EvaluateValue<float>;

/* Generic properties */
%template("FbxPropertyBool") FbxPropertyT<FbxBool>;
%template("FbxPropertyDouble") FbxPropertyT<FbxDouble>;
%template("FbxPropertyDouble3") FbxPropertyT<FbxDouble3>;
%template("FbxPropertyInt") FbxPropertyT<signed int>;

%csmethodmodifiers FbxPropertyT<FbxString>::Set(const FbxString&) "public new";
%template("FbxPropertyString") FbxPropertyT<FbxString>;

%csmethodmodifiers FbxPropertyT<float>::Set(const float&) "public new";
%template("FbxPropertyFloat") FbxPropertyT<float>;

/* NodeAttribute and subclasses properties */
%template("FbxPropertyEProjectionType") FbxPropertyT<FbxCamera::EProjectionType>;
%template("FbxPropertyELightType") FbxPropertyT<FbxLight::EType>;
%template("FbxPropertyEAreaLightShape") FbxPropertyT<FbxLight::EAreaLightShape>;
%template("FbxPropertyEDecayType") FbxPropertyT<FbxLight::EDecayType>;
%template("FbxPropertyMarkerELook") FbxPropertyT<FbxMarker::ELook>;
%template("FbxPropertyNullELook") FbxPropertyT<FbxNull::ELook>;
%template("FbxPropertyEInheritType") FbxPropertyT<FbxTransform::EInheritType>;

/* Texture properties */
%template("FbxPropertyEBlendMode") FbxPropertyT<FbxTexture::EBlendMode>;
%template("FbxPropertyEWrapMode") FbxPropertyT<FbxTexture::EWrapMode>;

/* Camera properties */
%template("FbxPropertyEGateFit") FbxPropertyT<FbxCamera::EGateFit>;
