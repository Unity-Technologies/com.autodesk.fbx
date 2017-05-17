// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/*
 * If you have a C++ class where all its functions and members are static,
 * declare it static in C#.
 */
%define %declare_static_class(THETYPE)
  %typemap(csclassmodifiers) THETYPE "public static class";
  %typemap(csinterfaces) THETYPE "";
  %typemap(csbody) THETYPE %{ %}
  /* Hack: We can't just not have the dispose function. But we *can* say that
   * it has a method modifier that's a comment. Which comments it out! */
  %typemap(csdestruct, methodname="Dispose", methodmodifiers="//") THETYPE %{ { } %}
  %typemap(csfinalize) THETYPE %{ %}
%enddef
