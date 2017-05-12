# Porting Classes

1. [How do I compile with multiple .i files?](#how-do-i-compile-with-multiple-i-files)
2. [How do I compile against a local version of FBXSDK?](#how-do-i-compile-against-a-local-version-of-fbxsdk)
3. [How do I unignore a class and include only the methods required for porting?](#how-do-i-unignore-a-class-and-include-only-the-methods-required-for-porting)
4. [What do I do if I need a function that uses FbxString?](#what-do-i-do-if-i-need-a-function-that-uses-fbxstring)
5. [Where will global defines/functions/variables end up?](#where-will-global-definesfunctionsvariables-end-up)
6. [What do I do if I need to use a REF or OUTPUT argument?](#what-do-i-do-if-i-need-to-use-a-ref-or-output-argument)
7. [How do I ignore/unignore only classes/methods/enums/etc?](#how-do-i-ignoreunignore-only-classesmethodsenumsetc)
8. [How do I prevent a function from crashing if passed a null argument?](#how-do-i-prevent-a-function-from-crashing-if-passed-a-null-argument)
9. [How do I add a function to a C++ class?](#how-do-i-add-a-function-to-a-c-class)
10. [How do I replace a function with a custom function?](#how-do-i-replace-a-function-with-a-custom-function)
11. [Where can I find a complete list of swig directives?](#where-can-i-find-a-complete-list-of-swig-directives)
---
### How do I compile with multiple .i files?

To compile a module with multiple .i files, each contain one or more headers and swig directives, create a .i file that contains the module definition (in this case fbxsdk.i), then include any additional .i files as follows:

fbxsdk.i
```
#define EXCLUDE_INTERFACE_FILES

# porting this file...
%include "fbxemitter.i"
```
---
### How do I compile against a local version of FBXSDK?

FindFBXSDK.cmake
```
  set(FBXSDK_INSTALL_PATH "~/Development/FbxSharp/spike")
```
---
### How do I unignore a class and include only the methods required for porting?

If we are using the ignore all include some (IGNORE_ALL_INCLUDE_SOME) approach, and there is a global ignore, ignoring everything in all the files, then the way to unignore classes and desired methods is as follows:

fbxemitter.i
```
%rename("%s") FbxIOBase;

// explicitly unignored the following methods:
%rename("%s") FbxIOBase::Initialize(const char *pFileName, int pFileFormat=-1, FbxIOSettings* pIOSettings=NULL);
```

NOTE: it would be great if we could unignore a specific method using a regex instead of have only two options:
* (a)  all ```%rename("%s") FbxIOBase::Initialize```
* (b)  one ```%rename("%s") FbxIOBase::Initialize(const char *pFileName, int pFileFormat=-1, FbxIOSettings* pIOSettings=NULL);```
---

### What do I do if I need a function that uses FbxString?

* (a) given the choice between methods that use const char* and FbxString, use the const char * one. 
* (b) There's no support at the moment for 'out' or 'ref' strings: a function that takes in a char* or a FbxString& or FbxString* requires extra scrutiny (taking in a const version of those is fine).
---

### Where will global defines/functions/variables end up?

They will be put into Globals.cs and can be accessed using FbxSdk.Globals.VarName

Globals.cs
```
public class Globals {
}
```

NOTE:
* we want to be able to move a global declaration or macro and move it to a specific namespace e.g. ```IOSROOT```
---
### What do I do if I need to use a REF or OUTPUT argument?

Apply typemap per argument e.g.
```
%apply int & OUTPUT { int & pMajor }; // will show up as 'out' in C#
%apply int & INOUT { int& pMajor }; // will show up as 'ref' in C#
```
---
### How do I Ignore/Unignore only classes/methods/enums/etc?

e.g.
```
%rename("%s", %$isclass) ClassName; // unignore all classes with name ClassName

// ignore everything in a class except the class itself and the 
// enum items (these will only show up in C# if we unignore the enum itself)
%rename("$ignore", "not" %$isenumitem, regextarget=1, fullname=1) "ClassName::.*";
```
---
### How do I prevent a function from crashing if passed a null argument?

Add a check typemap to the problem variable(s) of the function:
```
// make sure function doesn't crash if we pass it a null string as parameter
%typemap(check, canthrow=1) const char* parameter %{
  if(!$1){
    SWIG_CSharpSetPendingException(SWIG_CSharpNullReferenceException, "$1_basetype $1_name is null");
    return $null;
  }
%}

// for checking multiple variabls of the same function
%typemap(check, canthrow=1) (const char* parameter1, const char* parameter2) %{
  if(!$1){
    SWIG_CSharpSetPendingException(SWIG_CSharpNullReferenceException, "$1_basetype $1_name is null");
    return $null;
  }
%}
```
---
### How do I add a function to a C++ class?

Using the extend directive as follows:

```
%extend ClassName {
    void newFunction(){}
}
```
---
### How do I replace a function with a custom function?

You can replace the function with another function that will have the same name and format (or a different name/format if desired).  The new function can then do some extra operations and call the original function.

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
---
### Where can I find a complete list of swig directives?

A file containing the swig directives (swig.swg) comes with each installation of swig and can be found here: 
```
# OSX
/opt/local/share/swig/swig.swg
# Linux
/usr/local/share/swig/swig.swg
# Windows
/path/to/swig/directory/Lib/swig.swg
```
