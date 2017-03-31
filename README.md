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
export FBXSDK_SOURCE_PATH=~/Development/FbxSharp

# ${FBXSDK_SOURCE_PATH}/src  SWIG files found here
cd $FBXSDK_SOURCE_PATH

cmake -H. -Bbuild -DCMAKE_INSTALL_PREFIX:PATH=$FBXSDK_SOURCE_PATH/tests/UnitTests/Plugins
cd build
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
export UNITY3D_PATH=/Applications/Unity\ 4.6.9f1

LD_LIBRARY_PATH=$LD_LIBRARY_PATH:${FBXSDK_SOURCE_PATH}/tests/UnityTests/Assets/Plugins/fbxsdk MONO_LOG_MASK=dll ${UNITY3D_PATH}/Unity.app/Contents/MacOS/Unity -projectpath ${FBXSDK_SOURCE_PATH}/tests/UnityTests
```

**NOTES:**

Even though we install the library in the plugins folder on OSX and likely Ubuntu you will need to move the shared library to root of the Unity project or you will need to set the LD_LIBRARY_PATH.

```
LD_LIBRARY_PATH=$LD_LIBRARY_PATH:~/Development/FbxSharp/projects/HelloWorldSWIG/Assets/Editor MONO_LOG_MASK=dll /Applications/Unity\ 4.6.9f1/Unity.app/Contents/MacOS/Unity
```

