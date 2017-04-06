#ifdef IGNORE_ALL_INCLUDE_SOME

// don't ignore these
%rename("%s") FbxIOSettings;
%rename("%s") FbxIOSettings::Create(FbxManager *pManager, const char *pName);

#endif

%include "fbxsdk/fileio/fbxiosettings.h"
