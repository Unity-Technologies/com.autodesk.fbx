// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/* We are included with ignores off, so that we can generate the template code.
 * But that means we need to ignore a lot of stuff. */


/* Use the more efficient GetName implementation, and map ToString to it. */
%ignore FbxProperty::GetName() const;
%rename("GetName") FbxProperty::GetNameAsCStr() const;
%define_tostring(FbxProperty, GetName());

/* Create has an optional output when looking for duplicates. */
/* %ignore FbxProperty::Create; */
%apply bool *OUTPUT { bool* pWasFound };


/*
 * Define equality and hash code.
 *
 * Ignore equality to an int. Make users call IsValid instead.
 */
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

/*
 * These are the functions we've taken in, carefully, one by one.
 */

/*
%ignore FbxProperty::Destroy;
%ignore FbxProperty::DestroyChildren;
%ignore FbxProperty::DestroyRecursively;
%ignore FbxProperty::GetPropertyDataType;
%ignore FbxProperty::IsValid;
*/

/* TODO: take more of this stuff in! */
%ignore FbxProperty::CreateFrom;
%ignore FbxProperty::SetUserTag;
%ignore FbxProperty::GetUserTag;
%ignore FbxProperty::SetUserDataPtr;
%ignore FbxProperty::GetUserDataPtr;
%ignore FbxProperty::ModifyFlag;
%ignore FbxProperty::GetFlag;
%ignore FbxProperty::GetFlags;
%ignore FbxProperty::GetFlagInheritType;
%ignore FbxProperty::SetFlagInheritType;
%ignore FbxProperty::ModifiedFlag;
%ignore FbxProperty::CompareValue;
%ignore FbxProperty::CopyValue;
%ignore FbxProperty::HasDefaultValue;
%ignore FbxProperty::GetValueInheritType;
%ignore FbxProperty::SetValueInheritType;
%ignore FbxProperty::Modified;
%ignore FbxProperty::SupportSetLimitAsDouble;
%ignore FbxProperty::SetMinLimit;
%ignore FbxProperty::HasMinLimit;
%ignore FbxProperty::GetMinLimit;
%ignore FbxProperty::SetMaxLimit;
%ignore FbxProperty::HasMaxLimit;
%ignore FbxProperty::GetMaxLimit;
%ignore FbxProperty::SetLimits;
%ignore FbxProperty::AddEnumValue;
%ignore FbxProperty::InsertEnumValue;
%ignore FbxProperty::GetEnumCount;
%ignore FbxProperty::SetEnumValue;
%ignore FbxProperty::RemoveEnumValue;
%ignore FbxProperty::GetEnumValue;
%ignore FbxProperty::IsRoot;
%ignore FbxProperty::IsChildOf;
%ignore FbxProperty::IsDescendentOf;
%ignore FbxProperty::GetParent;
%ignore FbxProperty::GetChild;
%ignore FbxProperty::GetSibling;
%ignore FbxProperty::GetFirstDescendent;
%ignore FbxProperty::GetNextDescendent;
%ignore FbxProperty::Find;
%ignore FbxProperty::FindHierarchical;
%ignore FbxProperty::BeginCreateOrFindProperty;
%ignore FbxProperty::EndCreateOrFindProperty;
%ignore FbxProperty::FbxPropertyNameCache;
%ignore FbxProperty::GetAnimationEvaluator;
%ignore FbxProperty::IsAnimated;
%ignore FbxProperty::EvaluateValue;
%ignore FbxProperty::CreateCurveNode;
%ignore FbxProperty::GetCurveNode;
%ignore FbxProperty::GetCurve;
%ignore FbxProperty::ConnectSrcObject;
%ignore FbxProperty::IsConnectedSrcObject;
%ignore FbxProperty::DisconnectSrcObject;
%ignore FbxProperty::DisconnectAllSrcObject;
%ignore FbxProperty::GetSrcObjectCount;
%ignore FbxProperty::GetSrcObject;
%ignore FbxProperty::FindSrcObject;
%ignore FbxProperty::ConnectDstObject;
%ignore FbxProperty::IsConnectedDstObject;
%ignore FbxProperty::DisconnectDstObject;
%ignore FbxProperty::DisconnectAllDstObject;
%ignore FbxProperty::GetDstObjectCount;
%ignore FbxProperty::GetDstObject;
%ignore FbxProperty::FindDstObject;
%ignore FbxProperty::ConnectSrcProperty;
%ignore FbxProperty::IsConnectedSrcProperty;
%ignore FbxProperty::DisconnectSrcProperty;
%ignore FbxProperty::GetSrcPropertyCount;
%ignore FbxProperty::GetSrcProperty;
%ignore FbxProperty::FindSrcProperty;
%ignore FbxProperty::ConnectDstProperty;
%ignore FbxProperty::IsConnectedDstProperty;
%ignore FbxProperty::DisconnectDstProperty;
%ignore FbxProperty::GetDstPropertyCount;
%ignore FbxProperty::GetDstProperty;
%ignore FbxProperty::FindDstProperty;
%ignore FbxProperty::ClearConnectCache;
%ignore FbxProperty::operator=;
%ignore FbxProperty::sHierarchicalSeparator;

%ignore FbxPropertyT::StaticInit;

/* We likely don't want these ever. */
%ignore FbxProperty::operator<;
%ignore FbxProperty::operator>;
%ignore FbxProperty::FbxProperty;
%ignore FbxProperty::FbxProperty(const FbxProperty&);
%ignore FbxPropertyT::FbxPropertyT;
%ignore FbxPropertyT::FbxPropertyT(const FbxPropertyT&);
%ignore FbxPropertyT::operator T;

/*
 * We don't want to provide setters for properties, because we don't have operator=.
 * Solution: use %fbximmutable to declare a property.
 *
 * But in case you forget, this creates a warning 844 in swig and will fail to
 * compile in C#. */
%typemap("csvarin") const FbxPropertyT& {#error this should be a %fbximmutable}


/***************************************************************************/
%include "fbxsdk/core/fbxproperty.h"
/***************************************************************************/

%template(Set) FbxProperty::Set<float>;

%template("FbxPropertyDouble3") FbxPropertyT<FbxDouble3>;
%template("FbxPropertyDouble") FbxPropertyT<FbxDouble>;
%template("FbxPropertyString") FbxPropertyT<FbxString>;
%template("FbxPropertyBool") FbxPropertyT<FbxBool>;
