#!/bin/bash

if [[ -e build ]]; then
    rm -rf build
fi

installdir=${PWD}/artifacts/install
mkdir -p ${installdir}

mkdir -p build
pushd build
cmake .. -DCMAKE_BUILD_TYPE=Release \
    -DCMAKE_INSTALL_PREFIX=${installdir}
# can't find another way to tell SWIG where to find its libraries
export SWIG_LIB=${PWD}/deps/swig-mac-x64/share/swig/3.0.12
cmake --build . --target install --config Release
popd
