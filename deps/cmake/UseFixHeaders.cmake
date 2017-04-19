# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************

macro(SWIG_FIX_HEADER swigmodule inputheader)
    get_filename_component(basename ${inputheader} NAME)
    set(fix_header_output_dir "${CMAKE_BINARY_DIR}/${swigmodule}-fixed-headers")
    set(fix_header_output "${fix_header_output_dir}/${basename}")
    list(APPEND SWIG_MODULE_${swigmodule}_EXTRA_DEPS ${fix_header_output})
    add_custom_command(
        OUTPUT ${fix_header_output}
        COMMAND ${PYTHON_EXECUTABLE}
        ARGS "${CMAKE_SOURCE_DIR}/scripts/fix-headers.py" "${fix_header_output_dir}" "${inputheader}"
        MAIN_DEPENDENCY ${inputheader}
        DEPENDENCY "${CMAKE_SOURCE_DIR}/scripts/fix-headers.py"
    )
endmacro()
