FBX SDK C# Bindings
===================

This package contains only a subset of the Autodesk® FBX® SDK, and is designed to work in Unity only.

How to Access Bindings in Code
-------------------------------
All the bindings are located under the Autodesk.Fbx namespace,
and are accessed almost the same way as in C++.
e.g. FbxManager::Create() in C++ becomes Autodesk.Fbx.FbxManager.Create() in C#


How to Access Global Variables and Functions
--------------------------------------------
All global variables and functions are in Globals.cs, in the Globals class under the Autodesk.Fbx namespace.
e.g. if we want to access the IOSROOT variable, we would do Autodesk.Fbx.Globals.IOSROOT
