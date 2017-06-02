// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

// %define_unary_operator
// %define_binary_operator
// %define_commutative_binary_operator

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
