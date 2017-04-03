cmake_minimum_required (VERSION 2.6)
project (fbxsdk_csharp)

list(APPEND CMAKE_MODULE_PATH ${CMAKE_SOURCE_DIR}/deps/cmake)

# Find packages that we need.
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})

find_package(FBXSDK REQUIRED)

# Unity is not required, but we can't run the tests without it.
find_package(UNITY)

# Set up the include directories
include_directories(src)
include_directories(${FBXSDK_INCLUDE_DIR})

# Set up the swig run.
set_source_files_properties(src/fbxsdk.i PROPERTIES CPLUSPLUS ON)
set_source_files_properties(src/fbxsdk.i PROPERTIES SWIG_FLAGS "-namespace;FbxSdk")

SET(CMAKE_SWIG_OUTDIR ${CMAKE_BINARY_DIR}/swig/generated/csharp)

# Set up extra swig dependencies.
file(GLOB SWIG_MODULE_fbxsdk_csharp_EXTRA_DEPS "${CMAKE_SOURCE_DIR}/src/*.i")

# compile flags for shared library
add_definitions(-DFBXSDK_SHARED)

swig_add_module(fbxsdk_csharp csharp src/fbxsdk.i)

# OSX force SWIG to generate a libXXX[.dylib] module file, not just  XXX.so
IF(UNIX)
    SET_TARGET_PROPERTIES(${SWIG_MODULE_fbxsdk_csharp_REAL_NAME} PROPERTIES 
        PREFIX "lib")
    IF(APPLE)
      SET_TARGET_PROPERTIES(${SWIG_MODULE_fbxsdk_csharp_REAL_NAME} PROPERTIES
        SUFFIX ".dylib")
    ENDIF(APPLE)
ENDIF(UNIX)
   
swig_link_libraries(fbxsdk_csharp ${FBXSDK_DYLIBRARY})

install(DIRECTORY ${CMAKE_BINARY_DIR}/swig/generated/csharp DESTINATION fbxsdk FILES_MATCHING PATTERN "*.cs")
install(TARGETS fbxsdk_csharp DESTINATION fbxsdk)
