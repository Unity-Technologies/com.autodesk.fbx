#RELEASE NOTES

**Version**: 0.0.14a
Note: skipping some versions so that FbxSdk package version matches FbxExporter package version

Add bindings for FbxMesh export (i.e. FbxObject::GetScene)

**Version**: 0.0.10a

Added documentation of vector classes.

Added test to check that the FbxSdk DLL cannot be used without the Unity Editor (This is a legal requirement).

Improve build process so it is more robust.

**Version**: 0.0.9a

Set the Doxygen homepage to be README.txt instead of README.md

Rename namespace to Unity.FbxSdk

Rename FbxSharp.dll and fbxsdk_csharp libaries to UnityFbxSdk.dll and UnityFbxSdkNative respectively

Change documentation title to "Unity FBXSDK C# API Reference"

Package zip file containing Doxygen documentation

Update license in README to Autodesk license

**Version**: 0.0.8a

Updated LICENCSE.txt to include Autodesk license

Use .bundle on Mac instead of .so for shared libraries

Ship bindings as binaries without source

**Version**: 0.0.7a
Note: skipping version 0.0.6a so that FbxSdk package version matches FbxExporter package version

Add bindings for FbxIOFileHeaderInfo. 
  - Exposed mCreator and mFileVersion as read-only attributes.

Made it easier for performance tests to pass.

**Version**: 0.0.5a

Added Doxygen documentation generation for C# bindings.