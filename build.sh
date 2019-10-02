#!/bin/bash

if [ -e build ] ; then
    rm -rf build
fi

installdir=${PWD}/build/install
mkdir -p build
cd build
cmake .. -DCMAKE_BUILD_TYPE=Release \
    -DCMAKE_INSTALL_PREFIX=${installdir}

# can't find another way to tell SWIG where to find its libraries
SWIG_VERSION="3.0.12"
if test x`uname -s` = xDarwin; then
	export SWIG_LIB=${PWD}/deps/swig-mac-x64/share/swig/${SWIG_VERSION}
else
	export SWIG_LIB=${PWD}/deps/swig-linux-x64/share/swig/${SWIG_VERSION}
fi
cmake --build . --target install --config Release
