// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/*
 * Object lifetime support for FbxEmitter and a few other classes
 * in Fbx.
 *
 * In C#, if we hold a reference to something that's destroyed, we get an
 * exception -- not a crash-to-desktop! This code implements the swig hackery
 * to make that work for FbxSharp.
 *
 * The key concepts:
 * (1) The C# proxy objects actually point to a reference-counted
 *      WeakPointerHandle.
 * (2) When Fbx calls 'free', it actually calls our free, which checks if
 *      there's a WeakPointerHandle. If so, it sets the WPH pointer to null.
 * (3) When the C# calls into Fbx, we dereference the WPH. If its pointer is
 *      null, we throw an exception mentioning the object was destroyed.
 * (4) We also use this check to throw NREs (e.g. using a proxy that was
 *      Dispose()d). If it's legit to pass in a null pointer for that
 *      parameter, you need to mark it explicitly as such, e.g.:
 *              %apply FbxManager *MAYBENULL { FbxManager * pManager };
 * (5) Since Fbx isn't using multiple inheritance, we disable the SWIG
 *      multiple-inheritance code. It caused a bug and is also a slight
 *      performance issue.
 * (6) When the C# proxies get garbage-collected or Dispose(), the underlying
 *      object doesn't get a Destroy() call. However, the handle is released.
 */

%{
/* Inline the weak-pointer support. */
#include "WeakPointerHandle.cxx"

/* Allow CSharp to release references. */
extern "C" SWIGEXPORT void SWIGSTDCALL CSharp_$module_Release_WeakPointerHandle(void *handle) {
    if (!handle) { return; }
    static_cast<WeakPointerHandle*>(handle)->ReleaseReference();
}

/* Set up the FBX allocators and support the static structures by creating a manager. */
#include <fbxsdk.h>
extern "C" SWIGEXPORT int SWIGSTDCALL CSharp_$module_InitFbxAllocators() {
  fbxsdk::FbxSetMallocHandler(WeakPointerHandle::Allocators::AllocateMemory);
  fbxsdk::FbxSetFreeHandler(WeakPointerHandle::Allocators::FreeMemory);
  fbxsdk::FbxSetCallocHandler(WeakPointerHandle::Allocators::AllocateZero);
  fbxsdk::FbxSetReallocHandler(WeakPointerHandle::Allocators::Reallocate);

  /* Create a manager, and never release it. It will own some static
   * structures, including the FbxDataTypes.
   *
   * Otherwise, if the user accesses the FbxDataTypes without having an
   * FbxManager around, there's a risk of memory corruption. */
  fbxsdk::FbxManager::Create();
  return 1;
}

%}

/* Add code to the PINVOKE file to handle the weak refs and to register the FBX
 * allocators. */
%pragma(csharp) imclasscode=%{
  // Set up the FBX allocators at static init time.
  [global::System.Runtime.InteropServices.DllImport("$dllimport", EntryPoint="CSharp_$module_InitFbxAllocators")]
  private static extern int _InitFbxAllocators();

  private static int InitFbxAllocators()
  {
      int result = -1;
      bool verbose = UnityEngine.Debug.unityLogger.logEnabled;
      result = _InitFbxAllocators();

      if (result!=1 && verbose)
      {
            UnityEngine.Debug.LogError("Failed to configure FbxSdk memory allocators.");
      }

      return result;
  }
    
  protected static int initFbx = InitFbxAllocators(); /* protected to quiet a warning */

  [global::System.Runtime.InteropServices.DllImport("$dllimport", EntryPoint="CSharp_$module_Release_WeakPointerHandle")]
  public static extern void ReleaseWeakPointerHandle(global::System.Runtime.InteropServices.HandleRef handle);
%}

/* Use:
 *    weakpointerhandle(FbxObject)
 * to define that FbxObject should be handled using the weakref mechanism:
 * we wrap up the class in a weakref when we return, we check the weakref when
 * we use it. For this to work you need to use the WeakPointerHandle allocators.
 */
%define weakpointerhandle(THETYPE)

/* Ignore the class Id. */
%ignore THETYPE::ClassId;

/* When returning an object, wrap it up in a handle */
%typemap(out) THETYPE *, THETYPE & %{
  $result = WeakPointerHandle::GetHandle($1);
%}

/* When using an object, dereference the handle and also make sure it isn't null */
%typemap(in, canthrow=1) THETYPE *, THETYPE & %{
  if (!WeakPointerHandle::DerefHandle($input, &$1)) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "Attempt to use destroyed $1_basetype $1_name", 0);
    return $null;
  }
  if (!$1) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "$1_basetype is null", "$1_name");
    return $null;
  }
%}

/* Special-case you can apply to pointers that are allowed to be null */
%typemap(in, canthrow=1) THETYPE * MAYBENULL %{
  if (!WeakPointerHandle::DerefHandle($input, &$1)) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "Attempt to use destroyed $1_basetype $1_name", 0);
    return $null;
  }
  /* It's ok for $1_basetype $1_name to be null */
%}

/*
 * Redefine the body. Principally, remove the code to handle multiple inheritance
 * and object ownership.  There's two reasons:
 * (1) It's broken because of the WeakPointerHandle. We could fix that though.
 * (2) We never have object ownership.
 * (3) Faster performance / less memory usage -- we avoid having a handleref
 *     per subclass, and we avoid the ownership flag.
 * Obviously if we end up with multiple inheritance in Fbx, we'll have to revisit this.
 *
 * The ownership flag remains in the constructor just because it's a pain to
 * make the callers stop passing it.
 */
%typemap(csbody) THETYPE %{
  protected global::System.Runtime.InteropServices.HandleRef swigCPtr { get ; private set; }

  internal $csclassname(global::System.IntPtr cPtr, bool ignored) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }
  internal static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return ((object)obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }
%}

%typemap(csbody_derived) THETYPE %{
  internal $csclassname(global::System.IntPtr cPtr, bool ignored) : base(cPtr, ignored) { }
%}


/*
 * Dispose / finalize.
 *
 * We need to release the reference on the handle.
 *
 * We also want to follow proper dispose norms, which by default SWIG doesn't
 * do. See https://msdn.microsoft.com/en-us/library/ms244737.aspx
 */
%typemap(csdestruct, methodname="Dispose", methodmodifiers="public") THETYPE %{{
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }
  ~$csclassname() {
    Dispose(false);
  }
  protected void Dispose(bool disposing) {
    if (swigCPtr.Handle != global::System.IntPtr.Zero) {
      if (disposing) {
        Destroy();
      }
      lock(this) {
        $imclassname.ReleaseWeakPointerHandle(swigCPtr);
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }%}

/* We handled the finalizer already, clobber the default handling. */
%typemap(csfinalize) THETYPE %{ %}

/*
 * SWIG requires there be a destructor, but we can say it's a // method instead
 * of a public method... which comments it out. (It would be nice if we could
 * just not emit this at all.)
 */
%typemap(csdestruct_derived, methodname="Dispose", methodmodifiers="//") THETYPE %{{base.Dispose();}%}

/*
 * Equality and letting everyone know it's equatable.
 */
%define_pointer_equality_functions(THETYPE);
%typemap(csinterfaces) THETYPE "System.IDisposable, System.IEquatable<THETYPE>";

%enddef
