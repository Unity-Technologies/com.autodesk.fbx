// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
/* We marshal FbxString using char*. */
%typemap(ctype) FbxString "char *"

/*
 * When we get the char* in C# we marshal it with this code.
 * FBX uses UTF-8, both for FbxString and for char* strings.
 * TODO: use the .net 4.5 UnmanagedType.LPUTF8Str
 * For compatibility with .net 2.0 we use LPStr (which is ANSI) which might
 * lead to undefined behaviour with chinese, japanese, etc.
 */
%typemap(imtype,
         inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]",
         outattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]")
        char * "string"
%typemap(imtype,
         inattributes="[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]",
         outattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]")
        FbxString "string"

/* When we use FbxString in C# we use string instead. */
%typemap(cstype) FbxString "string"

/* An argument of type FbxString gets converted like this... */
%typemap(csin) FbxString "$csinput"
%typemap(in, canthrow=1) FbxString
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null string", 0);
    return $null;
   }
   $1 = $input; %}

/* A return value of type FbxString gets converted like this... */
%typemap(out) FbxString %{ $result = SWIG_csharp_string_callback($1.Buffer()); %}
%typemap(csout, excode=SWIGEXCODE) FbxString {
    string ret = $imcall;$excode
    return ret;
  }

/*
 * Now all that again but for const FbxString&.
 */

%typemap(ctype) const FbxString& "char *"
%typemap(imtype,
         inattributes="[global::System.Runtime.InteropServices.MarshalAs(UnmanagedType.LPUTF8Str)]",
         outattributes="[return: global::System.Runtime.InteropServices.MarshalAs(UnmanagedType.LPUTF8Str)]")
        const FbxString& "string"

/* When we use FbxString in C# we use string instead. */
%typemap(cstype) const FbxString& "string"

/* An argument of type FbxString gets converted like this... */
%typemap(csin) const FbxString& "$csinput"
%typemap(in, canthrow=1) const FbxString&
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null string", 0);
    return $null;
   }
   $1 = $input; %}

/* A return value of type FbxString gets converted like this... */
%typemap(out) const FbxString& %{ $result = SWIG_csharp_string_callback($1.Buffer()); %}
%typemap(csout, excode=SWIGEXCODE) const FbxString& {
    string ret = $imcall;$excode
    return ret;
  }

/*
%typemap(in, canthrow=1) const FbxString &
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null string", 0);
    return $null;
   }
   $*1_ltype $1_str($input);
   $1 = &$1_str; %}
*/

/* Setter/getter. Why SWIGEXCODE2? Dunno... */
%typemap(csvarin, excode=SWIGEXCODE2) const FbxString & %{
    set {
      $imcall;$excode
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) FbxString %{
    get {
      string ret = $imcall;$excode
      return ret;
    } %}
