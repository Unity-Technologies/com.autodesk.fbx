# FbxSharp Project : FBX SDK C# bindings

## Requirements

* [FBX SDK](http://www.autodesk.com/products/fbx/overview) 2016.0 or 2017.1
* [cmake](https://cmake.org/download/) 3.7.2
* [swig](http://www.swig.org/download.html) 3.0.12
* [Python](https://www.python.org/downloads/) 2.7.x or 3.x
* Windows: [Visual Studio Community 2015](https://www.visualstudio.com/downloads/)
* Ubuntu: ???
* OSX: [Xcode](https://developer.apple.com/xcode/features/) 7.3 with command-line tools installed
* Optional: Unity 5.6.

Newer versions of each software likely also work, except for the FBX SDK. To support newer (or older) versions of FBX SDK, you need to edit the FindFBXSDK.cmake file.

## tl;dr

Install all the software above, then copy-paste this code into a terminal or a cmd prompt:

### OSX or linux:
```
# get the source
git clone https://github.com/Unity-Technologies/FbxSharp.git
pushd FbxSharp
git submodule update --init --recursive
popd

# build the project
mkdir FbxSharpBuild
cd FbxSharpBuild
cmake ../FbxSharp -DCMAKE_BUILD_TYPE=Release
make
make install
make unitypackage

# run the sample Unity code
export PROJECT_PATH=`pwd`/tests/UnityTests
if test `uname -s` = 'Darwin' ; then
  UNITY_EDITOR_PATH=/Applications/Unity/Unity.app/Contents/MacOS/Unity
else
  UNITY_EDITOR_PATH=/opt/Unity/Editor/Unity/Unity
fi

MONO_LOG_MASK=dll "${UNITY_EDITOR_PATH}" -projectpath ${PROJECT_PATH}
```

### Windows:

```
# get the source
git clone https://github.com/Unity-Technologies/FbxSharp.git
pushd FbxSharp
git submodule update --init --recursive
popd

# build the project
mkdir FbxSharpBuild
cd FbxSharpBuild
cmake ../FbxSharp -G"Visual Studio 14 2015 Win64" -DCMAKE_BUILD_TYPE=Release
cmake --build . --target INSTALL --config Release
cmake --build . --target unitypackage

# run the sample Unity code
"C:/Program Files/Unity/Editor/Unity.exe" -projectpath %cd%/tests/UnityTests
```

## Information for developers

See the "tl;dr" instructions above for instructions you can copy-paste.

You can add some options to the cmake line:
* To build a debug version, omit the `-DCMAKE_BUILD_TYPE=Release` flag.
* To specify the path to Unity (e.g. to use a version other than the default), use the `-DUNITY_EDITOR_PATH="Path/to/Unity.exe"` flag.
* * On OSX, this needs to specify the executable, not the app bundle: `-DUNITY_EDITOR_PATH="/path/to/Unity.app/Contents/MacOS/Unity"`
* To save time and avoid building the package, omit the line that mentions 'unitypackage'

This project uses git submodules. In order to update the submodules to the latest, issue this command:
```
git submodule update --init --recursive
```
For more information about submodules see e.g. https://gist.github.com/gitaarik/8735255

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
