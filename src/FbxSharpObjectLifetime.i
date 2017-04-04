%{
/* Inline the weak-pointer support. */
#include "WeakPointerHandle.cxx"

/* Allow CSharp to release references. */
extern "C" SWIGEXPORT void SWIGSTDCALL CSharp_$module_Release_WeakPointerHandle(void *handle) {
    if (!handle) { return; }
    ((WeakPointerHandle*)handle)->ReleaseReference();
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
  public static extern void ReleaseWeakPointerHandle(global::System.IntPtr handle);
%}

/* Use:
 *    weakpointerhandle(FbxObject)
 * to define that FbxObject should be handled using the weakref mechanism:
 * we wrap up the class in a weakref when we return, we check the weakref when
 * we use it. For this to work you need to use the WeakPointerHandle allocators.
 */
/* When returning a pointer or a reference, wrap it up in a handle */
%define weakpointerhandle(THETYPE)
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
%enddef
