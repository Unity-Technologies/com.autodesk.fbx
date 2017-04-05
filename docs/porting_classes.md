# Porting Classes

1. compile only with your class

fbxsdk.i
```
#define EXCLUDE_INTERFACE_FILES

# porting this file...
%include "fbxemitter.i"
```

2. compile with warning as errors

CMakeList.txt
```
# compile with warnings as errors
set(PROJECT_COMPILE_FLAGS -Werror -Wno-error=null-dereference)
```

3. compile against a local version of FBXSDK so that, you can experiment with fixes

FindFBXSDK.cmake
```
  set(FBXSDK_INSTALL_PATH "~/Development/FbxSharp/spike")
```

4. exclude all methods not required for port from your class

fbxemitter.i
```
%ignore AddListener;
```

5. check global namespace has no new entries

Globals.cs
```
public class Globals {
}
```
