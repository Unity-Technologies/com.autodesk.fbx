# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************

# Look for the executable to find the Unity editor executable.
# If UNITY_EDITOR_PATH we use it, otherwise we set it.

# Platform-specific code.
if (NOT DEFINED UNITY_EDITOR_PATH)
    if(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
      set(UNITY_PATH "/Applications/Unity")
      set(MONODEVELOP_PATH "${UNITY_PATH}/MonoDevelop.app")
      list(APPEND UNITY_EXECUTABLE_PATHS "${UNITY_PATH}/Unity.app/Contents/MacOS/")
      
    elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Windows")
      SET(CMAKE_FIND_LIBRARY_SUFFIXES ".dll")
      
      set(UNITY_PATH "c:/Program Files/Unity2017.1.0f3")
      set(MONODEVELOP_PATH "${UNITY_PATH}/MonoDevelop")
      list(APPEND UNITY_EXECUTABLE_PATHS "${UNITY_PATH}/Editor/")
      
    elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Linux")
    
      set(UNITY_PATH "/opt/Unity")
      set(MONODEVELOP_PATH "${UNITY_PATH}/MonoDevelop")
      list(APPEND UNITY_EXECUTABLE_PATHS "${UNITY_PATH}/Editor/")
      
    endif()

    find_program(UNITY_EDITOR_PATH Unity PATHS ${UNITY_EXECUTABLE_PATHS})

    # Standard code to report whether we found the package or not.
    FIND_PACKAGE_HANDLE_STANDARD_ARGS(Unity DEFAULT_MSG UNITY_EDITOR_PATH)
else()
    message("Using ${UNITY_EDITOR_PATH}")
endif()

if (NOT DEFINED UNITY_EDITOR_DLL_PATH)
    SET(CMAKE_FIND_LIBRARY_SUFFIXES ".dll")
    
    if(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
        get_filename_component(UNITY_EDITOR_DLL_PATH "${UNITY_EDITOR_PATH}" PATH)
        get_filename_component(UNITY_EDITOR_DLL_PATH "${UNITY_EDITOR_DLL_PATH}" DIRECTORY)

        set(UNITY_EDITOR_DLL_PATH "${UNITY_EDITOR_DLL_PATH}/Managed")

    elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Windows")
        get_filename_component(UNITY_EDITOR_DLL_PATH "${UNITY_EDITOR_PATH}" PATH)
        set(UNITY_EDITOR_DLL_PATH "${UNITY_EDITOR_DLL_PATH}/Data/Managed")

    elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Linux")
        get_filename_component(UNITY_EDITOR_DLL_PATH "${UNITY_EDITOR_PATH}" PATH)
        set(UNITY_EDITOR_DLL_PATH "${UNITY_EDITOR_DLL_PATH}/Data/Managed")
    endif()

    message("Looking for UnityEditor.dll in ${UNITY_EDITOR_DLL_PATH}")
    find_library(CSHARP_UNITYEDITOR_LIBRARY UnityEditor.dll PATH ${UNITY_EDITOR_DLL_PATH})
    message("Found: ${CSHARP_UNITYEDITOR_LIBRARY}")
    
    # Standard code to report whether we found the package or not.
    FIND_PACKAGE_HANDLE_STANDARD_ARGS(UnityEditor DEFAULT_MSG CSHARP_UNITYEDITOR_LIBRARY)
else()
    message("Using ${UNITY_EDITOR_DLL_PATH}")
endif()
