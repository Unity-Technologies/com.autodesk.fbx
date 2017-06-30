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

## Installing from source

First, get the requirements above. Then:
```
git clone https://github.com/Unity-Technologies/FbxSharp.git
```
This project uses git submodules. After cloning you need to enter the FbxSharp directory and do:
```
git submodule update --init --recursive
```
If you are developing FbxSharp, you will need to re-issue that command whenever submodules are updated. See e.g.
https://gist.github.com/gitaarik/8735255


Copy-paste to begin developing for OSX or linux:
```
# get the source
git clone https://github.com/Unity-Technologies/FbxSharp.git
pushd FbxSharp
git submodule update --init --recursive
popd

# build the project
mkdir FbxSharpBuild
cd FbxSharpBuild
cmake ../FbxSharp
make
make install

# run the sample Unity code
export PROJECT_PATH=`pwd`/tests/UnityTests
if test `uname -s` = 'Darwin' ; then
  UNITY_EDITOR_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
else
  UNITY_EDITOR_PATH=/opt/Unity/Editor/Unity/Unity
fi

MONO_LOG_MASK=dll "${UNITY_EDITOR_PATH}" -projectpath ${PROJECT_PATH}
```

Copy-paste to begin developing for Windows:
```
# get the source
git clone https://github.com/Unity-Technologies/FbxSharp.git
pushd FbxSharp
git submodule update --init --recursive
popd

# build the project
mkdir FbxSharpBuild
cd FbxSharpBuild
cmake ../FbxSharp -G"Visual Studio 14 2015 Win64"
cmake --build . --target INSTALL --config Release

# run the sample Unity code
"C:/Program Files/Unity/Editor/Unity.exe" -projectpath %cd%/tests/UnityTests
```

To build a release version, give `cmake` the `-DCMAKE_BUILD_TYPE=Release` flag.

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

cmake $FBXSDK_CSHARP_PATH
make 
make install
```

### Building on Windows 
```
REM Win10
mkdir FbxSharpBuild
cd FbxSharpBuild

cmake %FBXSDK_CSHARP_PATH -G"Visual Studio 14 2015 Win64" -DCMAKE_BUILD_TYPE=Release
cmake --build . --target INSTALL --config Release
```

Note: For out of source builds the entire Unity project at test/UnityTests will be copied over.

### Packaging the Bindings

#### On Windows
```
REM Win10
mkdir FbxSharpBuild
cd FbxSharpBuild

# add -DCREATE_PACKAGE=ON to automatically create package after install
# add -DPACKAGE_VERSION=0.0.2 to change the package version to 0.0.2
# add -DUNITY_EDITOR_PATH="Path/to/Unity.exe" to set which version of Unity is used
cmake %FBXSDK_CSHARP_PATH -G"Visual Studio 14 2015 Win64" -DCMAKE_BUILD_TYPE=Release -DCREATE_PACKAGE=ON
cmake --build . --target INSTALL --config Release

# alternatively run unitypackage target
cmake --build . --target unitypackage
```

#### On OSX and Linux
```
export FBXSDK_CSHARP_PATH=~/Development/FbxSharp
mkdir FbxSharpBuild
cd FbxSharpBuild

# add -DCREATE_PACKAGE=ON to automatically create package after install
# add -DPACKAGE_VERSION=0.0.2 to change the package version to 0.0.2
# add -DUNITY_EDITOR_PATH="/path/to/Unity.app/Contents/MacOS/Unity" to set which version of Unity is used
export UNITY_EDITOR_PATH=/Applications/Unity\ 5.6.1f1/Unity.app/Contents/MacOS/Unity
cmake $FBXSDK_CSHARP_PATH -DCREATE_PACKAGE=ON -DPACKAGE_VERSION=0.0.2 -DUNITY_EDITOR_PATH="$UNITY_EDITOR_PATH"
make 
make install

# alternatively run unitypackage target
make unitypackage
```

### Running UnitTests

**Requires** [Unity 5.6+](https://store.unity.com/)

**OSX**
```
make test
```

**Ubuntu**

```
make test
```

**Windows**

```
${UNITY_PATH}/Unity.exe -projectpath ${BUILD_PATH}/tests/UnityTests
```

OR ```cmake --build . --target RUN_TESTS```
