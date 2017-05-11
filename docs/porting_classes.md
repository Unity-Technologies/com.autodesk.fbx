# Porting Classes

1. compile only with your class

fbxsdk.i
```
#define EXCLUDE_INTERFACE_FILES

# porting this file...
%include "fbxemitter.i"
```

2. compile against a local version of FBXSDK so that, you can experiment with fixes

FindFBXSDK.cmake
```
  set(FBXSDK_INSTALL_PATH "~/Development/FbxSharp/spike")
```

3. (IGNORE_ALL_INCLUDE_SOME) unignore class and include only the methods required for porting

fbxemitter.i
```
%rename("%s") FbxIOBase;

// explicitly unignored the following methods:
%rename("%s") FbxIOBase::Initialize(const char *pFileName, int pFileFormat=-1, FbxIOSettings* pIOSettings=NULL);
```

NOTE: it would be great if we could unignore a specific method using a regex instead of have only two options:
* (a)  all ```%rename("%s") FbxIOBase::Initialize```
* (b)  one ```%rename("%s") FbxIOBase::Initialize(const char *pFileName, int pFileFormat=-1, FbxIOSettings* pIOSettings=NULL);```

4. String handling: 

* (a) given the choice between methods that use const char* and FbxString, use the const char * one. 
* (b) There's no support at the moment for 'out' or 'ref' strings: a function that takes in a char* or a FbxString& or FbxString* requires extra scrutiny (taking in a const version of those is fine).

5. check global namespace has no new entries

Globals.cs
```
public class Globals {
}
```

NOTE:
* we want to be able to move a global declaration or macro and move it to a specific namespace e.g. ```IOSROOT```

6. REF and OUTPUT arguments

Apply typemap per argument e.g.
```%apply int & OUTPUT { int & pMajor };```

7. Ignore/Unignore only a class/methods/enums/etc. with name

e.g.
```
%rename("%s", %$isclass) ClassName; // unignore all classes with name ClassName

// ignore everything in a class except the class itself and the 
// enum items (these will only show up in C# if we unignore the enum itself)
%rename("$ignore", "not" %$isenumitem, regextarget=1, fullname=1) "ClassName::.*";
```

8. Add argument check so function doesn't cause crash if passed a null argument
```
// make sure function doesn't crash if we pass it a null string as parameter
%typemap(check, canthrow=1) const char* parameter %{
  if(!$1){
    SWIG_CSharpSetPendingException(SWIG_CSharpNullReferenceException, "$1_basetype $1_name is null");
    return $null;
  }
%}
```

9. Add a function to a C++ class

```
%extend ClassName {
    void newFunction(){}
}
```

10. How to replace a function with custom function (that then calls the original function)
```
/*
 * SetCode takes a format string and a vararg. That can crash.  Make C# pass in
 * just a string (which you can already format in C# easily)
 */
%ignore FbxStatus::SetCode(const EStatusCode rhs, const char* pErrorMsg, ...);
%rename("SetCode") FbxStatus::SetCodeNoFormat;
%extend FbxStatus {
  void SetCodeNoFormat(const EStatusCode rhs, const char* pErrorMsg) {
    $self->SetCode(rhs, "%s", pErrorMsg);
  }
}
```

*Reference for swig directives (swig.swg): 
```
# OSX
/opt/local/share/swig/swig.swg
# Linux
/usr/local/share/swig/swig.swg
# Windows
/path/to/swig/directory/Lib/swig.swg
```
