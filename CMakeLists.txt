# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************

# Some parts of this file *must* be above the 'project' line and others *must*
# be below it. Be careful about moving statements around.

cmake_minimum_required (VERSION 3.08)

# Default is a release build.
if (NOT CMAKE_BUILD_TYPE)
  # CMAKE_BUILD_TYPE is special, so we have to CACHE FORCE to actually set it,
  # or else our 'set' has very wonky scope.
  set(CMAKE_BUILD_TYPE "Release" CACHE STRING "" FORCE)
elseif (CMAKE_BUILD_TYPE STREQUAL "Debug")
  list(APPEND CMAKE_CXX_FLAGS "-DMEMORY_DEBUG")
endif()
message(STATUS "Building for ${CMAKE_BUILD_TYPE}")


# ***********************************************************************
project (UnityFbxSdkNative)
# ***********************************************************************


# CMake docs say this should be before project but it doesn't actually work that way.
set(CMAKE_OSX_ARCHITECTURES "arm64;x86_64")

list(APPEND CMAKE_MODULE_PATH ${CMAKE_SOURCE_DIR}/cmake)
list(APPEND CMAKE_PREFIX_PATH "${CMAKE_BINARY_DIR}/deps")

# We build for c++11
set(CMAKE_CXX_STANDARD 11)

if(MSVC)
set(PROJECT_COMPILE_FLAGS "/WX")
elseif(CMAKE_HOST_APPLE)
# requires gcc 6 or higher
set(PROJECT_COMPILE_FLAGS "-Werror -Wno-error=null-dereference")
else()
# No Werror because there's invalid use of incomplete type FbxAnimEvaluator in fbxproperty.h
set(PROJECT_COMPILE_FLAGS "-Wall -Wno-format -Wno-strict-aliasing")
endif()

add_definitions(${PROJECT_COMPILE_FLAGS})

# build to use the multithread, static version of the run-time library.
if (MSVC)
set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} /MD")
set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} /MDd")
endif()

# don't generate stubs for inherited virtuals e.g. GetSelected
SET(CMAKE_SWIG_FLAGS -fvirtual)

option(USE_STEVEDORE "Use stevedore to get dependencies (useful on contiuous integration platforms, default off)" OFF)

if (${USE_STEVEDORE} STREQUAL "ON")
    include(cmake/Stevedore.cmake)

# fetch dependencies
    set(stevedore_repo_name testing)

    if(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
        set(fbxsdk_artifact_name fbxsdk-mac-x64)
        set(fbxsdk_artifact_id 2020.2_bfa28d76a77eb68eb9254c389b244e2a024037424da655baf1dd3f554b09056e.7z)

        set(swig_artifact_name swig-mac-x64)
        set(swig_artifact_id 3.0.12_814b172e58d71a5f5a35d3e4a1bd07a16d50ac223249d561719a9e71409115e3.7z)
        set(swig_executable_name swig)
    elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Windows")
        set(fbxsdk_artifact_name fbxsdk-win-x64)
        set(fbxsdk_artifact_id 2020.2_63f59fe9bbdbaadd9dc8ed03be87bc102fbb1604ee798abf190954f3124c8859.7z)

        set(swig_artifact_name swig-win-x64)
        set(swig_artifact_id 3.0.12_2e184e8ce59cca5da026ac275ef53aff60e8b60ec5edb576efced3f7168f82d8.7z)
        set(swig_executable_name swig.exe)
    elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Linux")
        set(fbxsdk_artifact_name fbxsdk-linux-x64)
        set(fbxsdk_artifact_id 2020.2_d1094eaf7b70549ec8ed5eec542e88a790b92110dd91c0317ba95c25771daeb5.7z)

        set(swig_artifact_name swig-linux-x64)
        set(swig_artifact_id 3.0.12_f3f14d565afd0ec44d3d77298f708f41bd12ef1fa209a1535226cf3e30a5f15b.7z)
        set(swig_executable_name swig)
    endif()

    stevedore(internal-unpack ${stevedore_repo_name} ${fbxsdk_artifact_name}/${fbxsdk_artifact_id} "${CMAKE_BINARY_DIR}/deps/${fbxsdk_artifact_name}")
    stevedore(internal-unpack ${stevedore_repo_name} ${swig_artifact_name}/${swig_artifact_id} "${CMAKE_BINARY_DIR}/deps/${swig_artifact_name}")

    file(GLOB_RECURSE SWIG_EXECUTABLE "${CMAKE_BINARY_DIR}/deps/${swig_artifact_name}/${swig_executable_name}")
    file(GLOB_RECURSE swig_lib "${CMAKE_BINARY_DIR}/deps/${swig_artifact_name}/swig.swg")
    get_filename_component(swig_lib "${swig_lib}" DIRECTORY)
    file(TO_NATIVE_PATH "${swig_lib}" swig_lib_native)
    set(ENV{SWIG_LIB} "${swig_lib_native}")
    message("FOUND ${swig_lib_native}")
endif()

###########################################################################
# Find packages that we need.

find_package(SWIG 3.0.12 REQUIRED) # lower version not guaranteed to work on Linux
include(${SWIG_USE_FILE})

include(cmake/UseFixHeaders.cmake)

find_package(FBXSDK REQUIRED)
#find_package(Unity REQUIRED)
#find_package(CSharpCompiler REQUIRED)
find_package(Doxygen)

set(Python_ADDITIONAL_VERSIONS 2.7)
find_package(PythonInterp REQUIRED)
include(cmake/RunPythonScript.cmake)

# Set up the include directories
include_directories(Source)
include_directories(${FBXSDK_INCLUDE_DIR})

# Get git tag as package version
find_package(Git REQUIRED)
execute_process(COMMAND ${GIT_EXECUTABLE} describe --tags HEAD
        OUTPUT_VARIABLE PACKAGE_VERSION
        OUTPUT_STRIP_TRAILING_WHITESPACE)
# only keep x.y.z[-preview.n]
string(REGEX REPLACE "^.*([0-9]\.[0-9]\.[0-9].*)$" "\\1" PACKAGE_VERSION "${PACKAGE_VERSION}")

# Get git revision hash as package revision
execute_process(COMMAND ${GIT_EXECUTABLE} rev-parse HEAD
        OUTPUT_VARIABLE PACKAGE_REVISION
        OUTPUT_STRIP_TRAILING_WHITESPACE)

# Get git repo url
execute_process(COMMAND ${GIT_EXECUTABLE} config --get remote.origin.url
        OUTPUT_VARIABLE PACKAGE_REPO_URL
        OUTPUT_STRIP_TRAILING_WHITESPACE)

configure_file(${CMAKE_SOURCE_DIR}/proto.com.autodesk.fbx/package.json.in ${CMAKE_BINARY_DIR}/package.json @ONLY)

###########################################################################
# Set up the swig run.
set_source_files_properties(Source/fbxsdk.i PROPERTIES CPLUSPLUS ON)
set_source_files_properties(Source/fbxsdk.i PROPERTIES SWIG_FLAGS "-namespace;Autodesk.Fbx;-Werror")

# Set up extra swig dependencies. Must be before building the typemaps.
file(GLOB SWIG_MODULE_UnityFbxSdkNative_EXTRA_DEPS "${CMAKE_SOURCE_DIR}/Source/*.i")

# Fix the header files we need to fix. Must be before building the typemaps.
swig_fix_header(UnityFbxSdkNative "${FBXSDK_INCLUDE_DIR}/fbxsdk/core/fbxpropertytypes.h")
swig_fix_header(UnityFbxSdkNative "${FBXSDK_INCLUDE_DIR}/fbxsdk/core/math/fbxmatrix.h")
swig_fix_header(UnityFbxSdkNative "${FBXSDK_INCLUDE_DIR}/fbxsdk/core/math/fbxaffinematrix.h")
swig_fix_header(UnityFbxSdkNative "${FBXSDK_INCLUDE_DIR}/fbxsdk/scene/geometry/fbxlayer.h")

# Find the constants for FbxDataType.
fbxsharp_discover_immutables(OUTPUT ${CMAKE_BINARY_DIR}/fbxdatatypeconstants.i
        TARGETDEPS SWIG_MODULE_UnityFbxSdkNative_EXTRA_DEPS
        HEADERS ${FBXSDK_INCLUDE_DIR}/fbxsdk/core/fbxdatatypes.h
)

# Find the properties for FbxDocumentInfo.
fbxsharp_discover_immutables(OUTPUT ${CMAKE_BINARY_DIR}/fbxdocumentinfoimmutables.i
        TARGETDEPS SWIG_MODULE_UnityFbxSdkNative_EXTRA_DEPS
        HEADERS ${FBXSDK_INCLUDE_DIR}/fbxsdk/scene/fbxdocumentinfo.h
)

# Find the constants and properties for FbxSurfaceMaterial, Lambert and Phong
fbxsharp_discover_immutables(OUTPUT ${CMAKE_BINARY_DIR}/fbxsurfaceimmutables.i
        TARGETDEPS SWIG_MODULE_UnityFbxSdkNative_EXTRA_DEPS
        HEADERS ${FBXSDK_INCLUDE_DIR}/fbxsdk/scene/shading/fbxsurfacematerial.h
                ${FBXSDK_INCLUDE_DIR}/fbxsdk/scene/shading/fbxsurfacelambert.h
                ${FBXSDK_INCLUDE_DIR}/fbxsdk/scene/shading/fbxsurfacephong.h
)

# Find the constants and properties for FbxTexture and FbxFileTexture.
fbxsharp_discover_immutables(OUTPUT ${CMAKE_BINARY_DIR}/fbxtextureimmutables.i
        TARGETDEPS SWIG_MODULE_UnityFbxSdkNative_EXTRA_DEPS
        HEADERS ${FBXSDK_INCLUDE_DIR}/fbxsdk/scene/shading/fbxtexture.h ${FBXSDK_INCLUDE_DIR}/fbxsdk/scene/shading/fbxfiletexture.h
)

# Find the constants and properties for FbxMarker
fbxsharp_discover_immutables(OUTPUT ${CMAKE_BINARY_DIR}/fbxmarkerimmutables.i
        TARGETDEPS SWIG_MODULE_UnityFbxSdkNative_EXTRA_DEPS
        HEADERS ${FBXSDK_INCLUDE_DIR}/fbxsdk/scene/geometry/fbxmarker.h
)

# Find the constants and properties for FbxLight.
fbxsharp_discover_immutables(OUTPUT ${CMAKE_BINARY_DIR}/fbxlightimmutables.i
        TARGETDEPS SWIG_MODULE_UnityFbxSdkNative_EXTRA_DEPS
        HEADERS ${FBXSDK_INCLUDE_DIR}/fbxsdk/scene/geometry/fbxlight.h
)

# Autogenerate the weakpointerhandles.i file.
# It's a 2-step process:
# 1. generate the typedefs map. Unfortunately we need to generate all the code for that!
# 2. generate the weakpointerhandles.py from the typedefs.
# Important: only add the weakpointerhandles.i *after* this or it gets circular.
#       But other dependencies (e.g. for immutables) should go before.
get_filename_component(FBXSDK_SWIG_I_FILE Source/fbxsdk.i ABSOLUTE)
add_custom_command(OUTPUT ${CMAKE_BINARY_DIR}/fbxsdk.typedefs
        COMMAND ${CMAKE_COMMAND} -E make_directory "${CMAKE_BINARY_DIR}/fbxsdk.typedefs.temp"
        COMMAND "${SWIG_EXECUTABLE}"
        ARGS "-debug-typedef" "-DSWIG_GENERATING_TYPEDEFS"
        "-c++" "-csharp" "-Werror"
        "-outdir" "${CMAKE_BINARY_DIR}/fbxsdk.typedefs.temp"
        "-o" "${CMAKE_BINARY_DIR}/fbxsdk.typedefs.temp/wrap.cxx"
        ${CMAKE_SWIG_FLAGS} -I${FBXSDK_INCLUDE_DIR} ${FBXSDK_SWIG_I_FILE}
        ">" "${CMAKE_BINARY_DIR}/fbxsdk.typedefs"
        BYPRODUCTS "${CMAKE_BINARY_DIR}/fbxsdk.typedefs.temp"
        MAIN_DEPENDENCY ${FBXSDK_SWIG_I_FILE}
        DEPENDS ${SWIG_MODULE_UnityFbxSdkNative_EXTRA_DEPS}
)

# Find all the types that need weak-pointer handling. Add the top of each hierarchy as arguments.
fbxsharp_run_python(OUTPUT ${CMAKE_BINARY_DIR}/weakpointerhandles.i
        SCRIPT ${CMAKE_SOURCE_DIR}/scripts/discover-weakpointerhandles.py
        DEPENDS ${CMAKE_BINARY_DIR}/fbxsdk.typedefs
        TARGETDEPS SWIG_MODULE_UnityFbxSdkNative_EXTRA_DEPS
        ARGS ${CMAKE_BINARY_DIR}/weakpointerhandles.i ${CMAKE_BINARY_DIR}/fbxsdk.typedefs "FbxEmitter" "FbxManager")


# Finalize the swig output: build & link the library, munge the DllImport statements
SET(CMAKE_SWIG_OUTDIR ${CMAKE_BINARY_DIR}/swig/generated/csharp)

swig_add_library(UnityFbxSdkNative
         TYPE MODULE
         LANGUAGE csharp
         SOURCES Source/fbxsdk.i)

fbxsharp_run_python_postbuild(TARGET UnityFbxSdkNative
        SCRIPT ${CMAKE_SOURCE_DIR}/scripts/replace-dllimport.py
        ARGS ${CMAKE_BINARY_DIR}/swig/generated/csharp/NativeMethods.cs)
        
fbxsharp_run_python_postbuild(TARGET UnityFbxSdkNative
        SCRIPT ${CMAKE_SOURCE_DIR}/scripts/il2cpp-fix-swighelpers.py
        ARGS ${CMAKE_BINARY_DIR}/swig/generated/csharp/NativeMethods.cs)

fbxsharp_run_python_postbuild(TARGET UnityFbxSdkNative
        SCRIPT ${CMAKE_SOURCE_DIR}/scripts/add-runtime-define.py
        ARGS ${CMAKE_BINARY_DIR}/swig/generated/csharp)

swig_link_libraries(UnityFbxSdkNative ${FBXSDK_LIBRARY})

# Hide FBX symbols in case the target application also has a copy of FBX
# Happens to also reduce the output binary a lot too.
if (${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
  set_target_properties(UnityFbxSdkNative PROPERTIES BUNDLE TRUE)
  target_link_libraries(UnityFbxSdkNative "-exported_symbols_list ${CMAKE_CURRENT_SOURCE_DIR}/Source/exported_symbols.txt -Wl,-x,-dead_strip")
elseif (${CMAKE_SYSTEM_NAME} STREQUAL "Linux")
  target_link_libraries(UnityFbxSdkNative "-Wl,--version-script=${CMAKE_CURRENT_SOURCE_DIR}/Source/version-script.txt -Wl,-x,--gc-sections")
endif()

if (${YAMATO} STREQUAL "OFF")
        ###########################################################################
        # enable a test that the C# and C++ vectors behave the same
        add_executable(vector_test tests/Vectors/Vectors.cpp)
        target_link_libraries(vector_test ${FBXSDK_LIBRARY} ${CMAKE_DL_LIBS}) # need to include DL libs for Linux
        add_custom_command(OUTPUT vector_test.txt COMMAND vector_test ARGS ">" vector_test.txt DEPENDS vector_test)
        add_custom_target(vector_test_output ALL DEPENDS vector_test.txt)

        ###########################################################################
        # build the native C++ unit tests
        add_executable(unity_tests tests/NativePerformance/PerformanceBenchmarks.cpp)
        target_link_libraries(unity_tests ${FBXSDK_LIBRARY} ${CMAKE_DL_LIBS}) # need to include DL libs for Linux
        set_target_properties(unity_tests PROPERTIES OUTPUT_NAME "PerformanceBenchmarks")
        if(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
        set_target_properties(unity_tests PROPERTIES SUFFIX "-mac-x64")
        elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Windows")
        set_target_properties(unity_tests PROPERTIES SUFFIX "-win-x64.exe")
        elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Linux")
        find_package(Threads)
        set_target_properties(unity_tests PROPERTIES SUFFIX "-linux-x64")
        target_link_libraries(unity_tests ${CMAKE_THREAD_LIBS_INIT}) # need to include pthread libs for Linux
        endif()

        install(DIRECTORY ${CMAKE_SOURCE_DIR}/proto.com.autodesk.fbx.testing/ DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx/Tests PATTERN "*.in" EXCLUDE)
        # copy C++ performance tests
        install(TARGETS unity_tests DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx/Tests)
        install(FILES ${CMAKE_BINARY_DIR}/vector_test.txt DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx/Tests)
        
        ###########################################################################
endif()

install(DIRECTORY ${CMAKE_SOURCE_DIR}/proto.com.autodesk.fbx/ DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx PATTERN "*.in" EXCLUDE)
install(FILES ${CMAKE_BINARY_DIR}/package.json DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx)
install(DIRECTORY ${CMAKE_BINARY_DIR}/swig/generated/csharp/ DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx/Runtime/Scripts FILES_MATCHING PATTERN "*.cs")
install(DIRECTORY ${CMAKE_SOURCE_DIR}/Source/optimized/ DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx/Runtime/Scripts FILES_MATCHING PATTERN "*.cs")

install(TARGETS UnityFbxSdkNative DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx/Editor/Plugins)



install(FILES ${CMAKE_SOURCE_DIR}/CHANGELOG.md DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx)
install(FILES ${CMAKE_SOURCE_DIR}/LICENSE.md DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx)
install(FILES "${CMAKE_SOURCE_DIR}/Third Party Notices.md" DESTINATION ${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx)

# Copy the test-run framework

install(DIRECTORY "${CMAKE_SOURCE_DIR}/tests/RuntimeTests/Editor" DESTINATION  "${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx/Tests/")
install(FILES "${CMAKE_SOURCE_DIR}/tests/RuntimeTests/Editor.meta" DESTINATION "${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx/Tests")
install(DIRECTORY "${CMAKE_SOURCE_DIR}/tests/RuntimeTests/Runtime" DESTINATION  "${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx/Tests/")
install(FILES "${CMAKE_SOURCE_DIR}/tests/RuntimeTests/Runtime.meta" DESTINATION "${CMAKE_INSTALL_PREFIX}/com.autodesk.fbx/Tests")
