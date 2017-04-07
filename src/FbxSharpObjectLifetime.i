%{
/* Inline the weak-pointer support. */
#include "WeakPointerHandle.cxx"

/* Allow CSharp to release references. */
extern "C" SWIGEXPORT void SWIGSTDCALL CSharp_$module_Release_WeakPointerHandle(void *handle) {
    if (!handle) { return; }
    static_cast<WeakPointerHandle*>(handle)->ReleaseReference();
}

/* Set up the FBX allocators. */
#include <fbxsdk.h>
extern "C" SWIGEXPORT int SWIGSTDCALL CSharp_$module_InitFbxAllocators() {
  fbxsdk::FbxSetMallocHandler(WeakPointerHandle::Allocators::AllocateMemory);
  fbxsdk::FbxSetFreeHandler(WeakPointerHandle::Allocators::FreeMemory);
  fbxsdk::FbxSetCallocHandler(WeakPointerHandle::Allocators::AllocateZero);
  fbxsdk::FbxSetReallocHandler(WeakPointerHandle::Allocators::Reallocate);
  return 1;
}

%}

/* Add code to the PINVOKE file to handle the weak refs and to register the FBX
 * allocators. */
%pragma(csharp) imclasscode=%{
  // Set up the FBX allocators at static init time.
  [global::System.Runtime.InteropServices.DllImport("$dllimport", EntryPoint="CSharp_$module_InitFbxAllocators")]
  private static extern int InitFbxAllocators();
  private static int initFbx = InitFbxAllocators();

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

/* When returning a pointer or a reference, wrap it up in a handle */
%typemap(out) THETYPE * %{
  $result = WeakPointerHandle::GetHandle($1);
%}
%typemap(out) THETYPE & %{
  $result = WeakPointerHandle::GetHandle($1);
%}

/* When using a pointer, dereference the handle */
%typemap(in, canthrow=1) THETYPE * %{
  if (!WeakPointerHandle::DerefHandle($input, &$1)) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "Attempt to use destroyed $1_basetype $1_name", 0);
    return $null;
  }
%}

/* When using a reference, dereference the handle and also make sure it isn't null */
%typemap(in, canthrow=1) THETYPE & %{
  if (!WeakPointerHandle::DerefHandle($input, &$1)) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "Attempt to use destroyed $1_basetype $1_name", 0);
    return $null;
  }
  if (!$1) {
    SWIG_CSharpSetPendingExceptionArgument(SWIG_CSharpArgumentNullException, "$1_basetype $1_name is null", 0);
    return $null;
  }
%}

/*
 * Redefine the body; principally, remove the code to handle multiple inheritance
 * and object ownership.  There's two reasons:
 * (1) It's broken because of the WeakPointerHandle. We could fix that though.
 * (2) Faster performance / less memory usage -- we avoid having a handleref
 *     per subclass, and we avoid the ownership flag.
 * Obviously if we end up with multiple inheritance in Fbx, we'll have to revisit this.
 */
%typemap(csbody) THETYPE %{
  protected global::System.Runtime.InteropServices.HandleRef swigCPtr { get ; private set; }

  internal $csclassname(global::System.IntPtr cPtr, bool ignored) {
    swigCPtr = new global::System.Runtime.InteropServices.HandleRef(this, cPtr);
  }
  internal static global::System.Runtime.InteropServices.HandleRef getCPtr($csclassname obj) {
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
  }
%}

%typemap(csbody_derived) THETYPE %{
  internal $csclassname(global::System.IntPtr cPtr, bool ignored) : base(cPtr, ignored) { }
%}
%typemap(csdestruct_derived, methodname="Dispose", methodmodifiers="public") THETYPE {
    global::UnityEngine.Debug.Log("disposing $csclassname " + swigCPtr.Handle);
    base.Dispose();
  }

%enddef

/* Use:
 *    weakpointerhandlebase(FbxEmitter)
 * to define that FbxEmitter should be handled using the weakref mechanism, and
 * that it's the base class of the hierarchy.
 */
%define weakpointerhandlebase(THETYPE)

/* We need a destructor to exist so that SWIG emits a finalizer.
 * In the FbxEmitter hierarchy, there are no exposed destructors, so we create
 * one. */
%extend THETYPE { ~THETYPE() { } }

%typemap(csdestruct, methodname="Dispose", methodmodifiers="public") THETYPE {
    lock(this) {
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        global::UnityEngine.Debug.Log("releasing handle " + swigCPtr.Handle + " for $csclassname in Dispose()");
        $modulePINVOKE.ReleaseWeakPointerHandle(swigCPtr);
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
      global::UnityEngine.Debug.Log("disposed base class $csclassname");
    }
  }

%typemap(csfinalize) THETYPE %{
  ~$csclassname() {
    lock(this) {
      global::UnityEngine.Debug.Log("finalizing base class $csclassname @ " + swigCPtr.Handle);
      if (swigCPtr.Handle != global::System.IntPtr.Zero) {
        global::UnityEngine.Debug.Log("releasing handle " + swigCPtr.Handle + " for $csclassname in finalizer");
        $modulePINVOKE.ReleaseWeakPointerHandle(swigCPtr);
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }
%}

/* Otherwise it's the same as the derived classes. */
weakpointerhandle(THETYPE)


%enddef
