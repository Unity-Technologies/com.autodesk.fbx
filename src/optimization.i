// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/**
 * If you have a C++ type that you've hand-implemented in C#
 * in such a way as the struct layout is identical, declare that
 * here so that it gets passed correctly between C++ and C#.
 *
 * You need three classes:
 * - CPPTYPE is the C++ type that functions you're wrapping are declaring that
 *   they take or return.
 * - CPPTRANSITTYPE is a type that
 *      (a) exactly matches the C# layout
 *      (b) implicitly converts to and from CPPTYPE
 *      (c) is a standard-layout class according to c++11 (implicit no-arg ctor,
 *              and essentially no inheritance allowed)
 * - CSTYPE is the name of the type in C#. It must exactly match CPPTRANSITTYPE.
 *
 * Only do this if you have a use case that definitely requires the boost in
 * performance. You have to be extremely careful. See e.g.:
 *      http://www.mono-project.com/docs/advanced/pinvoke/
 */
%define %declare_hand_optimized_type(CPPTYPE, CPPTRANSITTYPE, CSTYPE)
%ignore CPPTYPE; /* Don't create a swig wrapper for the type. */
%typemap(ctype) CPPTYPE, const CPPTYPE& "CPPTRANSITTYPE";
%typemap(imtype) CPPTYPE, const CPPTYPE& "CSTYPE";
%typemap(cstype) CPPTYPE, const CPPTYPE& "CSTYPE";
%typemap(csin) CPPTYPE, const CPPTYPE& "$csinput";
%typemap(in) CPPTYPE %{$1 = $input;%}
%typemap(in) const CPPTYPE& %{$*1_ltype $1_temp = $input; $1 = &$1_temp; %}
%typemap(out, null="CPPTRANSITTYPE()") CPPTYPE %{$result = $1;%}
%typemap(out, null="CPPTRANSITTYPE()") const CPPTYPE& %{$result = *$1;%}
%typemap(csout, excode=SWIGEXCODE) CPPTYPE, const CPPTYPE& {
    var ret = $imcall;$excode
    return ret;
  }
%typemap(csvarin, excode=SWIGEXCODE2) CPPTYPE %{
    set {
      $imcall;$excode
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) const CPPTYPE& %{
    get {
      var ret = $imcall;$excode
      return ret;
    } %}

/**
 * Define OUTPUT and INOUT typemaps for a hand-optimized type.
 *
 * This uses the InOutReferenceManager defined below.
 */
%typemap(ctype) CPPTYPE & OUTPUT, CPPTYPE & INOUT, CPPTYPE * OUTPUT, CPPTYPE * INOUT "CPPTRANSITTYPE *";

%typemap(imtype) CPPTYPE & OUTPUT, CPPTYPE * OUTPUT "out CSTYPE";
%typemap(imtype) CPPTYPE & INOUT, CPPTYPE *INTOUT "ref CSTYPE";

%typemap(cstype) CPPTYPE & OUTPUT, CPPTYPE * OUTPUT "out CSTYPE";
%typemap(cstype) CPPTYPE & INOUT, CPPTYPE *INTOUT "ref CSTYPE";

%typemap(csin) CPPTYPE & OUTPUT, CPPTYPE * OUTPUT "out $csinput";
%typemap(csin) CPPTYPE & INOUT, CPPTYPE * INOUT "ref $csinput";

%typemap(in) CPPTYPE & OUTPUT, CPPTYPE * OUTPUT %{
    InOutReferenceManager<CPPTYPE, CPPTRANSITTYPE> $1_refmgr($input, /* isInOut */ false);
    $1 = $1_refmgr.GetCppPointer(); %}
%typemap(in) CPPTYPE & INOUT, CPPTYPE * INOUT %{
    InOutReferenceManager<CPPTYPE, CPPTRANSITTYPE> $1_refmgr($input, /* isInOut */ true);
    $1 = $1_refmgr.GetCppPointer(); %}
%enddef


%{
/**
 * See optimization.i
 *
 * This type is used to handle INOUT and OUTPUT arguments for hand-optimized
 * types. In a hand-optimized type, there's a transit type that perfectly matches
 * the C# layout, and there's a native C++ type that doesn't necessarily match it.
 *
 * To handle INOUT or OUTPUT arguments, we need:
 * - from C#, a pointer of type CPPTRANSITTYPE that corresponds to the memory
 *      location of the C# out argument.
 * - for the C++ library we're wrapping, a reference or pointer of type CPPTYPE
 *      for the library to put its result into
 * - if it's a INOUT argument, a way to initialize the CPPTYPE from the CPPTRANSITTYPE
 * - when we return from the C++ wrapper, a way to copy from the CPPTYPE to the CPPTRANSITTYPE
 */
template <class CPPTYPE, class CPPTRANSITTYPE>
struct InOutReferenceManager {
  CPPTYPE cpp_copy;
  CPPTRANSITTYPE *csharp_copy;

  InOutReferenceManager(CPPTRANSITTYPE *csharp_copy, bool isInOut) : csharp_copy(csharp_copy)
  {
    if (isInOut) {
        /* Initialize the native type from the transit type, but only if it's
         * a 'ref' argument (not an 'out' argument). */
        cpp_copy = *csharp_copy;
    }
  }

  ~InOutReferenceManager() {
    /* Update the C# data from the C++ data on the end of the scope. */
    *csharp_copy = cpp_copy;
  }

  /* Convert to the pointer type so that SWIG can get the pointer it wants */
  CPPTYPE * GetCppPointer() { return &cpp_copy; }
};
%}

/**
 * Now declare all the hand-optimized types.
 *
 * We need to do this before using them, or else we get SWIGTYPEs.
 */

%{
#include "optimized/FbxDoubleTemplates.h"
%}
%declare_hand_optimized_type(FbxColor, FbxSharpDouble4, FbxColor);
%declare_hand_optimized_type(FbxVectorTemplate2<double>, FbxSharpDouble2, FbxDouble2);
%declare_hand_optimized_type(FbxVectorTemplate3<double>, FbxSharpDouble3, FbxDouble3);
%declare_hand_optimized_type(FbxVectorTemplate4<double>, FbxSharpDouble4, FbxDouble4);
%declare_hand_optimized_type(FbxVector2, FbxSharpDouble2, FbxVector2);
%declare_hand_optimized_type(FbxVector4, FbxSharpDouble4, FbxVector4);
