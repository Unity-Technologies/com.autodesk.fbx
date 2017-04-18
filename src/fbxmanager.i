// ***********************************************************************
// Copyright (c) 2017 Unity Technologies. All rights reserved.
//
// Licensed under the ##LICENSENAME##.
// See LICENSE.md file in the project root for full license information.
// ***********************************************************************

#ifdef IGNORE_ALL_INCLUDE_SOME

// Unignore class
%rename("%s") FbxManager;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxManager::Create; 
%rename("%s") FbxManager::Destroy; 
%rename("%s") FbxManager::FindClass;
%rename("%s") FbxManager::SetIOSettings;
%rename("%s") FbxManager::GetIOSettings;
%rename("%s") FbxManager::GetVersion;
%rename("%s") FbxManager::GetFileFormatVersion;

#endif

%nodefaultctor FbxManager;                      // Disable the default constructor for class FbxManager.

%apply int & OUTPUT { int & pMajor };
%apply int & OUTPUT { int & pMinor };
%apply int & OUTPUT { int & pRevision };

/*
 * Add a GetHashCode() and Equals() function to allow
 * us to perform identity tests in C#.
 * Use the swigCPtr to check for equality.
 */
%typemap(cscode) FbxManager %{ 
  public override int GetHashCode(){
      return swigCPtr.Handle.GetHashCode();
  }

  public override bool Equals(object obj){
      if (obj == null || GetType() != obj.GetType()) 
          return false;

      FbxManager fm = (FbxManager)obj;
      return this.swigCPtr.Handle.Equals (fm.swigCPtr.Handle);
  }
%}

%include "fbxsdk/core/fbxmanager.h"

