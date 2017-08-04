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
      set(UNITY_PATH "c:/Program Files/Unity")
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


