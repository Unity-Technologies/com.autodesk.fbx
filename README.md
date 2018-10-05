# FbxSharp Project : FBX SDK C# bindings

## Requirements

* [Unity](http://unity3d.com) 2018.1
* [cmake](https://cmake.org/download/) 3.8
* [swig](http://www.swig.org/download.html) 3.0.12
* [Python](https://www.python.org/downloads/) 2.7.x or 3.x
* [Doxygen](http://www.stack.nl/~dimitri/doxygen/download.html) 1.8.13+
* Windows: [Visual Studio Community 2015](https://www.visualstudio.com/downloads/)
* OSX: [Xcode](https://developer.apple.com/xcode/features/) 9.x with command-line tools installed
* Linux: not supported

Newer versions of each software likely also work.

### Windows

When installing Visual Studio, make sure to install C# sdk, C++ sdk, and Universal Windows App Development Tools (this can be done by doing a custom install or
relaunching the installer and selecting "Modify").

### OSX or Linux:

```
# get the source
git clone https://github.com/Unity-Technologies/FbxSharp.git
cd FbxSharp
./build.sh
```

### Windows:

```
# get the source
git clone https://github.com/Unity-Technologies/FbxSharp.git
cd FbxSharp
build.cmd
```

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

### Running Tests

Open TestProjects/FBXSdk in Unity and run using the TestRunner.

### API Documentation

After compiling, the documentation will be in
```
FbxSharp/build/docs/html/index.html
```
Or in zipped form in
```
FbxSharp/build/install/com.autodesk.fbx/Documentation~/docs.zip
```

In the Unity package, the documentation is packaged as the zip file.
