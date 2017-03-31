# Developer Documentation

* The assembly name is ```FbxSdk```
* The shared library containing the C# wrappers is ```fbxsdk_csharp.dll```
* ```FbxSdk.Globals``` contains global functions and constants

* ```src/fbxsdk.i``` is the master interface file
* ```src/fbx{classname}.i``` for files we're exposing

* We don't create a .i ( no %import) if we're not going to write tests / write performance / write documentation

* Unknown types are left as SWIGTYPE_p_{type}
