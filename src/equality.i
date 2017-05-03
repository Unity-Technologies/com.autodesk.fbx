// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************


/*
 * Add various functions that fulfill most of the IEquatable<THETYPE> contract.
 *
 * THETYPE must also define Equals(THETYPE) and GetHashCode().
 * That's on you.
 *
 * If you have a C++ operator== or equivalent, look at %define_equality_from_function.
 *
 * If you want reference equality, look at %define_pointer_equality_functions
 * (the standard C# reference equality won't match when you get two proxies to the
 * same C++ object; %define_pointer_equality_functions fixes that).
 *
 * You might also want to add IEquatable<THETYPE> to the interface list:
 *   %typemap(csinterfaces) THETYPE "IDisposable, IEquatable<THETYPE>";
 * But that's on you as well.
 */
%define %define_generic_equality_functions(THETYPE)
%extend THETYPE { %proxycode %{
  public override bool Equals(object obj){
    if (object.ReferenceEquals(obj, null)) { return false; }
    /* is obj a subclass of this type; if so use our Equals */
    var typed = obj as $csclassname;
    if (!object.ReferenceEquals(typed, null)) {
      return this.Equals(typed);
    }
    /* are we a subclass of the other type; if so use their Equals */
    if (typeof($csclassname).IsSubclassOf(obj.GetType())) {
      return obj.Equals(this);
    }
    /* types are unrelated; can't be a match */
    return false;
  }

  public static bool operator == ($csclassname a, $csclassname b) {
    if (object.ReferenceEquals(a, b)) { return true; }
    if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null)) { return false; }
    return a.Equals(b);
  }

  public static bool operator != ($csclassname a, $csclassname b) {
    return !(a == b);
  }
%} }
%enddef

/*
 * Given a function:
 *    bool THETYPE::EQUALFN(const THETYPE&)
 * use that to define equality, and create all the C# Equals functions and
 * operators you'd expect.
 *
 * You need to define GetHashCode.
 */
%define %define_equality_from_function(THETYPE, EQUALFN)
%rename ("_equals") THETYPE::EQUALFN;
%csmethodmodifiers THETYPE::EQUALFN "private";
%extend THETYPE { %proxycode %{
  public bool Equals($csclassname other) {
    if (object.ReferenceEquals(other, null)) { return false; }
    return _equals(other);
  }
%} }
%define_generic_equality_functions(THETYPE)
%enddef

/*
 * Use operator== to define equality. Hide operator!=.
 *
 * You need to define GetHashCode.
 */
%define %define_equality_from_operator(THETYPE)
%ignore THETYPE::operator!=;
%define_equality_from_function(THETYPE, operator==)
%enddef

/*
 * Add a GetHashCode() and Equals() function to allow
 * us to perform identity tests in C#.
 *
 * Uses the swigCPtr to check for equality.
 */
%define %define_pointer_equality_functions(THETYPE)
%extend THETYPE { %proxycode %{
  public override int GetHashCode(){
      return swigCPtr.Handle.GetHashCode();
  }

  public bool Equals($csclassname other) {
    if (object.ReferenceEquals(other, null)) { return false; }
    return this.swigCPtr.Handle.Equals (other.swigCPtr.Handle);
  }
%} }
%define_generic_equality_functions(THETYPE)
%enddef
