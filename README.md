# FbxSharp Project : FBX SDK C# bindings

## Requirements

* [cmake 3.7.2+](https://cmake.org/download/)
* [Visual Studio Community 2015+](https://www.visualstudio.com/downloads/) (for Windows)
* [swig 3.0.12+](http://www.swig.org/download.html)
* [FbxSdk 2016.1+](http://www.autodesk.com/products/fbx/overview)
* [Python 2.7.13+](https://www.python.org/downloads/)

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
export FBXSDK_SOURCE_PATH=~/Development/FbxSharp
mkdir FbxSharpBuild
cd FbxSharpBuild

# TODO: don't install in the source path!
cmake $FBXSDK_SOURCE_PATH -DCMAKE_INSTALL_PREFIX:PATH=$FBXSDK_SOURCE_PATH/tests/UnityTests/Assets/Plugins
make 
make install
```

### Building on Windows 
```
REM Win10
cd %{dev_dir}/FbxSharp

cmake -H. -Bbuild -G"Visual Studio 14 2015 Win64" -DCMAKE_INSTALL_PREFIX:PATH=/path/to/Unity/project/Assets/Plugins
cd build
cmake --build . --target INSTALL --config Release
```

### Running UnitTests

**OSX**
```
export FBXSDK_SOURCE_PATH=~/Development/FbxSharp
export UNITY3D_PATH=/Applications/Unity

LD_LIBRARY_PATH=$LD_LIBRARY_PATH:${FBXSDK_SOURCE_PATH}/tests/UnityTests/Assets/Plugins/fbxsdk MONO_LOG_MASK=dll ${UNITY3D_PATH}/Unity.app/Contents/MacOS/Unity -projectpath ${FBXSDK_SOURCE_PATH}/tests/UnityTests
```

**NOTES:**

Even though we install the library in the plugins folder on OSX and likely Ubuntu you will need to move the shared library to root of the Unity project or you will need to set the LD_LIBRARY_PATH.

```
LD_LIBRARY_PATH=$LD_LIBRARY_PATH:~/Development/FbxSharp/projects/HelloWorldSWIG/Assets/Editor MONO_LOG_MASK=dll /Applications/Unity\ 4.6.9f1/Unity.app/Contents/MacOS/Unity
```

