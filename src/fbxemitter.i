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

%include "fbxsdk/core/fbxemitter.h"

