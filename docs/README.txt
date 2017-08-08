FBX SDK C# Bindings
===================

Copyright (c) 2017 Unity Technologies. All rights reserved.

Licensed under the ##LICENSENAME##.
See LICENSE.txt file in the project root for full license information.

**Version**:

This package contains only a subset of the FbxSdk, and is designed to work in the Unity Editor only.

How to Access Bindings in Code
-------------------------------
All the bindings are located under the FbxSdk namespace,
and are accessed almost the same way as in C++.
e.g. FbxManager::Create() in C++ becomes FbxSdk.FbxManager.Create() in C#


How to Access Global Variables and Functions
--------------------------------------------
All global variables and functions are in Globals.cs, in the Globals class under the FbxSdk namespace.
e.g. if we want to access the IOSROOT variable, we would do FbxSdk.Globals.IOSROOT

   
How to Access Documentation for Bindings
----------------------------------------
1. Unzip docs.zip outside of the Assets folder
2. Open docs/html/index.html