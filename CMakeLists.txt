# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************

cmake_minimum_required (VERSION 2.6)

# has to be done before project is set, or else CMAKE_INSTALL_PREFIX will already by set to a default value
SET(CMAKE_INSTALL_PREFIX "${CMAKE_BINARY_DIR}/tests/UnityTests/Assets/Plugins" CACHE PATH "Install path prefix")
MESSAGE(STATUS "CMAKE_INSTALL_PREFIX is: " ${CMAKE_INSTALL_PREFIX})

project (fbxsdk_csharp)

# Default is a debug build.
if (CMAKE_BUILD_TYPE STREQUAL "")
  set(CMAKE_BUILD_TYPE "Debug")
  list(APPEND CMAKE_CXX_FLAGS "-DMEMORY_DEBUG")
endif()

# We build for c++11
set(CMAKE_CXX_STANDARD 11)

# promote warnings to errors
if(MSVC)
set(PROJECT_COMPILE_FLAGS "/WX")
else()
set(PROJECT_COMPILE_FLAGS "-Werror -Wno-error=null-dereference")
endif()

add_definitions(${PROJECT_COMPILE_FLAGS})

# build uto se the multithread, static version of the run-time library.
if (MSVC)
set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} /MT")
set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} /MTd")
endif()

list(APPEND CMAKE_MODULE_PATH ${CMAKE_SOURCE_DIR}/deps/cmake)

# don't generate stubs for inherited virtuals e.g. GetSelected
SET(CMAKE_SWIG_FLAGS -fvirtual)

###########################################################################
# Find packages that we need.
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})

include(deps/cmake/UseFixHeaders.cmake)

find_package(FBXSDK REQUIRED)
set(Python_ADDITIONAL_VERSIONS 2.7)
find_package(PythonInterp REQUIRED)
include(deps/cmake/RunPythonScript.cmake)

# Unity is not required, but we can't run the tests without it.
# Note: In Linux the package name and Find{package}.cmake file are case sensitive
# (e.g. has to be FindUNITY.cmake and UNITY or FindUnity.cmake and Unity)
find_package(Unity)

# Set up the include directories
include_directories(src)
include_directories(${FBXSDK_INCLUDE_DIR})

###########################################################################
# Set up the swig run.
set_source_files_properties(src/fbxsdk.i PROPERTIES CPLUSPLUS ON)
set_source_files_properties(src/fbxsdk.i PROPERTIES SWIG_FLAGS "-namespace;FbxSdk")

# Set up extra swig dependencies. Must be before building the typemaps.
file(GLOB SWIG_MODULE_fbxsdk_csharp_EXTRA_DEPS "${CMAKE_SOURCE_DIR}/src/*.i")

# Fix the header files we need to fix. Must be before building the typemaps.
swig_fix_header(fbxsdk_csharp "${FBXSDK_INCLUDE_DIR}/fbxsdk/core/fbxpropertytypes.h")
swig_fix_header(fbxsdk_csharp "${FBXSDK_INCLUDE_DIR}/fbxsdk/core/math/fbxmatrix.h")
swig_fix_header(fbxsdk_csharp "${FBXSDK_INCLUDE_DIR}/fbxsdk/core/math/fbxaffinematrix.h")
swig_fix_header(fbxsdk_csharp "${FBXSDK_INCLUDE_DIR}/fbxsdk/scene/geometry/fbxlayer.h")

# Autogenerate the weakpointerhandles.i file.
# It's a 2-step process:
# 1. generate the typedefs map. Unfortunately we need to generate all the code for that!
# 2. generate the weakpointerhandles.py from the typedefs.
# Important: only add dependencies *after* generating the typedefs, or it gets circular.
get_filename_component(FBXSDK_SWIG_I_FILE src/fbxsdk.i ABSOLUTE)
add_custom_command(OUTPUT ${CMAKE_BINARY_DIR}/fbxsdk.typedefs
        COMMAND ${CMAKE_COMMAND} -E make_directory "${CMAKE_BINARY_DIR}/fbxsdk.typedefs.temp"
        COMMAND "${SWIG_EXECUTABLE}"
        ARGS "-debug-typedef" "-DSWIG_GENERATING_TYPEDEFS" "-outdir" "${CMAKE_BINARY_DIR}/fbxsdk.typedefs.temp"
        "-c++" "-csharp" ${CMAKE_SWIG_FLAGS} -I${FBXSDK_INCLUDE_DIR} ${FBXSDK_SWIG_I_FILE}
        ">" "${CMAKE_BINARY_DIR}/fbxsdk.typedefs"
        BYPRODUCTS "${CMAKE_BINARY_DIR}/fbxsdk.typedefs.temp"
        MAIN_DEPENDENCY ${FBXSDK_SWIG_I_FILE}
        DEPENDS ${SWIG_MODULE_fbxsdk_csharp_EXTRA_DEPS}
)

# Find all the types that need weak-pointer handling. Add the top of each hierarchy as arguments.
fbxsharp_run_python(OUTPUT ${CMAKE_BINARY_DIR}/weakpointerhandles.i
        SCRIPT ${CMAKE_SOURCE_DIR}/scripts/discover-weakpointerhandles.py
        DEPENDS ${CMAKE_BINARY_DIR}/fbxsdk.typedefs
        TARGETDEPS SWIG_MODULE_fbxsdk_csharp_EXTRA_DEPS
        ARGS ${CMAKE_BINARY_DIR}/weakpointerhandles.i ${CMAKE_BINARY_DIR}/fbxsdk.typedefs "FbxEmitter" "FbxManager")

# Find the constants for FbxDataType.
fbxsharp_run_python(OUTPUT ${CMAKE_BINARY_DIR}/fbxdatatypeconstants.i
        SCRIPT ${CMAKE_SOURCE_DIR}/scripts/discover-datatypes.py
        DEPENDS ${FBXSDK_INCLUDE_DIR}/fbxsdk/core/fbxdatatypes.h
        TARGETDEPS SWIG_MODULE_fbxsdk_csharp_EXTRA_DEPS
        ARGS ${CMAKE_BINARY_DIR}/fbxdatatypeconstants.i ${FBXSDK_INCLUDE_DIR}/fbxsdk/core/fbxdatatypes.h)

# Finalize the swig output.
SET(CMAKE_SWIG_OUTDIR ${CMAKE_BINARY_DIR}/swig/generated/csharp)

swig_add_module(fbxsdk_csharp csharp src/fbxsdk.i)

swig_link_libraries(fbxsdk_csharp ${FBXSDK_LIBRARY})

# hide FBX symbols in case the target application also has a copy of FBX
if (${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
  set_target_properties(fbxsdk_csharp PROPERTIES LINK_FLAGS "-exported_symbols_list ${CMAKE_SOURCE_DIR}/deps/exported_symbols.txt")
endif()

###########################################################################
# build the native C++ unit tests
add_executable(unity_tests tests/NativePerformance/PerformanceBenchmarks.cpp)
target_link_libraries(unity_tests ${FBXSDK_LIBRARY} ${CMAKE_DL_LIBS}) # need to include DL libs for Linux
set_target_properties(unity_tests PROPERTIES OUTPUT_NAME "PerformanceBenchmarks")
set_target_properties(unity_tests PROPERTIES SUFFIX ".exe")

###########################################################################
include(deps/cmake/InstallMonoReflection.cmake)
install(DIRECTORY ${CMAKE_SOURCE_DIR}/tests/UnityTests DESTINATION ${CMAKE_BINARY_DIR}/tests)
install(DIRECTORY ${CMAKE_SOURCE_DIR}/examples/export/Assets/ DESTINATION ${CMAKE_BINARY_DIR}/tests/UnityTests/Assets/examples)
install(DIRECTORY ${CMAKE_BINARY_DIR}/swig/generated/csharp DESTINATION fbxsdk FILES_MATCHING PATTERN "*.cs")
install(DIRECTORY ${CMAKE_SOURCE_DIR}/src/optimized/ DESTINATION fbxsdk/csharp FILES_MATCHING PATTERN "*.cs")
install(TARGETS fbxsdk_csharp DESTINATION fbxsdk)
install(TARGETS unity_tests DESTINATION fbxsdk)

enable_testing()
add_test(NAME run-all COMMAND ${UNITY_EDITOR_PATH} -batchmode -projectPath ${CMAKE_BINARY_DIR}/tests/UnityTests -runEditorTests)
