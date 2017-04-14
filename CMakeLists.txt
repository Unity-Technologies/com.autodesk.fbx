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

# Find packages that we need.
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})

include(deps/cmake/UseFixHeaders.cmake)

find_package(FBXSDK REQUIRED)
set(Python_ADDITIONAL_VERSIONS 2.7)
find_package(PythonInterp REQUIRED)

# Unity is not required, but we can't run the tests without it.
# Note: In Linux the package name and Find{package}.cmake file are case sensitive 
# (e.g. has to be FindUNITY.cmake and UNITY or FindUnity.cmake and Unity)
find_package(Unity)

# Set up the include directories
include_directories(src)
include_directories(${FBXSDK_INCLUDE_DIR})

# Set up the swig run.
set_source_files_properties(src/fbxsdk.i PROPERTIES CPLUSPLUS ON)
set_source_files_properties(src/fbxsdk.i PROPERTIES SWIG_FLAGS "-namespace;FbxSdk")

# Set up extra swig dependencies. Must be before building the typemaps.
file(GLOB SWIG_MODULE_fbxsdk_csharp_EXTRA_DEPS "${CMAKE_SOURCE_DIR}/src/*.i")

# Fix the header files we need to fix. Must be before building the typemaps.
swig_fix_header(fbxsdk_csharp "${FBXSDK_INCLUDE_DIR}/fbxsdk/core/fbxpropertytypes.h")
swig_fix_header(fbxsdk_csharp "${FBXSDK_INCLUDE_DIR}/fbxsdk/core/math/fbxmatrix.h")
swig_fix_header(fbxsdk_csharp "${FBXSDK_INCLUDE_DIR}/fbxsdk/core/math/fbxaffinematrix.h")

# Autogenerate the weakpointerhandles.i file.
# It's a 2-step process:
# 1. generate the typedefs map. Unfortunately we need to generate all the code for that!
# 2. generate the weakpointerhandles.py from the typedefs.
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
# important: only add this dependency *after* adding the custom command, or else we get a circular loop
list(APPEND SWIG_MODULE_fbxsdk_csharp_EXTRA_DEPS "${CMAKE_BINARY_DIR}/weakpointerhandles.i")
add_custom_command(OUTPUT ${CMAKE_BINARY_DIR}/weakpointerhandles.i
        COMMAND ${PYTHON_EXECUTABLE}
        ARGS "${CMAKE_SOURCE_DIR}/scripts/discover-weakpointerhandles.py" "${CMAKE_BINARY_DIR}/weakpointerhandles.i" "${CMAKE_BINARY_DIR}/fbxsdk.typedefs"
                "FbxEmitter" "FbxManager"
        MAIN_DEPENDENCY ${CMAKE_BINARY_DIR}/fbxsdk.typedefs
        DEPENDS "${CMAKE_SOURCE_DIR}/scripts/discover-weakpointerhandles.py"
)


SET(CMAKE_SWIG_OUTDIR ${CMAKE_BINARY_DIR}/swig/generated/csharp)
    
swig_add_module(fbxsdk_csharp csharp src/fbxsdk.i)

swig_link_libraries(fbxsdk_csharp ${FBXSDK_LIBRARY})

# build the native C++ unit tests
add_executable(unity_tests tests/NativePerformance/PerformanceBenchmarks.cpp)
target_link_libraries(unity_tests ${FBXSDK_LIBRARY} ${CMAKE_DL_LIBS}) # need to include DL libs for Linux
set_target_properties(unity_tests PROPERTIES OUTPUT_NAME "PerformanceBenchmarks")
set_target_properties(unity_tests PROPERTIES SUFFIX ".exe")

install(DIRECTORY ${CMAKE_SOURCE_DIR}/tests/UnityTests DESTINATION ${CMAKE_BINARY_DIR}/tests)
install(DIRECTORY ${CMAKE_BINARY_DIR}/swig/generated/csharp DESTINATION fbxsdk FILES_MATCHING PATTERN "*.cs")
install(TARGETS fbxsdk_csharp DESTINATION fbxsdk)
install(TARGETS unity_tests DESTINATION fbxsdk)

enable_testing()
add_test(NAME run-all COMMAND ${UNITY_EDITOR_PATH} -batchmode -projectPath ${CMAKE_BINARY_DIR}/tests/UnityTests -runEditorTests)
