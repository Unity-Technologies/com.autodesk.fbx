%ignore __declspec(deprecated);

//%rename($ignore, %isvariable) "";
//%rename($ignore, %ismember) "";

%ignore ""; // ignore everything

// don't ignore these
%rename("%s") FbxIOSettings;
%rename("%s") FbxIOSettings::Create(FbxManager *pManager, const char *pName);

%include "fbxsdk/fileio/fbxiosettings.h"
