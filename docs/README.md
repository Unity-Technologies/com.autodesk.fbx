FBX SDK C# Bindings
===================

Copyright (c) 2017 Unity Technologies. All rights reserved.

Licensed under the ##LICENSENAME##.
See LICENSE.txt file in the project root for full license information.

**Version**:

This package contains only a subset of the FbxSdk.

How to Access Bindings in Code
-------------------------------
All the bindings are located under the FbxSdk namespace,
and are accessed almost the same way as in C++.
e.g. FbxManager::Create() in C++ becomes FbxSdk.FbxManager.Create() in C#


How to Access Global Variables and Functions
--------------------------------------------
All global variables and functions are in Globals.cs, in the Globals class under the FbxSdk namespace.
e.g. if we want to access the IOSROOT variable, we would do FbxSdk.Globals.IOSROOT


How to Run the Unit Tests
-------------------------
1. Open the Test Runner window at Window->Test Runner
2. Make sure the EditMode tab is selected
3. Two categories of tests should show up (UnitTests, UseCaseTests).
   Select the tests to run, and click Run Selected, or Run All to run all the tests.
