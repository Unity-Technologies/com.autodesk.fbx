// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************
#ifdef IGNORE_ALL_INCLUDE_SOME
// Unignore class
%rename("%s") FbxEmitter;
%rename("%s") FbxEmitter::Destroy;
%nodefaultctor FbxEmitter;

#endif

/*
 * While FbxEmitter doesn't have a Destroy function, everything else whose
 * memory we manage does. So invent one. This way we can call Destroy on
 * anything we manage. Because it's listed as virtual here, the C# side
 * uses dynamic dispatch.
 */
%extend FbxEmitter {
  virtual void Destroy(bool recursive = false) { }
}

/*
 * Add a GetHashCode() and Equals() function to allow
 * us to perform identity tests in C# for FbxObjects and all
 * other derived classes.
 * Use the swigCPtr to check for equality.
 */
%typemap(cscode) FbxEmitter %{ 
  public override int GetHashCode(){
      return swigCPtr.Handle.GetHashCode();
  }

  public override bool Equals(object obj){
      if (obj == null || GetType() != obj.GetType()) 
          return false;

      FbxEmitter fe = (FbxEmitter)obj;
      return this.swigCPtr.Handle.Equals (fe.swigCPtr.Handle);
  }
%}

%include "fbxsdk/core/fbxemitter.h"

