# Changes in FBX SDK C# Bindings

## [2.0.1-preview] - 2019-06-12

NEW FEATURES
* Added IL2CPP build support for bindings

KNOWN ISSUES
* Using the FbxExporter::SetProgressCallback function with IL2CPP will give Runtime errors

## [2.0.0-preview.7] - 2018-02-08
CHANGES
* Fixed compiler warnings in tests

## [2.0.0-preview.6] - 2018-02-01

CHANGES
* Updated asmdefs to only include Editor platform

## [2.0.0-preview.5] - 2018-01-25

CHANGES
* Moved tests into separate package

## [2.0.0-preview.4] - 2018-12-04

CHANGES
* Updated changelog

## [2.0.0-preview.3] - 2018-12-03

CHANGES
* Updated documentation

## [2.0.0-preview.2] - 2018-11-13

CHANGES
* Removed version number from documentation (already available in changelog)
* Added missing .meta files
* Corrected asmdef name and platform settings
* Corrected plugin .meta file platform settings
* Experimental Linux support

## [2.0.0-preview.1] - 2018-10-25

CHANGES
* Updated documentation to conform to package validation requirements

## [2.0.0-preview] - 2018-06-22

NEW FEATURES
* The C# Bindings package has been renamed to com.autodesk.fbx
* The Autodesk.Fbx assembly can now be used in standalone builds (runtime)
* Added support for physical camera attributes
* Added support for constraints: FbxConstraint, FbxConstraintParent, FbxConstraintAim, and related methods
* Updated to FBX SDK 2018.1

KNOWN ISSUES
* The FBX SDK C# Bindings package is not supported if you build using the IL2CPP backend.

## [1.3.0f1] - 2018-04-17

NEW FEATURES
* Added bindings for FbxAnimCurveFilterUnroll
* Added binding for FbxGlobalSettings SetTimeMode to set frame rate
* Exposed bindings to set FbxNode's transformation inherit type
* Added binding for FbxCamera's FieldOfView property
* Added FbxObject::GetScene
* Added bindings for FbxIOFileHeaderInfo. 
* Exposed mCreator and mFileVersion as read-only attributes.

FIXES
* Fix Universal Windows Platform build error caused by UnityFbxSdk.dll being set as compatible with any platform instead of editor only.
* Enforced FbxSdk DLL only works with Unity 2017.1+
