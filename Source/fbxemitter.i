// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

%rename("%s", %$isclass) FbxEmitter;

/*
 * While FbxEmitter doesn't have a Destroy function, everything else whose
 * memory we manage does. So invent one. This way we can call Destroy in
 * the Dispose().
 *
 * FbxObject overrides this.
 * If we reveal the other FbxEmitter subclass, we'll have to create a Destroy
 * for it.
 */
%extend FbxEmitter { %proxycode %{
  public abstract void Destroy();
  public abstract void Destroy(bool recursive);
%} }

%typemap(csclassmodifiers) FbxEmitter "public abstract class";

%include "fbxsdk/core/fbxemitter.h"
