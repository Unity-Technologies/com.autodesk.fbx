
# The list of FBX SDK versions to look for.
# The first of these is the most-preferred.
set(FBXSDK_VERSIONS "2017.1" "2016.0")
message("Using versions ${FBXSDK_VERSIONS}")

# Platform-specific code.
# Autodesk installs the FBX SDK in a non-standard spot so we need to find it.
# Result of this code:
# - Add some platform defines to CMAKE_SWIG_FLAGS that we need for FBX SDK.
# - Set FBXSDK_INCLUDE_PATHS and FBXSDK_LIB_PATHS to where we should look for to find the SDK.
if(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
  list(APPEND CMAKE_SWIG_FLAGS "-D__APPLE__")
  list(APPEND CMAKE_SWIG_FLAGS "-D__x86_64__")
  list(APPEND CMAKE_SWIG_FLAGS "-DFBXSDK_COMPILER_GNU")

  foreach(VERSION ${FBXSDK_VERSIONS})
    message("Looking for fbxsdk in /Applications/Autodesk/FBX SDK/${VERSION}")
    list(APPEND FBXSDK_INCLUDE_PATHS "/Applications/Autodesk/FBX SDK/${VERSION}/include")
    list(APPEND FBXSDK_LIB_PATHS "/Applications/Autodesk/FBX SDK/${VERSION}/lib/clang/release")
  endforeach()

elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Windows")
  SET(CMAKE_FIND_LIBRARY_PREFIXES "lib")
  SET(CMAKE_FIND_LIBRARY_SUFFIXES ".lib" ".dll")

  list(APPEND CMAKE_SWIG_FLAGS "-D_WIN64")
  list(APPEND CMAKE_SWIG_FLAGS "-D_M_X64")
  list(APPEND CMAKE_SWIG_FLAGS "-D_MSC_VER")

  foreach(VERSION ${FBXSDK_VERSIONS})
    message("Windows: Looking for fbxsdk in C:/Program Files/Autodesk/FBX/FBX SDK/${VERSION}")
    list(APPEND FBXSDK_INCLUDE_PATHS "C:/Program Files/Autodesk/FBX/FBX SDK/${VERSION}/include")
    list(APPEND FBXSDK_LIB_PATHS "C:/Program Files/Autodesk/FBX/FBX SDK/${VERSION}/lib/vs2010/x64/release")
  endforeach()  
endif()

message("Looking for fbxsdk.h in ${FBXSDK_INCLUDE_PATHS}")
find_path(FBXSDK_INCLUDE_DIR fbxsdk.h PATHS ${FBXSDK_INCLUDE_PATHS})

message("Looking for fbxsdk library in ${FBXSDK_LIB_PATHS}")
find_library(FBXSDK_LIBRARY fbxsdk PATHS ${FBXSDK_LIB_PATHS})

# Standard code to report whether we found the package or not.
#include(${CMAKE_CURRENT_LIST_DIR}/FindPackageHandleStandardArgs.cmake)
FIND_PACKAGE_HANDLE_STANDARD_ARGS(FBXSDK DEFAULT_MSG FBXSDK_LIBRARY FBXSDK_INCLUDE_DIR)
