// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%ignore FbxMin;
%ignore FbxMax;

/* First we start with the optimized vectors. */
%{
#include "optimized/FbxDoubleTemplates.h"
%}
%declare_hand_optimized_type(FbxVectorTemplate2<double>, FbxSharpDouble2, FbxDouble2);
%declare_hand_optimized_type(FbxVectorTemplate3<double>, FbxSharpDouble3, FbxDouble3);
%declare_hand_optimized_type(FbxVectorTemplate4<double>, FbxSharpDouble4, FbxDouble4);

/* Then we provide support for the non-optimized types. */
%define %rename_vector_operators(THETYPE, N)

/* No operator=, just a copy constructor */
%ignore THETYPE::operator=;
%extend THETYPE {
  THETYPE(const THETYPE<T>& other) {
    THETYPE<T> *self = new THETYPE<T>();
    *self = other;
    return self;
  }
}

/* Handle equality in C#. Also define a ToString operation. */
%csmethodmodifiers THETYPE::operator== "private";
%rename("_equals") THETYPE::operator==;
%ignore THETYPE::operator!=;
%define_generic_equality_functions(THETYPE);
%extend THETYPE {
  %proxycode %{
  public bool Equals($csclassname other) {
    if (object.ReferenceEquals(other, null)) { return false; }
    return _equals(other);
  }
  public override int GetHashCode() {
    uint hash = 0;
    for(int i = 0; i < N; ++i) {
      hash = (hash << (32 / N)) | (hash >> (32 - (32 / N)));
      hash ^= (uint)this[i].GetHashCode();
    }
    return (int)hash;
  }

  public override string ToString() {
    var builder = new System.Text.StringBuilder("$csclassname(");
    for(int i = 0; i < N; ++i) {
      builder.Append(this[i].ToString());
      builder.Append(',');
    }
    builder.Replace(',', ')', builder.Length - 1, 1);
    return builder.ToString();
  }
  %}
}

/* Store the data */
%ignore THETYPE::Buffer;
%ignore THETYPE::mData;
%ignore operator THETYPE<T>&;
%ignore THETYPE::operator[];
%extend THETYPE {
  %csmethodmodifiers _get "private";
  %csmethodmodifiers _set "private";
  const T& _get(int i) const { return self->mData[i]; }
  void _set(int i, const T& v) { self->mData[i] = v; }
  %proxycode %{
  public $typemap(cstype, T) this[int index] {
    get {
      if (index < 0 || index >= N) {
        throw new System.IndexOutOfRangeException();
      } else {
        return this._get(index);
      }
    }
    set {
      if (index < 0 || index >= N) {
        throw new System.IndexOutOfRangeException();
      } else {
        this._set(index, value);
      }
    }
  }
  %}
}
%enddef

/* Provide X/Y/Z/W properties to look more C#-like. */
%define %implement_vector_variables(THETYPE, NAME, INDEX)
%extend THETYPE {
  %proxycode %{
  public $typemap(cstype, T) NAME {
    get { return this._get(INDEX); }
    set { this._set(INDEX, value); }
  }
  %}
}
%enddef

%rename_vector_operators(FbxVectorTemplate2, 2);
%implement_vector_variables(FbxVectorTemplate2, X, 0);
%implement_vector_variables(FbxVectorTemplate2, Y, 1);

%rename_vector_operators(FbxVectorTemplate3, 3);
%implement_vector_variables(FbxVectorTemplate3, X, 0);
%implement_vector_variables(FbxVectorTemplate3, Y, 1);
%implement_vector_variables(FbxVectorTemplate3, Z, 2);

%rename_vector_operators(FbxVectorTemplate4, 4);
%implement_vector_variables(FbxVectorTemplate4, X, 0);
%implement_vector_variables(FbxVectorTemplate4, Y, 1);
%implement_vector_variables(FbxVectorTemplate4, Z, 2);
%implement_vector_variables(FbxVectorTemplate4, W, 3);

/* We aren't ignoring everything, so we need to explicitly ignore all these
 * constants. */
%ignore FBXSDK_CHAR_MIN;
%ignore FBXSDK_CHAR_MAX;
%ignore FBXSDK_UCHAR_MIN;
%ignore FBXSDK_UCHAR_MAX;
%ignore FBXSDK_SHORT_MIN;
%ignore FBXSDK_SHORT_MAX;
%ignore FBXSDK_USHORT_MIN;
%ignore FBXSDK_USHORT_MAX;
%ignore FBXSDK_INT_MIN;
%ignore FBXSDK_INT_MAX;
%ignore FBXSDK_UINT_MIN;
%ignore FBXSDK_UINT_MAX;
%ignore FBXSDK_LONG_MIN;
%ignore FBXSDK_LONG_MAX;
%ignore FBXSDK_ULONG_MIN;
%ignore FBXSDK_ULONG_MAX;
%ignore FBXSDK_LONGLONG_MIN;
%ignore FBXSDK_LONGLONG_MAX;
%ignore FBXSDK_ULONGLONG_MIN;
%ignore FBXSDK_ULONGLONG_MAX;
%ignore FBXSDK_FLOAT_MIN;
%ignore FBXSDK_FLOAT_MAX;
%ignore FBXSDK_FLOAT_EPSILON;
%ignore FBXSDK_DOUBLE_MIN;
%ignore FBXSDK_DOUBLE_MAX;
%ignore FBXSDK_DOUBLE_EPSILON;
%ignore FBXSDK_TOLERANCE;
%ignore FBXSDK_SYSTEM_IS_LP64;
%ignore FBXSDK_REF_MIN;
%ignore FBXSDK_REF_MAX;
%ignore FBXSDK_ATOMIC_MIN;
%ignore FBXSDK_ATOMIC_MAX;


%include "fbxsdk/core/arch/fbxtypes.h"

/* Templates must *follow* the include, unlike everything else in swig. */
%template("FbxDouble2") FbxVectorTemplate2<double>;
%template("FbxDouble3") FbxVectorTemplate3<double>;
%template("FbxDouble4") FbxVectorTemplate4<double>;
%template("FbxDouble4x4") FbxVectorTemplate4<FbxDouble4>;
