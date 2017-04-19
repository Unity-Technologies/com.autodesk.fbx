// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
/* We marshal FbxString using char*. */
%typemap(ctype) FbxString, const FbxString& "char *"

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
        const FbxString&, FbxString "string"

/* When we use FbxString in C# we use string instead. */
%typemap(cstype) FbxString, const FbxString& "string"

/* An argument of type FbxString or const FbxString& gets converted like this... */
%typemap(csin) FbxString, const FbxString& "$csinput"

// If we take in an FbxString or FbxString*, the input is a pointer to FbxString.
%typemap(in, canthrow=1) FbxString
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null string", 0);
    return $null;
   }
   $1 = $input; %}
// If we take in a const-ref FbxString, the input is a char* but needs to be converted to FbxString.
%typemap(in, canthrow=1) const FbxString&
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "null string", 0);
    return $null;
   }
   FbxString $1_copy($input);
   $1 = &$1_copy; %}

/* A return value of type FbxString gets converted like this... */
%typemap(out) FbxString, const FbxString& %{ $result = SWIG_csharp_string_callback($1.Buffer()); %}
%typemap(csout, excode=SWIGEXCODE) FbxString {
    string ret = $imcall;$excode
    return ret;
  }

/* Setter/getter. Why SWIGEXCODE2? Dunno... */
%typemap(csvarin, excode=SWIGEXCODE2) FbxString %{
    set {
      $imcall;$excode
    } %}
%typemap(csvarout, excode=SWIGEXCODE2) FbxString %{
    get {
      string ret = $imcall;$excode
      return ret;
    } %}
