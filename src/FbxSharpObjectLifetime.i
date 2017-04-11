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
    return (obj == null) ? new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero) : obj.swigCPtr;
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
      lock(this) {
        $modulePINVOKE.ReleaseWeakPointerHandle(swigCPtr);
        swigCPtr = new global::System.Runtime.InteropServices.HandleRef(null, global::System.IntPtr.Zero);
      }
    }
  }%}

/* We handled the finalizer already, clobber the default handling. */
%typemap(csfinalize) THETYPE %{ %}

/*
 * Derived classes just call the superclass.
 * Probably there's a way to not even emit this call, though it's nice for debugging.
 */
%typemap(csdestruct_derived, methodname="Dispose", methodmodifiers="public") THETYPE %{{
    base.Dispose();
  }%}

%enddef
