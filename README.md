# FbxSharp Project : FBX SDK C# bindings

## Overview

**Writing C# code**
```
// Using FbxSdk Assembly

using FbxSdk;

// global functions found in FbxSdk.Globals
var a = FbxSdk.Globals.FbxGetDataTypeNameForIO(b);

var sdkManager = FbxManager.Create();

sdkManager.Destroy();
```

**What gets installed into your Unity Project**
```
{unity_project}/Assets/Plugins/
    fbxsdk/
        libfbxsdk_csharp.so
        csharp/
               FbxManager.cs
               ...
```               

## How to do out-of-source build

### Building on OSX 
```
# ${dev_dir}/FbxSharp/src  SWIG files found here
cd ${dev_dir}/FbxSharp

cmake -H. -Bbuild -DCMAKE_INSTALL_PREFIX:PATH=/path/to/Unity/project/Assets/Plugins CMakeLists.txt
cd build
make 
make install
```

### Building on Windows 
```
REM Win10
cd %{dev_dir}/FbxSharp

cmake -H. -Bbuild -G"Visual Studio 14 2015 Win64" -DCMAKE_INSTALL_PREFIX:PATH=/path/to/Unity/project/Assets/Plugins CMakeLists.txt
REM you might need to edit PATH so you can find devenv & vcvarsall
cd build
vcvarsall amd64
devenv fbxsdk_csharp.sln /Build "Release"
devenv fbxsdk_csharp.sln /Build "Release" /project INSTALL
```

**NOTES:**

Even though we install the library in the plugins folder on OSX and likely Ubuntu you will need to move the shared library to root of the Unity project or you will need to set the LD_LIBRARY_PATH.

```
LD_LIBRARY_PATH=$LD_LIBRARY_PATH:~/Development/FbxSharp/projects/HelloWorldSWIG/Assets/Editor MONO_LOG_MASK=dll /Applications/Unity\ 4.6.9f1/Unity.app/Contents/MacOS/Unity
```

