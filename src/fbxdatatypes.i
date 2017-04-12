#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxDataType;

%rename("%s") FbxDataType::Create(const char* pName, const EFbxType pType);
%rename("%s") FbxDataType::Destroy;
%rename("%s") EFbxType;
#endif

%include "fbxsdk/core/fbxdatatypes.h"
%include "fbxsdk_csharp-fixed-headers/fbxpropertytypes.h"
