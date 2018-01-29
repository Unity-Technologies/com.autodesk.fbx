# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************

cmake_minimum_required (VERSION 3.8)

# Default is a release build.
if (NOT CMAKE_BUILD_TYPE)
  # CMAKE_BUILD_TYPE is special, so we have to CACHE FORCE to actually set it,
  # or else our 'set' has very wonky scope.
  set(CMAKE_BUILD_TYPE "Release" CACHE STRING "" FORCE)
elseif (CMAKE_BUILD_TYPE STREQUAL "Debug")
  list(APPEND CMAKE_CXX_FLAGS "-DMEMORY_DEBUG")
endif()
message(STATUS "Building for ${CMAKE_BUILD_TYPE}")

# has to be done before project is set, or else CMAKE_INSTALL_PREFIX will already be set to a default value
SET(CMAKE_INSTALL_PREFIX "${CMAKE_BINARY_DIR}/tests/UnityTests/Assets/FbxSdk" CACHE PATH "Install path prefix")
MESSAGE(STATUS "CMAKE_INSTALL_PREFIX is: " ${CMAKE_INSTALL_PREFIX})

option(CREATE_PACKAGE "Create Package" OFF)
if (NOT DEFINED PACKAGE_VERSION)
    set(PACKAGE_VERSION "sprint43")
endif()

if (NOT DEFINED PACKAGE_PATH)
    set(PACKAGE_PATH "${CMAKE_BINARY_DIR}/FbxSdk_${PACKAGE_VERSION}.unitypackage")
endif()

project (UnityFbxSdkNative)

# We build for c++11
set(CMAKE_CXX_STANDARD 11)

# promote warnings to errors
if(MSVC)
set(PROJECT_COMPILE_FLAGS "/WX")
else()
# requires gcc 6 or higher
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
find_package(SWIG 3.0.12 REQUIRED) # lower version not guaranteed to work on Linux
include(${SWIG_USE_FILE})

include(deps/cmake/UseFixHeaders.cmake)

find_package(FBXSDK REQUIRED)
find_package(Unity REQUIRED)
find_package(CSharpAssemblies REQUIRED)
find_package(Doxygen)

set(Python_ADDITIONAL_VERSIONS 2.7)
find_package(PythonInterp REQUIRED)
include(deps/cmake/RunPythonScript.cmake)
include(deps/cmake/BuildCSharp.cmake)

# Set up the include directories
include_directories(src)
include_directories(${FBXSDK_INCLUDE_DIR})

###########################################################################
# Set up the swig run.
set_source_files_properties(src/fbxsdk.i PROPERTIES CPLUSPLUS ON)
set_source_files_properties(src/fbxsdk.i PROPERTIES SWIG_FLAGS "-namespace;Unity.FbxSdk;-Werror")

# Set up extra swig dependencies. Must be before building the typemaps.
file(GLOB SWIG_MODULE_UnityFbxSdkNative_EXTRA_DEPS "${CMAKE_SOURCE_DIR}/src/*.i")

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
get_filename_component(FBXSDK_SWIG_I_FILE src/fbxsdk.i ABSOLUTE)
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


# Finalize the swig output.
SET(CMAKE_SWIG_OUTDIR ${CMAKE_BINARY_DIR}/swig/generated/csharp)

swig_add_library(UnityFbxSdkNative
         TYPE MODULE
         LANGUAGE csharp
         SOURCES src/fbxsdk.i)

swig_link_libraries(UnityFbxSdkNative ${FBXSDK_LIBRARY})

# hide FBX symbols in case the target application also has a copy of FBX
if (${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
  set_target_properties(UnityFbxSdkNative PROPERTIES
  LINK_FLAGS "-exported_symbols_list ${CMAKE_SOURCE_DIR}/deps/exported_symbols.txt"
  BUNDLE TRUE)
endif()

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
set_target_properties(unity_tests PROPERTIES SUFFIX ".exe")

###########################################################################
# Add target for creating a package
# TODO: synthesize the right project depending on configuration.
# Make install does the right thing in all cases but it's overkill in release.
add_custom_command(
    OUTPUT ${PACKAGE_PATH}
        COMMAND "${CMAKE_COMMAND}" --build . --target install --config Release
        COMMAND "${UNITY_EDITOR_PATH}" -batchmode -projectPath ${CMAKE_BINARY_DIR}/tests/UnityTests -exportPackage Assets/FbxSdk ${PACKAGE_PATH} -quit
        COMMENT "Creating Unity Package ${PACKAGE_PATH}"
        DEPENDS UnityFbxSdkNative
    )
add_custom_target(unitypackage DEPENDS ${PACKAGE_PATH})

###########################################################################
# Add target to replace the version number in README.txt
set(README_TARGET readme_replace)
add_custom_target(
    ${README_TARGET}
    COMMAND ${PYTHON_EXECUTABLE}
            "${CMAKE_SOURCE_DIR}/scripts/file-search-replace.py"
            "${CMAKE_INSTALL_PREFIX}/README.txt"
            "**Version**:"
            "**Version**: ${PACKAGE_VERSION}"
    DEPENDS "${CMAKE_SOURCE_DIR}/scripts/file-search-replace.py"
    COMMENT "Replacing version number in README.txt"
)

###########################################################################
# Add custom command to build the generated csharp files into a multi-platform DLL
if("${CMAKE_BUILD_TYPE}" STREQUAL "Release")
    set(CLASS_LIBRARY_DEST "${CMAKE_BINARY_DIR}/${CMAKE_BUILD_TYPE}")
    set(CLASS_LIBRARY_NAME "UnityFbxSdk.dll")

    add_custom_command(OUTPUT ${CLASS_LIBRARY_DEST}
        COMMAND ${CMAKE_COMMAND} -E make_directory ${CLASS_LIBRARY_DEST}
    )
    fbxsharp_compile_csharp(OUTPUT ${CLASS_LIBRARY_DEST}/${CLASS_LIBRARY_NAME}
            EXTRA_ARGS
                "/target:library"
            SOURCES
                "${CMAKE_SOURCE_DIR}/scripts/Properties/*.cs"
                "${CMAKE_SOURCE_DIR}/src/optimized/*.cs"
                "${CMAKE_BINARY_DIR}/swig/generated/csharp/*.cs"
            REFERENCES
                "${CSHARP_MSCORLIB_LIBRARY}"
                "${CSHARP_SYSTEM_CORE_LIBRARY}"
                "${CSHARP_SYSTEM_LIBRARY}"
                "${CSHARP_UNITYEDITOR_LIBRARY}"
                "${CSHARP_UNITYENGINE_LIBRARY}"
            DEPENDS
                UnityFbxSdkNative
                "${CLASS_LIBRARY_DEST}"
    )
    add_custom_target(UnityFbxSdk ALL DEPENDS ${CLASS_LIBRARY_DEST}/${CLASS_LIBRARY_NAME})

    fbxsharp_compile_csharp(OUTPUT "${CLASS_LIBRARY_DEST}/UnityDependancyTest.exe"
            SOURCES ${CMAKE_SOURCE_DIR}/tests/UnityEditorDependancy/UnityDependancyTest.cs
            REFERENCES "${CLASS_LIBRARY_DEST}/${CLASS_LIBRARY_NAME}"
            DEPENDS UnityFbxSdk
    )
    add_custom_target(
        unity_dependency_test ALL
        DEPENDS "${CLASS_LIBRARY_DEST}/UnityDependancyTest.exe"
        COMMENT "Compiling Unity Dependancy Test"
    )
endif()

###########################################################################
# Doxygen documentation
# add a target to generate API documentation with Doxygen
set(DOXYGEN_TARGET doc)
set(DOC_ZIP_NAME "docs.zip")
if(DOXYGEN_FOUND)
    configure_file(${CMAKE_CURRENT_SOURCE_DIR}/Doxyfile.in ${CMAKE_CURRENT_BINARY_DIR}/Doxyfile @ONLY)
    add_custom_command(OUTPUT ${CMAKE_BINARY_DIR}/doxygen.stamp
        COMMAND ${DOXYGEN_EXECUTABLE} ${CMAKE_CURRENT_BINARY_DIR}/Doxyfile
        COMMAND cmake -E touch ${CMAKE_BINARY_DIR}/doxygen.stamp
        WORKING_DIRECTORY ${CMAKE_CURRENT_BINARY_DIR}
        DEPENDS ${CMAKE_CURRENT_BINARY_DIR}/Doxyfile
        COMMENT "Generating API documentation with Doxygen" VERBATIM
    )
    add_custom_command(OUTPUT ${DOC_ZIP_NAME}
        COMMAND ${CMAKE_COMMAND} -E tar "cfv" ${CMAKE_BINARY_DIR}/${DOC_ZIP_NAME} --format=zip
           "${CMAKE_CURRENT_BINARY_DIR}/docs"
        DEPENDS ${CMAKE_BINARY_DIR}/doxygen.stamp
    )
    add_custom_target(${DOXYGEN_TARGET} DEPENDS ${DOC_ZIP_NAME})
endif(DOXYGEN_FOUND)

###########################################################################
# Install Unit Tests and Mono Reflection for test converage
include(deps/cmake/InstallMonoReflection.cmake)

if(CMAKE_BUILD_TYPE STREQUAL "Release")
    install(FILES ${CLASS_LIBRARY_DEST}/${CLASS_LIBRARY_NAME} DESTINATION Plugins)
    # install all the unit tests outside the package folder
    install(DIRECTORY ${CMAKE_SOURCE_DIR}/tests/UnityTests
        DESTINATION ${CMAKE_BINARY_DIR}/tests)
else() # Debug Build
    install(DIRECTORY ${CMAKE_SOURCE_DIR}/tests/UnityTests
        DESTINATION ${CMAKE_BINARY_DIR}/tests
        PATTERN "Editor/UnitTests" EXCLUDE
        PATTERN "Editor/UseCaseTests" EXCLUDE)

    install(DIRECTORY ${CMAKE_SOURCE_DIR}/tests/UnityTests/Assets/Editor/UnitTests DESTINATION Editor)
    install(DIRECTORY ${CMAKE_SOURCE_DIR}/tests/UnityTests/Assets/Editor/UseCaseTests DESTINATION Editor)
    install(DIRECTORY ${CMAKE_BINARY_DIR}/swig/generated/csharp/ DESTINATION CSharp FILES_MATCHING PATTERN "*.cs")
    install(DIRECTORY ${CMAKE_SOURCE_DIR}/src/optimized/ DESTINATION CSharp FILES_MATCHING PATTERN "*.cs")
endif()

install(DIRECTORY ${CMAKE_SOURCE_DIR}/examples/export/Assets/ DESTINATION ${CMAKE_BINARY_DIR}/tests/UnityTests/Assets/examples)
install(DIRECTORY ${CMAKE_SOURCE_DIR}/examples/import/Assets/ DESTINATION ${CMAKE_BINARY_DIR}/tests/UnityTests/Assets/examples)

if (${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
    install(TARGETS UnityFbxSdkNative DESTINATION Plugins/x64/MacOS)
elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Linux")
    install(TARGETS UnityFbxSdkNative DESTINATION Plugins/x64/Linux)
else()
    install(TARGETS UnityFbxSdkNative DESTINATION Plugins/x64/Windows)
endif()

# copy C++ performance tests
install(TARGETS unity_tests DESTINATION ${CMAKE_BINARY_DIR}/tests/UnityTests/Assets/Plugins/fbxsdk)
install(FILES ${CMAKE_BINARY_DIR}/vector_test.txt DESTINATION ${CMAKE_BINARY_DIR}/tests/UnityTests/)

install(FILES ${CMAKE_SOURCE_DIR}/docs/README.txt DESTINATION ${CMAKE_INSTALL_PREFIX})
if(CMAKE_BUILD_TYPE STREQUAL "Release")
    install(FILES ${CMAKE_SOURCE_DIR}/docs/RELEASE_LICENSE.txt DESTINATION ${CMAKE_INSTALL_PREFIX} RENAME LICENSE.txt)

    # TODO: Figure out how to make this work so that the executable does not have to be in the same folder as or
    #       above the UnityFbxSdk.dll.
    install(FILES ${CLASS_LIBRARY_DEST}/UnityDependancyTest.exe
                  ${CMAKE_SOURCE_DIR}/tests/UnityEditorDependancy/UnityDependancyTest.exe.config
            DESTINATION ${CMAKE_BINARY_DIR}/tests/UnityTests/Assets)
else()
    install(FILES ${CMAKE_SOURCE_DIR}/docs/DEBUG_LICENSE.txt DESTINATION ${CMAKE_INSTALL_PREFIX} RENAME LICENSE.txt)
endif()

install(CODE "execute_process(COMMAND cmake --build . --target ${README_TARGET})")

if(DOXYGEN_FOUND AND CMAKE_BUILD_TYPE STREQUAL "Release")
    # generate and copy docs. This has to happen after the README.txt has been copied into the build dir
    install(CODE "execute_process(COMMAND cmake --build . --target ${DOXYGEN_TARGET})")
    install(FILES ${CMAKE_BINARY_DIR}/${DOC_ZIP_NAME} DESTINATION ${CMAKE_INSTALL_PREFIX})
endif()

# Create the package after all the installs complete if CREATE_PACKAGE set to ON
if(CREATE_PACKAGE)
    install(CODE "execute_process(COMMAND cmake --build . --target ${PACKAGE_TARGET})")
endif()

enable_testing()
add_test(NAME run-all COMMAND "${UNITY_EDITOR_PATH}" -batchmode -projectPath ${CMAKE_BINARY_DIR}/tests/UnityTests runEditorTests -quit)
