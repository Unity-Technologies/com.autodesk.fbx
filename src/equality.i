// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/*
 * This file has macros that allow easily setting up IEquatable<> and
 * IComparable<> proxies.
 */


/*
 * Add various functions that fulfill most of the IEquatable<THETYPE> interface.
 *
 * The caller of this macro must define C# functions:
 *      public bool THETYPE.Equals(THETYPE)
 *      public override int THETYPE.GetHashCode()
 *
 * This macro defines C# functions:
 *      public override bool THETYPE.Equals(object)
 *      public static bool operator == (THETYPE, THETYPE)
 *      public static bool operator != (THETYPE, THETYPE)
 *
 * The caller may also want to add IEquatable<THETYPE> to the interface list by declaring:
 *      %typemap(csinterfaces) THETYPE "IDisposable, IEquatable<THETYPE>";
 *
 * If you have a C++ operator== or equivalent, look at %define_equality_from_function.
 *
 * If you want reference equality, look at %define_pointer_equality_functions
 * (the standard C# reference equality won't match when you get two proxies to the
 * same C++ object; %define_pointer_equality_functions fixes that).
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
 * Add various functions that fulfill most of the IEquatable<THETYPE> interface.
 *
 * The caller of this macro must define C# function:
 *      public override int THETYPE.GetHashCode()
 * And C++ function:
 *      bool THETYPE::EQUALFN(const THETYPE&)
 *
 * This macro uses EQUALFN to define equality. It defines C# functions:
 *      private bool THETYPE._equals(THETYPE)
 *      public bool THETYPE.Equals(THETYPE)
 *      public override bool THETYPE.Equals(object)
 *      public static bool operator == (THETYPE, THETYPE)
 *      public static bool operator != (THETYPE, THETYPE)
 *
 * The caller may also want to add IEquatable<THETYPE> to the interface list by declaring:
 *      %typemap(csinterfaces) THETYPE "IDisposable, IEquatable<THETYPE>";
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
 * The caller of this macro must define C# function:
 *      public override int THETYPE.GetHashCode()
 * And be wrapping a C++ type with operator== defined.
 *
 * This macro uses operator== to define equality. It defines C# functions:
 *      private bool THETYPE._equals(THETYPE)
 *      public bool THETYPE.Equals(THETYPE)
 *      public override bool THETYPE.Equals(object)
 *      public static bool operator == (THETYPE, THETYPE)
 *      public static bool operator != (THETYPE, THETYPE)
 *
 * The caller may also want to add IEquatable<THETYPE> to the interface list by declaring:
 *      %typemap(csinterfaces) THETYPE "IDisposable, IEquatable<THETYPE>";
 */
%define %define_equality_from_operator(THETYPE)
%ignore THETYPE::operator!=;
%define_equality_from_function(THETYPE, operator==)
%enddef

/*
 * Add all the functions needed to fulfill the IEquatable<THETYPE> interface.
 *
 * The caller of this macro must be wrapping a C++ type where pointer equality
 * is a reasonable definition of equality.
 *
 * This macro uses the C++ pointer to define equality and the hash code. It
 * defines C# functions:
 *      public override bool THETYPE.Equals(object)
 *      public bool THETYPE.Equals(THETYPE)
 *      public static bool operator == (THETYPE, THETYPE)
 *      public static bool operator != (THETYPE, THETYPE)
 *      public override int THETYPE.GetHashCode()
 *
 * The caller may also want to add IEquatable<THETYPE> to the interface list by declaring:
 *      %typemap(csinterfaces) THETYPE "IDisposable, IEquatable<THETYPE>";
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

/*
 * Add functions that fulfill most of the IEquatable<THETYPE> and
 * IComparable<THETYPE> interfaces.
 *
 * The caller of this macro must define C# functions:
 *      public int THETYPE.CompareTo(THETYPE)
 *      public override int THETYPE.GetHashCode()
 *
 * This macro defines:
 *      public override bool THETYPE.Equals(object)
 *      public int THETYPE.CompareTo(THETYPE)
 *      public static bool operator <  (THETYPE, THETYPE)
 *      public static bool operator <= (THETYPE, THETYPE)
 *      public static bool operator == (THETYPE, THETYPE)
 *      public static bool operator != (THETYPE, THETYPE)
 *      public static bool operator >= (THETYPE, THETYPE)
 *      public static bool operator >  (THETYPE, THETYPE)
 *
 * We sort null as smallest.
 *
 * The caller may also want to add IEquatable and IComparable to the interface list by declaring:
 *      %typemap(csinterfaces) THETYPE "IDisposable, IEquatable<THETYPE>, IComparable<THETYPE>";
 */
%define %define_comparison_functions(THETYPE)
%extend THETYPE { %proxycode %{
  public override bool Equals(object obj) {
    if (object.ReferenceEquals(obj, null)) { return false; }
    if (! (obj is $csclassname)) { return false; }
    return CompareTo(($csclassname)obj) == 0;
  }
  public int CompareTo(object other) {
    if (object.ReferenceEquals(other, null)) { return 1; }
    if (! (other is $csclassname)) { throw new System.ArgumentException("other is not the same type as this instance of $csclassname"); }
    return CompareTo(($csclassname)other);
  }
  static int _compare($csclassname a, $csclassname b) {
    if (object.ReferenceEquals(a, b))    { return  0; }
    if (object.ReferenceEquals(a, null)) { return -1; }
    if (object.ReferenceEquals(b, null)) { return  1; }
    return a.CompareTo(b);
  }
  public static bool operator <  ($csclassname a, $csclassname b) { return _compare(a, b) <  0; }
  public static bool operator <= ($csclassname a, $csclassname b) { return _compare(a, b) <= 0; }
  public static bool operator == ($csclassname a, $csclassname b) { return _compare(a, b) == 0; }
  public static bool operator != ($csclassname a, $csclassname b) { return _compare(a, b) != 0; }
  public static bool operator >= ($csclassname a, $csclassname b) { return _compare(a, b) >= 0; }
  public static bool operator >  ($csclassname a, $csclassname b) { return _compare(a, b) >  0; }
  public bool Equals($csclassname other) { return CompareTo(other) == 0; }
%} }
%enddef
