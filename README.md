# FbxSharp Project : FBX SDK C# bindings

## Requirements

* [Unity](http://unity3d.com) 2017.1 or later
* [FBX SDK](http://www.autodesk.com/products/fbx/overview) 2017.1
* [cmake](https://cmake.org/download/) 3.8
* [swig](http://www.swig.org/download.html) 3.0.12
* [Python](https://www.python.org/downloads/) 2.7.x or 3.x
* Windows: [Visual Studio Community 2015](https://www.visualstudio.com/downloads/)
* Linux: ???
* OSX: [Xcode](https://developer.apple.com/xcode/features/) 7.3 with command-line tools installed
* OSX: [mono](http://www.mono-project.com/) 3.12 (`sudo port install mono` or `sudo brew install mono`)

Newer versions of each software likely also work.

### Windows

Make sure to download the version of the FBX SDK corresponding to the version of Visual Studio being used. e.g. if using Visual Studio 2015, make sure to download
FBX SDK for VS2015.

When installing Visual Studio, make sure to install C# sdk, C++ sdk, and Universal Windows App Development Tools (this can be done by doing a custom install or
relaunching the installer and selecting "Modify").

In addition to installing the above software, the following items need to be added to the PATH environment variable:

```
C:\Program Files (x86)\MSBuild\14.0\Bin
C:\Windows\Microsoft.NET\Framework\v4.0.30319
D:\Program Files (x86)\Microsoft Visual Studio 14.0\VC
D:\Users\Viktoria\Downloads\swigwin-3.0.12\swigwin-3.0.12
D:\Program Files\Autodesk\FBX\FBX SDK\2017.1
D:\Python27
```

Set paths according to install locations.

## tl;dr

Install all the software above, then copy-paste this code into a terminal or a cmd prompt:

### OSX or Linux:
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
cmake ../FbxSharp -G"Visual Studio 14 2015 Win64"
cmake --build . --target INSTALL --config Release
cmake --build . --target unitypackage --config Release

# run the sample Unity code
"C:/Program Files/Unity/Editor/Unity.exe" -projectpath %cd%/tests/UnityTests
```

## Information for developers

See the "tl;dr" instructions above for instructions you can copy-paste.

You can add some options to the cmake line:
* To build a debug version, add the `-DCMAKE_BUILD_TYPE=Debug` flag.
* To specify the path to Unity (e.g. to use a version other than the default), use the `-DUNITY="Path/to/UnityFolder"` flag.
* By default the version number is specified in the CMakeLists.txt but can be overriden with `-DPACKAGE_VERSION=x.y.z` flag.
* By default the unity package drops in the FbxSharpBuild directory, but this can be overriden with `-DPACKAGE_PATH=/path/to/package` flag.
* To save time and avoid building the package, omit the line that mentions 'unitypackage'
* Conversely, to build the package in the `all` target, add the `-DCREATE_PACKAGE=on` flag.

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
{unity_project}/Assets/
    FbxSdk/
        Plugins/
            x64/
                UnityFbxSdk.dll
                MacOS/
                    UnityFbxSdkNative.bundle
                Windows/
                    UnityFbxSdkNative.dll
```

### Running UnitTests

**Requires** [Unity 5.6+](https://store.unity.com/)

**OSX**
```
make test
```

**Linux**

```
make test
```

**Windows**

```
${UNITY_PATH}/Unity.exe -projectpath ${BUILD_PATH}/tests/UnityTests
```

OR ```cmake --build . --target RUN_TESTS```

### Creating Documentation with Doxygen

**Requires** [Doxygen 1.8.13+](http://www.stack.nl/~dimitri/doxygen/download.html)

After compiling, the documentation will be in
```
FbxSharpBuild/docs/html/index.html
```
Or in zipped form in
```
FbxSharpBuild/docs.zip
```

In the unity package, the documentation is packaged as the zip file.
