# Look for the executable to find the Unity editor executable.
# If UNITY_EDITOR_PATH we use it, otherwise we set it.

# Platform-specific code.
if (NOT DEFINED UNITY_EDITOR_PATH)
    if(${CMAKE_SYSTEM_NAME} STREQUAL "Darwin")
      list(APPEND UNITY_EXECUTABLE_PATHS "/Applications/Unity/Unity.app/Contents/MacOS/")
    elseif(${CMAKE_SYSTEM_NAME} STREQUAL "Windows")
      list(APPEND UNITY_EXECUTABLE_PATHS "c:/Program Files/Unity/")
    endif()

    find_program(UNITY_EDITOR_PATH Unity PATHS ${UNITY_EXECUTABLE_PATHS})
endif()

# Standard code to report whether we found the package or not.
#include(${CMAKE_CURRENT_LIST_DIR}/FindPackageHandleStandardArgs.cmake)
FIND_PACKAGE_HANDLE_STANDARD_ARGS(UNITY DEFAULT_MSG UNITY_EDITOR_PATH)
