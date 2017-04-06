%ignore __declspec(deprecated);

%ignore ""; // ignore everything

// don't ignore these
%rename("%s") FbxIOSettings;
%rename("%s") FbxIOSettings::Create(FbxManager *pManager, const char *pName);

%include "fbxsdk/fileio/fbxiosettings.h"
