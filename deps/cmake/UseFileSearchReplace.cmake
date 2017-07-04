# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************

macro(SWIG_FILE_SEARCH_REPLACE swigmodule targetname filename searchstr replacestr)
    add_custom_target(
        ${targetname} ALL
        COMMAND ${PYTHON_EXECUTABLE} "${CMAKE_SOURCE_DIR}/scripts/file-search-replace.py" "${filename}" "${searchstr}" "${replacestr}"
        DEPENDS ${swigmodule} "${CMAKE_SOURCE_DIR}/scripts/file-search-replace.py"
        COMMENT "Replacing ${searchstr} with ${replacestr} in ${filename}"
    )
endmacro()