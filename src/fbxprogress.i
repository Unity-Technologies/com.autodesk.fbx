// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

/*
 * Define a setup to handle the progress callback.
 * Use e.g.
 *      %setup_SetProgressCallback(FbxExporer)
 * To declare FbxExporter::SetProgressCallback.
 *
 * FbxExporter::SetProgressCallback takes a C++ callback and a void*.
 *
 * Supporting that in the face of GC and going into managed code is a bit tricky;
 * we use the SWIG directors feature to handle it.
 *
 * 1. We define a "global" (to the module) delegate
 *      public delegate bool FbxProgressCallback(float, string)
 * 2. We define a C++ class that we export as
 *      internal abstract class FbxSharpProgressCallback
 *    and which has a Progress() function to override.
 * 3. We define a C# subclass:
 *      internal class FbxSharpProgressCallback.Wrapper
 *    whose constructor takes a delegate, and whose Progress() calls
 *    the delegate.
 * 4. The FbxExporter/FbxImporter.SetProgressCallback function takes a
 *    delegate, wraps it up in the wrapper, and stores the wrapper with the
 *    proxy.
 *
 * The only reason to use directors is that this way we can rely on SWIG having
 * figured out the many gotchas with object lifetime and garbage collection. If
 * performance is an issue we can simplify this, taking inspiration from SWIG's
 * automatically-generated code.
 */

%pragma(csharp) modulecode=%{
  public delegate bool FbxProgressCallback(float percentage, string status);
%}

%feature("director") FbxSharpProgressCallback;
%typemap(csclassmodifiers) FbxSharpProgressCallback "internal abstract class";
%rename("%s", %$isclass) FbxSharpProgressCallback;
%rename("%s") FbxSharpProgressCallback::FbxSharpProgressCallback;
%rename("%s") FbxSharpProgressCallback::~FbxSharpProgressCallback;
%rename("%s") FbxSharpProgressCallback::Progress;

%extend FbxSharpProgressCallback { %proxycode %{
  internal class Wrapper : FbxSharpProgressCallback {
    $module.FbxProgressCallback m_callback;
    internal Wrapper ($module.FbxProgressCallback callback) {
      m_callback = callback;
    }
    public override bool Progress(float percentage, string status) {
      return m_callback(percentage, status);
    }
  }
%} }

%define %define_fbxprogress(THETYPE)
%ignore THETYPE::SetProgressCallback; /* we define it by hand below */
%rename("%s") THETYPE::SetFbxSharpProgressCallback;
%csmethodmodifiers THETYPE::SetFbxSharpProgressCallback "private";
%extend THETYPE {
  void SetFbxSharpProgressCallback(FbxSharpProgressCallback *callback) {
    $self->SetProgressCallback(FbxSharpProgressCallback::CallProgress, callback);
  }

  %proxycode %{
  FbxSharpProgressCallback m_progressCallback;

  public void SetProgressCallback($module.FbxProgressCallback callback) {
    if (m_progressCallback != null) { m_progressCallback.Dispose(); }
    m_progressCallback = (callback == null) ? null : new FbxSharpProgressCallback.Wrapper(callback);
    SetFbxSharpProgressCallback(m_progressCallback);
  } %}
}
%enddef

/* We put the FbxSharpProgressCallback into its own file because we need to
 * both #include and %include it. */
%{
#include "FbxSharpProgressCallback.h"
%}
%include "FbxSharpProgressCallback.h"
