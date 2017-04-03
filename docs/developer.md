# Developer Documentation

* The assembly name is ```FbxSdk```
* The shared library containing the C# wrappers is ```fbxsdk_csharp.dll```
* ```FbxSdk.Globals``` contains global functions and constants

* ```src/fbxsdk.i``` is the master interface file
* ```src/fbx{classname}.i``` for files we're exposing

* We don't create a .i ( no %import) if we're not going to write tests / write performance / write documentation

* Unknown types are left as SWIGTYPE_p_{type}

# Setting up an Ubuntu VM

1. Download [VirtualBox](https://www.virtualbox.org/wiki/Downloads)
2. Download [Ubuntu .iso](https://www.ubuntu.com/download/desktop)
3. Follow this [tutorial](http://www.psychocats.net/ubuntu/virtualbox) to setup Ubuntu x64 machine
    * Note: it says it's for Windows, but it should work on Mac also
    * Note: In order to be able to create a 64-bit Ubuntu, may have to set "Enable Virtualization Technology" to true in the BIOS
    * Note: Should allocate at least 20GB of virtual hard drive
4. [Download Unity 5.5.2 .deb](https://forum.unity3d.com/threads/unity-on-linux-release-notes-and-known-issues.350256/)
    * In order to be able to open a file with MonoDevelop, may need to open Unity and in Edit->Preferences->External Tools set the external script editor to be /opt/Unity/MonoDevelop/bin/monodevelop
5. Install by running `sudo dpkg -i /path/to/deb/file` followed by `sudo apt-get install -f`
6. [Download FBX SDK](http://usa.autodesk.com/adsk/servlet/pc/item?siteID=123112&id=26012646)
7. Install FBX SDK by extracting the contents of the archive, then reading install_FbxSdk.txt which is inside the archive
    * For the location to install the FBX SDK, use: /opt/Autodesk/FBX SDK/{version}/ (e.g. for Fbx sdk 2017.1: /opt/Autodesk/FBX SDK/2017.1/)
8. Copy the libfbxsdk.so file from the FBX SDK to /opt/lib (this will be useful when running PerformanceBenchmarks.exe)
9. Clone the Github repository
10. Follow the steps in the root readme to generate and test the bindings
