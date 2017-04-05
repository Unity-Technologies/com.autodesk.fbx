# Porting Classes

0. compile only your class

fbxsdk.i
```
#define EXCLUDE_INTERFACE_FILES

# porting this file...
%include "fbxemitter.i"
```

1. compile with warning as errors

CMakeList.txt
```
# compile with warnings as errors
set(PROJECT_COMPILE_FLAGS -Werror)
```

2. compile against a local and modified version of FBXSDK

FindFBXSDK.cmake
```
  set(FBXSDK_INSTALL_PATH "~/Development/FbxSharp/spike")
```

3. exclude all methods not required for port

fbxemitter.i
```
%ignore AddListener;
```

4. check global namespace has no new entries

Globals.cs
```
public class Globals {
}
```
