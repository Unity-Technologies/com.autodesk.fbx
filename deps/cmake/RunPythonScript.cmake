# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************

# Run a python script to get an output file.
# fbxsharp_run_python(OUTPUT out
#       SCRIPT foo.py
#       [DEPENDS a.h b.cxx c.i...]
#       [ARGS arg1 arg2...]
#       TARGETDEPS SWIG_MODULE_${module}_EXTRA_DEPS)
#
# Will add a dependency from a given module onto the 'out' file, which will be
# created by running:
#     foo.py arg1 arg2

macro(FBXSHARP_RUN_PYTHON)
  cmake_parse_arguments(_runpy
        ""
        "OUTPUT;SCRIPT;TARGETDEPS"
        "ARGS;DEPENDS"
        ${ARGN})

  add_custom_command(OUTPUT ${_runpy_OUTPUT}
        COMMAND ${PYTHON_EXECUTABLE}
        ARGS ${_runpy_SCRIPT} ${_runpy_ARGS}
        DEPENDS ${_runpy_SCRIPT} ${_runpy_DEPENDS}
  )
  list(APPEND ${_runpy_TARGETDEPS} ${_runpy_OUTPUT})
endmacro()


# fbxsharp_discover_immutables(OUTPUT foo.i
#       HEADERS bar.h quux.h
#       TARGETDEPS SWIG_MODULE_${module}_EXTRA_DEPS)
macro(FBXSHARP_DISCOVER_IMMUTABLES)
  cmake_parse_arguments(_immutables
        ""
        "OUTPUT;TARGETDEPS"
        "HEADERS"
        ${ARGN})
  fbxsharp_run_python(OUTPUT ${_immutables_OUTPUT}
        SCRIPT ${CMAKE_SOURCE_DIR}/scripts/discover-immutables.py
        DEPENDS ${_immutables_HEADERS}
        ARGS ${_immutables_OUTPUT} ${_immutables_HEADERS}
        TARGETDEPS ${_immutables_TARGETDEPS}
  )
endmacro()
