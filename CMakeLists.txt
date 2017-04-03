cmake_minimum_required (VERSION 2.6)
project (fbxsdk_csharp)

# We build for c++11
set(CMAKE_CXX_STANDARD 11)

list(APPEND CMAKE_MODULE_PATH ${CMAKE_SOURCE_DIR}/deps/cmake)

# Find packages that we need.
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})

find_package(FBXSDK REQUIRED)

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

SET(CMAKE_SWIG_OUTDIR ${CMAKE_BINARY_DIR}/swig/generated/csharp)
swig_add_module(fbxsdk_csharp csharp src/fbxsdk.i)
swig_link_libraries(fbxsdk_csharp ${FBXSDK_LIBRARY})

# build the native C++ unit tests
add_executable(unity_tests tests/NativePerformance/PerformanceBenchmarks.cpp)
target_link_libraries(unity_tests ${FBXSDK_LIBRARY} ${CMAKE_DL_LIBS}) # need to include DL libs for Linux
set_target_properties(unity_tests PROPERTIES OUTPUT_NAME "PerformanceBenchmarks")
set_target_properties(unity_tests PROPERTIES SUFFIX ".exe")

install(DIRECTORY ${CMAKE_BINARY_DIR}/swig/generated/csharp DESTINATION fbxsdk FILES_MATCHING PATTERN "*.cs")
install(TARGETS fbxsdk_csharp DESTINATION fbxsdk)
install(TARGETS unity_tests DESTINATION fbxsdk)

enable_testing()
add_test(NAME run-all COMMAND ${UNITY_EDITOR_PATH} -batchmode -projectPath ${CMAKE_SOURCE_DIR}/tests/UnityTests -runEditorTests)
