// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
/* In order to comply to the FDG, we want the intermediary class to be named NativeMethods (not GlobalsPInvoke)
 * https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/index
 * https://msdn.microsoft.com/library/ms182161.aspx
 */
%module (imclassname="NativeMethods") Globals

%module(directors="1") Globals
%{
#include "fbxsdk.h"
%}
/* TODO: just remove this; we aren't testing whether it works without. */
#define IGNORE_ALL_INCLUDE_SOME

/* helpers for defining equality correctly */
%include "equality.i"

/* helpers for defining arithmetic operators correctly */
%include "operators.i"

/* Helper for defining static classes correctly */
%include "staticclass.i"

/* Helper for defining ToString() correctly:
 *   %define_tostring(FbxDataType, GetName())
 * Defines a C# function FbxDataType.ToString() that returns GetName()
 *
 * Note the ... is so that you can pass arguments to the function, e.g.:
 *   %define_tostring(FbxDataType, GetName(a,b))
 */
%define %define_tostring(THETYPE, THECALL...)
%extend THETYPE { %proxycode %{
  public override string ToString() {
    return THECALL;
  } %} }
%enddef

/*
 * Swig support for ref and out arguments. E.g.:
 *   %apply int & OUTPUT { int & pMajor };
 * will make a function with an int& argument called pMajor
 * have that argument in C# be an 'out int' parameter.
 *
 * Using INOUT instead of OUTPUT will have it be a C# 'ref int' parameter.
 */
%include typemaps.i

/*
 * Often we want to reveal a variable but leave it immutable.
 * Declare it like
 *    %fbximmutable(FbxSurfacePhong::Specular)
 * Check the support in the CMakeLists.txt to discover immutable fields
 * automatically.
 */
%define %fbximmutable(THENAME)
%immutable THENAME;
%rename("%s") THENAME;
%enddef

/*
 * For the most part we hide everything except what we explicitly want.
 * But for some classes we just want everything.
 *
 * Use %reveal_all_start to enable including everything.
 *      Then @include the header file.
 * Use %reveal_all_end at the end of your file, or else parsing will
 *      break afterwards.
 */

%define %reveal_all_start
  /* Export everything under its default name. */
  %rename("%s") "";

  /* Don't export anything marked deprecated. */
  %ignore __declspec(deprecated);
%enddef

%define %reveal_all_end
  /* Don't export, unless a more specific rule says we should. */
  %ignore "";

  /* Export all the enum items. They're only actually exported if the enum itself
   * is exported. */
  %rename("%s", %$isenumitem) "";

  /* Don't export anything marked deprecated. */
  %ignore __declspec(deprecated);
%enddef

/*
 * Handle object lifetime in Fbx by adding indirection.
 *
 * Important: we need to declare all the weak-pointer classes here *before*
 * we %include them later. Otherwise e.g. FbxObject::GetScene won't wrap
 * up its scene. We do that by including weakpointerhandles.i
 *
 * Chicken-and-egg problem: weakpointerhandles.i is generated automatically by
 * running swig on this .i file. When we run swig on this .i file, we define
 * SWIG_GENERATING_TYPEDEFS to avoid including a file that hasn't been generated yet.
 */
%include "FbxSharpObjectLifetime.i"
#ifndef SWIG_GENERATING_TYPEDEFS
  %include "weakpointerhandles.i"
#endif

/*
 * Do null-pointer checking when setting variables of struct/class type.
 * e.g. if we have a global
 *      struct time_t g_startTime;
 * setting it will raise an exception if we set it with a null pointer.
 */
%naturalvar;

/*
 * Do null-pointer checking when passing the 'this' pointer.
 */
%typemap(in, canthrow=1) SWIGTYPE *self
%{ if (!$input) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "'this' is null ($1_basetype)", "this");
    return $null;
  }
  $1 = ($1_ltype)$input; %}

%define %null_arg_check(PARAMS)
%typemap(check, canthrow=1) PARAMS %{
  if(!$1){
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "$1_basetype is null", "$1_name");
    return $null;
  }
%}
%enddef

/*
 * Add a new constructor to a template class, that calls the original constructor
 * with a default value.
 */
%define %add_default_template_constructor(THECLASS, DEFAULTVALUE)
%extend THECLASS {
  THECLASS(){
        return new THECLASS(DEFAULTVALUE);
    }
}
%enddef

/* In C++ chars are stored as a single byte, whereas in C#
 * chars are stored as 2 bytes (16-bit unicode). To ensure that the
 * char is correct in C#, we return it as a byte and then convert it back to
 * a char in C#.
 * In C# a byte is an 8-bit unsigned int.
 */
%typemap(imtype,
         outattributes="[return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U8)]")
         char "byte";
%typemap(csout, excode=SWIGEXCODE) char {
    byte ret = $imcall;$excode
    return System.Convert.ToChar(ret);
  }

/*
 * How to handle strings. Must be before the includes that actually include code.
 */
%include "fbxstring.i"

/*
 * How to handle certain types we've optimized so they can be blitted.
 */
%include "optimization.i"

/*
 * This code allows the module to load when it's a Unity Package Manager package.
 *
 * Hopefully Package Manager will eventually be able to handle [DllImport()]
 * correctly and not need a full path.
 *
 * See also the replace-dllimport.py script.
 */
%pragma(csharp) imclasscode=%{
  /// <summary>
  /// String to use in the DllImport below.
  ///
  /// This must be a constant, but it changes depending on the platform and deployment.
  ///
  /// When deploying as an asset (a .unitypackage), define
  /// COM_UNITY_FORMATS_FBX_AS_ASSET and compile the package.
  ///
  /// When deploying with Unity Package Manager, do not add defines: the
  /// default platform defines suffice.
  /// </summary>
#if COM_UNITY_FORMATS_FBX_AS_ASSET || UNITY_STANDALONE 
  const string DllImportName = "$dllimport";
#elif UNITY_EDITOR_OSX
  const string DllImportName = "Packages/com.autodesk.fbx/MacOS/$dllimport.bundle/Contents/MacOS/$dllimport";
#elif UNITY_EDITOR_LINUX
  const string DllImportName = "Packages/com.autodesk.fbx/Linux/$dllimport.so";
#elif UNITY_EDITOR_WIN
  const string DllImportName = "Packages/com.autodesk.fbx/Windows/$dllimport.dll";
#else
  #error "FbxSdk: C# bindings for this platform haven't been implemented yet, sorry."
  const string DllImportName = "$dllimport";
#endif

// Because of a non-static delegate (ProgressCallback) that is called from NativeFBX->Managed, 
// we are not supporting IL2CPP
#if ENABLE_IL2CPP
     #error Autodesk.Fbx is not supported on IL2CPP
#endif

%}

/*
 * Import a bunch of typedefs and macros, so that SWIG can parse FBX files.
 */
%import "fbxsdk.h"
%import "fbxsdk/fbxsdk_def.h"
%import "fbxsdk/fbxsdk_nsbegin.h"
%import "fbxsdk/fbxsdk_nsend.h"
%import "fbxsdk/fbxsdk_version.h"
%import "fbxsdk/core/arch/fbxarch.h"
%import "fbxsdk/core/arch/fbxnew.h"

/*
 * Don't parse certain undocumented, subject-to-change, or private bits of
 * header files.
 */
#define DOXYGEN_SHOULD_SKIP_THIS

/*
 * Include all the code that uses templates here. It's important to include
 * them *before* the %ignore "" directive -- even after we reveal all,
 * templates don't seem to work in swig-3.7.12.
 */
%include "fbxtemplates.i"

/***************************************************************************
 * Ignore everything, and force the devs to allow certain items back in one by
 * one.
 ***************************************************************************/
%reveal_all_end;

/* Core classes */
%include "fbxmath.i"
%include "fbxmanager.i"
%include "fbxaxissystem.i"
%include "fbxsystemunit.i"
%include "fbxdatatypes.i"
%include "fbxpropertytypes.i"
%include "fbxtime.i"
%include "fbxstatus.i"
%include "fbxquaternion.i"
%include "fbxprogress.i"

/* The emitter hierarchy. Must be in partial order (base class before derived class). */
%include "fbxemitter.i"
%include "fbxobject.i"
%include "fbxcollection.i"
%include "fbxdocumentinfo.i"
%include "fbxdocument.i"
%include "fbxscene.i"
%include "fbxiobase.i"
%include "fbxexporter.i"
%include "fbximporter.i"
%include "fbxiosettings.i"
%include "fbxnode.i"
%include "fbxnodeattribute.i"
%include "fbxnull.i"
%include "fbxlayercontainer.i"
%include "fbxgeometrybase.i"
%include "fbxgeometry.i"
%include "fbxmesh.i"
%include "fbxnurbscurve.i"
%include "fbxglobalsettings.i"
%include "fbximplementation.i"
%include "fbxsurfacematerial.i"
%include "fbxtexture.i"
%include "fbxbindingtable.i"
%include "fbxpose.i"
%include "fbxdeformer.i"
%include "fbxsubdeformer.i"
%include "fbxcluster.i"
%include "fbxskin.i"
%include "fbxskeleton.i"
%include "fbxiopluginregistry.i"
%include "fbxanimlayer.i"
%include "fbxanimstack.i"
%include "fbxanimcurvebase.i"
%include "fbxanimcurve.i"
%include "fbxanimcurvefilter.i"
%include "fbxanimcurvenode.i"
%include "fbxcamera.i"
%include "fbxconnectionpoint.i"
%include "fbxmarker.i"
%include "fbxlight.i"
%include "fbxblendshape.i"
%include "fbxblendshapechannel.i"
%include "fbxshape.i"
%include "fbxio.i"
%include "fbxtransforms.i"
%include "fbxconstraint.i"
