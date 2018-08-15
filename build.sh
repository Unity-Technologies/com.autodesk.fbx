#!/bin/bash

if [[ -e build ]]; then
    rm -rf build
fi

installdir=${PWD}/build/install
mkdir -p build
pushd build
cmake .. -DCMAKE_BUILD_TYPE=Release \
    -DCMAKE_INSTALL_PREFIX=${installdir}
cmake --build . --target install --config Release
popd
