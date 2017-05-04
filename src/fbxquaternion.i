// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/*
 * FbxQuaternion
 *
 * Lots of operators to remap, since swig-3.0.12 doesn't do that.
 *
 * We provide almost the whole class.
 */
%reveal_all_start;

/* Hide the assignment operator, doesn't make sense in C#. */
%ignore FbxQuaternion::operator=;

/* Accessing elements. The C++ doesn't bounds-test, so we do. */
%ignore FbxQuaternion::operator[];
%rename("%sUnchecked") FbxQuaternion::GetAt;
%rename("%sUnchecked") FbxQuaternion::SetAt;
%csmethodmodifiers FbxQuaternion::GetAt "private";
%csmethodmodifiers FbxQuaternion::SetAt "private";
%extend FbxQuaternion { %proxycode %{
  public double GetAt(int index) { return this[index]; }
  public void SetAt(int index, double value) { this[index] = value; }
  public double this[int index] {
    get {
      if (index < 0 || index >= 4) { throw new System.IndexOutOfRangeException(); }
      return GetAtUnchecked(index);
    }
    set {
      if (index < 0 || index >= 4) { throw new System.IndexOutOfRangeException(); }
      SetAtUnchecked(index, value);
    }
  } %} }

/*
 * We create a bunch of C# operators because swig can't do it on its own.
 *
 * We rename the C++ to an ugly name and mark it private, then we provide
 * pretty C# operators.
 *
 * Unfortunately the in-place operator can't be overloaded in C#.
 */
%define %define_unary_operator(THETYPE, OPERATOR, CSNAME)
%csmethodmodifiers THETYPE::operator OPERATOR () const "private";
%rename("operator_"##"CSNAME") THETYPE::operator OPERATOR () const;
%extend THETYPE { %proxycode %{
  public static THETYPE operator OPERATOR (THETYPE a) {
    return a.operator_##CSNAME();
  }
%} }
%enddef

%define %define_binary_operator(THETYPE, OPERATOR, CSNAME, OPERANDTYPE)
%csmethodmodifiers THETYPE::operator OPERATOR (OPERANDTYPE) const "private";
%csmethodmodifiers THETYPE::operator OPERATOR (const OPERANDTYPE &) const "private";
%rename("operator_"##"CSNAME") THETYPE::operator OPERATOR (OPERANDTYPE) const;
%rename("operator_"##"CSNAME") THETYPE::operator OPERATOR (const OPERANDTYPE &) const;
%extend THETYPE { %proxycode %{
  public static THETYPE operator OPERATOR (THETYPE a, OPERANDTYPE b) {
    return a.operator_##CSNAME(b);
  }
%} }
%enddef

%define %define_commutative_binary_operator(THETYPE, OPERATOR, CSNAME, OPERANDTYPE)
%csmethodmodifiers THETYPE::operator OPERATOR (OPERANDTYPE) const "private";
%csmethodmodifiers THETYPE::operator OPERATOR (const OPERANDTYPE &) const "private";
%rename("operator_"##"CSNAME") THETYPE::operator OPERATOR (OPERANDTYPE) const;
%rename("operator_"##"CSNAME") THETYPE::operator OPERATOR (const OPERANDTYPE &) const;
%extend THETYPE { %proxycode %{
  public static THETYPE operator OPERATOR (THETYPE a, OPERANDTYPE b) {
    return a.operator_##CSNAME(b);
  }
  public static THETYPE operator OPERATOR (OPERANDTYPE a, THETYPE b) {
    return b.operator_##CSNAME(a);
  }
%} }
%enddef

%ignore FbxQuaternion::operator*=;
%ignore FbxQuaternion::operator/=;
%ignore FbxQuaternion::operator+=;
%ignore FbxQuaternion::operator-=;

/* Binary operators: scaling. */
%define_binary_operator(FbxQuaternion, *, Mul, FbxQuaternion);
%define_commutative_binary_operator(FbxQuaternion, *, Mul, double);
%define_binary_operator(FbxQuaternion, /, Div, FbxQuaternion);
%define_binary_operator(FbxQuaternion, /, Div, double);

/* Binary operators: addition/subtraction. */
%define_binary_operator(FbxQuaternion, +, Add, FbxQuaternion);
%define_binary_operator(FbxQuaternion, -, Sub, FbxQuaternion);
%define_binary_operator(FbxQuaternion, +, Add, double);
%define_binary_operator(FbxQuaternion, -, Sub, double);

/* Unary operator: negation. */
%define_unary_operator(FbxQuaternion, -, Negate);

/* Equality and hash code. */
%define_equality_from_operator(FbxQuaternion);
%csmethodmodifiers FbxQuaternion::GetHashCode "public override";
%extend FbxQuaternion {
  int GetHashCode() {
    uint64_t hash = 0;
    union {
      uint64_t u;
      double d;
    };
    for(int i = 0; i < 4; ++i) {
      hash = (hash << 16) | (hash >> 48);
      d = $self->GetAt(i);
      hash ^= u;
    }
    return int32_t(hash) ^ int32_t(hash >> 32);
  }
}

/* Hide the direct access to double* since we can't use it in C# anyway */
%ignore operator double* ();
%ignore operator const double* () const;

%include "fbxsdk/core/math/fbxquaternion.h"

%reveal_all_end;
