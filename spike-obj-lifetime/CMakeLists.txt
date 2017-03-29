cmake_minimum_required (VERSION 2.6)
project (Tutorial)
list(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_LIST_DIR}/cmake")

# Use C++11 (but not the GNU extensions!)
set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_EXTENSIONS OFF)

# Find packages that we need.
find_package(SWIG REQUIRED)
include(${SWIG_USE_FILE})

find_package(FBXSDK REQUIRED)

# Set up the include directories
include_directories(${CMAKE_CURRENT_SOURCE_DIR})
include_directories(${FBXSDK_INCLUDE_DIR})

# Set up the test program.
add_executable(test-free test-free.cpp)
target_link_libraries(test-free ${FBXSDK_LIBRARY})

# Set up the swig run.
#set_source_files_properties(fbxsdk.i PROPERTIES CPLUSPLUS ON)
#set_source_files_properties(fbxsdk.i PROPERTIES SWIG_FLAGS "-namespace;FbxSdk")
#set(CMAKE_SWIG_OUTDIR, "out")
#swig_add_module(fbxsdk_sharp csharp fbxsdk.i)
#swig_link_libraries(fbxsdk_sharp ${FBXSDK_LIBRARY})

#install(DIRECTORY . DESTINATION fbxsharp/csharp FILES_MATCHING PATTERN "*.cs")
#install(TARGETS fbxsdk_sharp DESTINATION fbxsharp/csharp)
