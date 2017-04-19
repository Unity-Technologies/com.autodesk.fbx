#ifdef IGNORE_ALL_INCLUDE_SOME
%rename("%s") FbxLayerContainer;

// As the ignore everything will include the constructor, destructor, methods etc
// in the class, these have to be explicitly unignored too:
#endif

%include "fbxsdk/scene/geometry/fbxlayercontainer.h"
