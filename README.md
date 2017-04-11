# FbxSharp Project : FBX SDK C# bindings

## Requirements

* [FBX SDK](http://www.autodesk.com/products/fbx/overview) 2016.0 or 2017.1
* [cmake](https://cmake.org/download/) 3.7.2
* [swig](http://www.swig.org/download.html) 3.0.12
* [NUNIT](http://www.nunit.org/) 3.x
* [Python](https://www.python.org/downloads/) 2.7.x or 3.x
* Windows: [Visual Studio Community 2015](https://www.visualstudio.com/downloads/)
* Ubuntu: ???
* OSX: [Xcode](https://developer.apple.com/xcode/features/) 7.3 with command-line tools installed

Newer versions of each software likely also work, except for the FBX SDK. To support newer (or older) versions of FBX SDK, you need to edit the FindFBXSDK.cmake file.

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

### Building on OSX and Linux
```
# Define where the FBX SDK source is, and where you want to do your build.
export FBXSDK_CSHARP_PATH=~/Development/FbxSharp
mkdir FbxSharpBuild
cd FbxSharpBuild

# TODO: don't install in the source path!
cmake $FBXSDK_CSHARP_PATH -DCMAKE_INSTALL_PREFIX:PATH=$FBXSDK_CSHARP_PATH/tests/UnityTests/Assets/Plugins
make 
make install
```

### Building on Windows 
```
REM Win10
cd %FBXSDK_CSHARP_PATH

cmake -H. -Bbuild -G"Visual Studio 14 2015 Win64" -DCMAKE_INSTALL_PREFIX:PATH=%FBXSDK_CSHARP_PATH/tests/UnitTests/Assets/Plugins
cd build
cmake --build . --target INSTALL --config Release
```

### Running UnitTests

**Requires** [Unity 5.6+](https://store.unity.com/)

**OSX**
```
export FBXSDK_CSHARP_PATH=~/Development/FbxSharp
export UNITY3D_PATH=/Applications/Unity
export FBXSDK_PATH=/Applications/Autodesk/FBX\ SDK/2017.1

# finding fbxsdk_csharp linked against fbxsdk dynamic library
DYLD_LIBRARY_PATH=$DYLD_LIBRARY_PATH:${FBXSDK_CSHARP_PATH}/tests/UnityTests/Assets/Plugins/fbxsdk:${FBXSDK_PATH}/lib/clang/release MONO_LOG_MASK=dll ${UNITY3D_PATH}/Unity.app/Contents/MacOS/Unity -projectpath ${FBXSDK_SOURCE_PATH}/tests/UnityTests

# finding fbxsdk_csharp linked against fbxsdk static library
LD_LIBRARY_PATH=$LD_LIBRARY_PATH:${FBXSDK_CSHARP_PATH}/tests/UnityTests/Assets/Plugins/fbxsdk MONO_LOG_MASK=dll ${UNITY3D_PATH}/Unity.app/Contents/MacOS/Unity -projectpath ${FBXSDK_CSHARP_PATH}/tests/UnityTests
```

**Ubuntu**

```
export FBXSDK_CSHARP_PATH=~/Development/FbxSharp
export UNITY3D_PATH=/opt/Unity/Editor/Unity

LD_LIBRARY_PATH=$LD_LIBRARY_PATH:${FBXSDK_CSHARP_PATH}/tests/UnityTests/Assets/Plugins/fbxsdk MONO_LOG_MASK=dll ${UNITY3D_PATH}/Unity.app/Contents/MacOS/Unity -projectpath ${FBXSDK_CSHARP_PATH}/tests/UnityTests
```

