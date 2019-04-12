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

  list(APPEND _fbxsdk_lib_paths "lib/clang/release")

elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Windows")
  SET(CMAKE_FIND_LIBRARY_PREFIXES "lib")

  list(APPEND CMAKE_SWIG_FLAGS "-D_WIN64")
  list(APPEND CMAKE_SWIG_FLAGS "-D_M_X64")
  list(APPEND CMAKE_SWIG_FLAGS "-D_MSC_VER")

  list(APPEND _fbxsdk_lib_paths "lib/vs2015/x64/release")

elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Linux")
  list(APPEND CMAKE_SWIG_FLAGS "-D__linux__")
  list(APPEND CMAKE_SWIG_FLAGS "-D__x86_64__")
  list(APPEND CMAKE_SWIG_FLAGS "-D__GNUC__")

  list(APPEND _fbxsdk_lib_paths "lib/gcc4/x64/release")
endif()

# Iterate over the versions. Pick the first one (reverse-alphabetically)
file(GLOB _fbxsdk_VERSIONS LIST_DIRECTORIES true "${CMAKE_PREFIX_PATH}/*")
list(SORT _fbxsdk_VERSIONS)
list(REVERSE _fbxsdk_VERSIONS)
foreach(_fbxsdk_PATH ${_fbxsdk_VERSIONS})
  list(APPEND FBXSDK_INCLUDE_PATHS "${_fbxsdk_PATH}/include")
  foreach(_fbxsdk_lib_path ${_fbxsdk_lib_paths})
    list(APPEND FBXSDK_LIB_PATHS "${_fbxsdk_PATH}/${_fbxsdk_lib_path}")
  endforeach()
endforeach()

find_path(FBXSDK_INCLUDE_DIR fbxsdk.h PATHS ${FBXSDK_INCLUDE_PATHS})
find_library(FBXSDK_LIBRARY libfbxsdk.a libfbxsdk-md.lib PATHS ${FBXSDK_LIB_PATHS})

find_file(_fbxsdk_VERSION_HEADER fbxsdk_version.h PATHS ${FBXSDK_INCLUDE_DIR}/fbxsdk)
if(${_fbxsdk_VERSION_HEADER} STREQUAL "_fbxsdk_VERSION_HEADER-NOTFOUND")
  message(FATAL_ERROR "Couldn't find fbxsdk_version.h")
else()
  file(READ ${_fbxsdk_VERSION_HEADER} _fbxsdk_VERSION_HEADER_CONTENTS)
  string(REGEX MATCH "FBXSDK_VERSION_MAJOR[\t ]+[0-9]+" FBXSDK_VERSION_MAJOR "${_fbxsdk_VERSION_HEADER_CONTENTS}")
  string(REGEX REPLACE "FBXSDK_VERSION_MAJOR[\t ]+" "" FBXSDK_VERSION_MAJOR "${FBXSDK_VERSION_MAJOR}")
  string(REGEX MATCH "FBXSDK_VERSION_MINOR[\t ]+[0-9]+" FBXSDK_VERSION_MINOR "${_fbxsdk_VERSION_HEADER_CONTENTS}")
  string(REGEX REPLACE "FBXSDK_VERSION_MINOR[\t ]+" "" FBXSDK_VERSION_MINOR "${FBXSDK_VERSION_MINOR}")
  string(REGEX MATCH "FBXSDK_VERSION_POINT[\t ]+[0-9]+" FBXSDK_VERSION_POINT "${_fbxsdk_VERSION_HEADER_CONTENTS}")
  string(REGEX REPLACE "FBXSDK_VERSION_POINT[\t ]+" "" FBXSDK_VERSION_POINT "${FBXSDK_VERSION_POINT}")
  set(FBXSDK_VERSION ${FBXSDK_VERSION_MAJOR}.${FBXSDK_VERSION_MINOR}.${FBXSDK_VERSION_POINT})
endif()

# On OSX we need to link to Cocoa when we statically link.
# (But if we didn't find FBX, don't link to Cocoa.)
if(APPLE)
  if (NOT(FBXSDK_LIBRARY STREQUAL ""))
      find_library(COCOA_LIBRARY Cocoa)
      list(APPEND FBXSDK_LIBRARY ${COCOA_LIBRARY})
  endif()
endif()

# Standard code to report whether we found the package or not.
FIND_PACKAGE_HANDLE_STANDARD_ARGS(FBXSDK
  FOUND_VAR FBXSDK_FOUND
  REQUIRED_VARS
    FBXSDK_LIBRARY
    FBXSDK_INCLUDE_DIR
    FBXSDK_VERSION FBXSDK_VERSION_MAJOR FBXSDK_VERSION_MINOR FBXSDK_VERSION_POINT
  VERSION_VAR
    FBXSDK_VERSION
)
