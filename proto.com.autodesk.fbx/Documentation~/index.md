# About Autodesk® FBX® SDK for Unity

The C# Autodesk® FBX® SDK package provides access to a subset of the Autodesk® FBX® SDK from Unity C# scripts.

The Autodesk® FBX® SDK is a C++ software development platform and API toolkit that is free and easy-to-use. It allows application and content vendors to transfer existing content into the FBX format with minimal effort.

> ***Note:*** The C# Autodesk® FBX® SDK exposes only a subset of the full API. That subset enables exporter tools, such as the [FBX Exporter](https://docs.unity3d.com/Packages/com.unity.formats.fbx@latest) package. Using the C# Autodesk® FBX® SDK package for importing is not recommended. See [Known issues](#issues) below for more information.

## Requirements

The Autodesk® FBX® SDK for Unity package is compatible with the following versions of the Unity Editor:

* 2018.2 and later

## Contents

The Autodesk® FBX® SDK for Unity package contains:

* C# bindings
* Compiled binaries for MacOS and Windows that include the FBX SDK

## Installation

The Autodesk® FBX® SDK is automatically installed as a dependency of the [FBX Exporter](https://docs.unity3d.com/Packages/com.unity.formats.fbx@latest) package. It is not discoverable from the Package Manager UI, but can be installed without installing the FBX Exporter by adding it to your package manifest [Package Manager documentation](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@latest/index.html#PackManManifestsProject).

## Known issues

### Importing

In this version, you cannot downcast SDK C# objects, which limits the use of the bindings for an importer. For example, if the FBX SDK declares in C++ that it will return an `FbxDeformer`, you could safely cast the deformer to a skin deformer on the C++ side if you happen to know it is an `FbxSkinDeformer`. However, on the C# side, this is not permitted.

### Invalid operations

While there are guards against some common errors, it is possible to crash Unity by writing C# code that directs the FBX SDK to perform invalid operations. For example, if you have an `FbxProperty` in C# and you delete the `FbxNode` that contains the property, using the `FbxProperty` may produce undefined behavior This may even include crashing the Unity Editor. Make sure to read the editor log if you have unexplained crashes when writing FBX SDK C# code.

### IL2CPP backend

The C# Autodesk® FBX® SDK package is not supported at Runtime if you build using the IL2CPP backend.

### Linux

Linux support is currently experimental and unsupported.

## API documentation

There is no API documentation in the preview package. See the <a href="http://help.autodesk.com/cloudhelp/2018/ENU/FBX-Developer-Help/cpp_ref/annotated.html">Autodesk® FBX® SDK API documentation</a>.

The bindings are in the `Autodesk.Fbx` namespace:

```
using Autodesk.Fbx;
using UnityEditor;
using UnityEngine;

public class HelloFbx {
  [MenuItem("Fbx/Hello")]
  public static void Hello() {
    using(var manager = FbxManager.Create()) {
      Debug.LogFormat("FBX SDK is version {0}", FbxManager.GetVersion());
    }
  }
}
```
