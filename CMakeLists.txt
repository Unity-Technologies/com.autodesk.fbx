cmake_minimum_required (VERSION 2.6)
project (fbxsdk_csharp)

list(APPEND CMAKE_MODULE_PATH ${CMAKE_SOURCE_DIR}/deps/cmake)

# Find packages that we need.
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})

find_package(FBXSDK REQUIRED)
find_package(PYTHONINTERP REQUIRED)

# Unity is not required, but we can't run the tests without it.
find_package(UNITY)

# Set up the include directories
include_directories(src)
include_directories(${FBXSDK_INCLUDE_DIR})

# Set up the swig run.
set_source_files_properties(src/fbxsdk.i PROPERTIES CPLUSPLUS ON)
set_source_files_properties(src/fbxsdk.i PROPERTIES SWIG_FLAGS "-namespace;FbxSdk")

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
add_custom_command(OUTPUT ${CMAKE_BINARY_DIR}/weakpointerhandles.i
        COMMAND ${PYTHON_EXECUTABLE}
        ARGS "${CMAKE_SOURCE_DIR}/src/discover-weakpointerhandles.py" "${CMAKE_BINARY_DIR}/weakpointerhandles.i" FbxEmitter "${CMAKE_BINARY_DIR}/fbxsdk.typedefs"
        MAIN_DEPENDENCY ${CMAKE_BINARY_DIR}/fbxsdk.typedefs
)
list(APPEND SWIG_MODULE_fbxsdk_csharp_EXTRA_DEPS "${CMAKE_BINARY_DIR}/weakpointerhandles.i")

SET(CMAKE_SWIG_OUTDIR ${CMAKE_BINARY_DIR}/swig/generated/csharp)
swig_add_module(fbxsdk_csharp csharp src/fbxsdk.i)
swig_link_libraries(fbxsdk_csharp ${FBXSDK_LIBRARY})

install(DIRECTORY ${CMAKE_BINARY_DIR}/swig/generated/csharp DESTINATION fbxsdk FILES_MATCHING PATTERN "*.cs")
install(TARGETS fbxsdk_csharp DESTINATION fbxsdk)
