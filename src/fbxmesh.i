#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxMesh;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
%rename("%s") FbxMesh::Create;

#endif

%include "fbxsdk/scene/geometry/fbxmesh.h"
