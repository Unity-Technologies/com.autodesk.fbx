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

4. (IGNORE_ALL_INCLUDE_SOME) unignore class and include only the methods required for porting

fbxemitter.i
```
%rename("%s") FbxIOBase;

// explicitly unignored the following methods:
%rename("%s") FbxIOBase::Initialize(const char *pFileName, int pFileFormat=-1, FbxIOSettings* pIOSettings=NULL);
```

NOTE: it would be great if we could unignore a specific method using a regex instead of have only two options:
* (a)  all ```%rename("%s") FbxIOBase::Initialize```
* (b)  one ```%rename("%s") FbxIOBase::Initialize(const char *pFileName, int pFileFormat=-1, FbxIOSettings* pIOSettings=NULL);```

5. String handling: 

* (a) given the choice between methods that use const char* and FbxString, use the const char * one. 
* (b) There's no support at the moment for 'out' or 'ref' strings: a function that takes in a char* or a FbxString& or FbxString* requires extra scrutiny (taking in a const version of those is fine).

6. check global namespace has no new entries

Globals.cs
```
public class Globals {
}
```

NOTE:
* we want to be able to move a global declaration or macro and move it to a specific namespace e.g. ```IOSROOT```

7. REF and OUTPUT arguments

Apply typemap per argument e.g.
```%apply int & OUTPUT { int & pMajor };```


