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

%enddef
