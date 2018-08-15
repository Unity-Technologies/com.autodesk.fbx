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

  set(_fbxsdk_artifact_name fbxsdk-mac-x64)
  set(_fbxsdk_artifact_id 2018.1.1_61b679df32b3967a62ca4a8285a79bcbb396b329c860307e7c667ec91745b236.7z)

  list(APPEND _fbxsdk_lib_paths "lib/clang/release")

elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Windows")
  SET(CMAKE_FIND_LIBRARY_PREFIXES "lib")

  list(APPEND CMAKE_SWIG_FLAGS "-D_WIN64")
  list(APPEND CMAKE_SWIG_FLAGS "-D_M_X64")
  list(APPEND CMAKE_SWIG_FLAGS "-D_MSC_VER")

  set(_fbxsdk_artifact_name fbxsdk-win-x64)
  set(_fbxsdk_artifact_id 2018.1.1_53da254f05c8aecb53045d5d74178004d0a7a76b367c1464d11e3ce3ce949925.7z)

  list(APPEND _fbxsdk_lib_paths "lib/vs2015/x64/release")

elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Linux")
  list(APPEND CMAKE_SWIG_FLAGS "-D__linux__")
  list(APPEND CMAKE_SWIG_FLAGS "-D__x86_64__")
  list(APPEND CMAKE_SWIG_FLAGS "-D__GNUC__")

  set(_fbxsdk_artifact_name fbxsdk-linux-x64)
  set(_fbxsdk_artifact_id 2018.1.1_9fd6bb314e0761415879fe91228f536902aa90082adb198889f71bdf094415fd.7z)

  list(APPEND _fbxsdk_lib_paths "lib/gcc4/x64/release")
endif()

set(_fbxsdk_INSTALL_PATH "${CMAKE_BINARY_DIR}/deps/${_fbxsdk_artifact_name}")
find_program(_fbxsdk_7ZA 7za)
message("-- Fetching FBXSDK from Stevedore: ${_fbxsdk_artifact_name}/${_fbxsdk_artifact_id}")
execute_process(COMMAND ${CMAKE_COMMAND} -E env "BEE_INTERNAL_STEVEDORE_7ZA=${_fbxsdk_7ZA}" mono ${CMAKE_SOURCE_DIR}/bee.exe steve internal-unpack testing ${_fbxsdk_artifact_name}/${_fbxsdk_artifact_id} ${_fbxsdk_INSTALL_PATH})
list(APPEND FBXSDK_INCLUDE_PATHS "${_fbxsdk_INSTALL_PATH}/include")
foreach(_fbxsdk_lib_path ${_fbxsdk_lib_paths})
  list(APPEND FBXSDK_LIB_PATHS "${_fbxsdk_INSTALL_PATH}/${_fbxsdk_lib_path}")
endforeach()
find_path(FBXSDK_INCLUDE_DIR fbxsdk.h PATHS ${FBXSDK_INCLUDE_PATHS})
find_library(FBXSDK_LIBRARY libfbxsdk.a libfbxsdk-mt.lib PATHS ${FBXSDK_LIB_PATHS})

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
find_library(FBXSDK_LIBRARY libfbxsdk.a libfbxsdk-md.lib PATHS ${FBXSDK_LIB_PATHS})

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
