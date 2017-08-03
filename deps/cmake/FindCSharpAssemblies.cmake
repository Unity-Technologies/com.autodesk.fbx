# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************

# The list of .NET versions to look for.
# The first of these is the most-preferred.
set(NET_COMPILER_VERSIONS "4.5" "4.0.30319" "4.0")
set(NET_REFERENCE_ASSEMBLIES_VERSIONS "3.5" "2.0")
set(NET_LIB_VERSIONS "2.0.50727" "2.0")
message("Using .Net versions ${NET_COMPILER_VERSIONS}")

# Platform-specific code.
if(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")

  set(NET_PATH "/Applications/Unity/MonoDevelop.app/Contents/Frameworks/Mono.framework/Versions/Current/lib/mono")
  set(MONO_COMPILER_PATH "/Applications/Unity/MonoDevelop.app/Contents/Frameworks/Mono.framework/Versions/Current/bin")
  
  foreach(VERSION ${NET_COMPILER_VERSIONS})
    message("Looking for mcs.exe in ${NET_PATH}/${VERSION}")
    list(APPEND CSHARP_COMPILER_PATHS "${NET_PATH}/${VERSION}")
  endforeach()
  
  foreach(VERSION ${NET_REFERENCE_ASSEMBLIES_VERSIONS})
    message("Looking for System.Core.dll in ${NET_PATH}/${VERSION}")
    list(APPEND REFERENCE_ASSEMBLIES_PATHS "${NET_PATH}/${VERSION}")
  endforeach()

  foreach(VERSION ${NET_LIB_VERSIONS})
    message("Looking for System.dll and mscorlib.dll in ${NET_PATH}/${VERSION}")
    list(APPEND DLL_PATHS "${NET_PATH}/${VERSION}")
  endforeach()
  
  message("Looking for mono.exe in ${MONO_COMPILER_PATH}")
  find_program(MONO_COMPILER mono PATH ${MONO_COMPILER_PATH} NO_DEFAULT_PATH)
  find_program(MONO_COMPILER mono PATH ${MONO_COMPILER_PATH})
  message("Found: ${MONO_COMPILER}\n")
  
  message("Looking for mcs.exe in ${CSHARP_COMPILER_PATHS}")
  find_program(CSHARP_COMPILER mcs PATHS ${CSHARP_COMPILER_PATHS} NO_DEFAULT_PATH)
  find_program(CSHARP_COMPILER mcs PATHS ${CSHARP_COMPILER_PATHS})
  message("Found: ${CSHARP_COMPILER}\n")
  
elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Windows")
  SET(CMAKE_FIND_LIBRARY_SUFFIXES ".dll")

  set(NET_PATH "C:/Windows/Microsoft.NET/Framework")
  set(REFERENCE_ASSEMBLIES_PATH "C:/Program Files \(x86\)/Reference Assemblies/Microsoft/Framework")
  
  foreach(VERSION ${NET_COMPILER_VERSIONS})
    list(APPEND CSHARP_COMPILER_PATHS "${NET_PATH}/v${VERSION}")
  endforeach()
  
  foreach(VERSION ${NET_REFERENCE_ASSEMBLIES_VERSIONS})
    list(APPEND REFERENCE_ASSEMBLIES_PATHS "${REFERENCE_ASSEMBLIES_PATH}/v${VERSION}")
  endforeach()

  foreach(VERSION ${NET_LIB_VERSIONS})
    list(APPEND DLL_PATHS "${NET_PATH}/v${VERSION}")
  endforeach()
  
  message("Looking for Csc.exe in ${CSHARP_COMPILER_PATHS}")
  find_program(CSHARP_COMPILER csc PATHS ${CSHARP_COMPILER_PATHS} NO_DEFAULT_PATH)
  find_program(CSHARP_COMPILER csc PATHS ${CSHARP_COMPILER_PATHS})
  message("Found: ${CSHARP_COMPILER}\n")
  
elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Linux")
  message(WARNING "Linux: Not Implemented")
endif()


message("Looking for mscorlib.dll in ${DLL_PATHS}")
find_library(MSCORLIB_LIBRARY mscorlib.dll PATHS ${DLL_PATHS} NO_DEFAULT_PATH)
find_library(MSCORLIB_LIBRARY mscorlib.dll PATHS ${DLL_PATHS})
message("Found: ${MSCORLIB_LIBRARY}\n")

message("Looking for System.dll in ${DLL_PATHS}")
find_library(SYSTEM_LIBRARY System.dll PATHS ${DLL_PATHS} NO_DEFAULT_PATH)
find_library(SYSTEM_LIBRARY System.dll PATHS ${DLL_PATHS})
message("Found: ${SYSTEM_LIBRARY}\n")

message("Looking for System.Core.dll in ${REFERENCE_ASSEMBLIES_PATHS}")
find_library(SYSTEM_CORE_LIBRARY System.Core.dll PATHS ${REFERENCE_ASSEMBLIES_PATHS} NO_DEFAULT_PATH)
find_library(SYSTEM_CORE_LIBRARY System.Core.dll PATHS ${REFERENCE_ASSEMBLIES_PATHS})
message("Found: ${SYSTEM_CORE_LIBRARY}\n")

# Standard code to report whether we found the package or not.
#include(${CMAKE_CURRENT_LIST_DIR}/FindPackageHandleStandardArgs.cmake)
FIND_PACKAGE_HANDLE_STANDARD_ARGS(CSHARP_ASSEMBLIES DEFAULT_MSG CSHARP_COMPILER MSCORLIB_LIBRARY SYSTEM_LIBRARY SYSTEM_CORE_LIBRARY)
