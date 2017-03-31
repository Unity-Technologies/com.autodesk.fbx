%nodefaultctor FbxManager;                      // Disable the default constructor for class FbxManager.

%apply int & OUTPUT { int & pMajor };
%apply int & OUTPUT { int & pMinor };
%apply int & OUTPUT { int & pRevision };

%include "fbxsdk/core/fbxmanager.h"

