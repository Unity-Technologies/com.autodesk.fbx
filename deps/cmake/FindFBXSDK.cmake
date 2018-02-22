# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************

# Platform-specific code.
# Autodesk installs the FBX SDK in a non-standard spot so we need to find it.
# Result of this code:
# - Add some platform defines to CMAKE_SWIG_FLAGS that we need for FBX SDK.
# - Set FBXSDK_INCLUDE_PATHS and FBXSDK_LIB_PATHS to where we should look for to find the SDK.
if(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
  list(APPEND CMAKE_SWIG_FLAGS "-D__APPLE__")
  list(APPEND CMAKE_SWIG_FLAGS "-D__x86_64__")
  list(APPEND CMAKE_SWIG_FLAGS "-DFBXSDK_COMPILER_GNU")

  set(_fbxsdk_INSTALL_PATH "/Applications/Autodesk/FBX SDK")
  list(APPEND _fbxsdk_lib_paths "lib/clang/release")

elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Windows")
  SET(CMAKE_FIND_LIBRARY_PREFIXES "lib")

  list(APPEND CMAKE_SWIG_FLAGS "-D_WIN64")
  list(APPEND CMAKE_SWIG_FLAGS "-D_M_X64")
  list(APPEND CMAKE_SWIG_FLAGS "-D_MSC_VER")

  set(_fbxsdk_INSTALL_PATH "C:/Program Files/Autodesk/FBX/FBX SDK")
  list(APPEND _fbxsdk_lib_paths "lib/vs2015/x64/release")
  list(APPEND _fbxsdk_lib_paths "lib/vs2010/x64/release")

elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Linux")
  list(APPEND CMAKE_SWIG_FLAGS "-D__linux__")
  list(APPEND CMAKE_SWIG_FLAGS "-D__x86_64__")
  list(APPEND CMAKE_SWIG_FLAGS "-D__GNUC__")

  set(_fbxsdk_INSTALL_PATH "/opt/Autodesk/FBX/FBX SDK")
  list(APPEND _fbxsdk_lib_paths "lib/gcc4/x64/release")
endif()

# Iterate over the versions. Pick the first one (reverse-alphabetically)
file(GLOB _fbxsdk_VERSIONS LIST_DIRECTORIES true "${_fbxsdk_INSTALL_PATH}/*")
list(SORT _fbxsdk_VERSIONS)
list(REVERSE _fbxsdk_VERSIONS)
foreach(_fbxsdk_PATH ${_fbxsdk_VERSIONS})
  list(APPEND FBXSDK_INCLUDE_PATHS "${_fbxsdk_PATH}/include")
  foreach(_fbxsdk_lib_path ${_fbxsdk_lib_paths})
    list(APPEND FBXSDK_LIB_PATHS "${_fbxsdk_PATH}/${_fbxsdk_lib_path}")
  endforeach()
endforeach()

find_path(FBXSDK_INCLUDE_DIR fbxsdk.h PATHS ${FBXSDK_INCLUDE_PATHS})
find_library(FBXSDK_LIBRARY libfbxsdk.a libfbxsdk-mt.lib PATHS ${FBXSDK_LIB_PATHS})

# On OSX we need to link to Cocoa when we statically link.
# (But if we didn't find FBX, don't link to Cocoa.)
if(APPLE)
  if (NOT(FBXSDK_LIBRARY STREQUAL ""))
      find_library(COCOA_LIBRARY Cocoa)
      list(APPEND FBXSDK_LIBRARY ${COCOA_LIBRARY})
  endif()
endif()

# Standard code to report whether we found the package or not.
#include(${CMAKE_CURRENT_LIST_DIR}/FindPackageHandleStandardArgs.cmake)
FIND_PACKAGE_HANDLE_STANDARD_ARGS(FBXSDK DEFAULT_MSG FBXSDK_LIBRARY FBXSDK_INCLUDE_DIR)
