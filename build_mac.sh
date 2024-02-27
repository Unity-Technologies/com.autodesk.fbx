#!/bin/bash

## Any subsequent(*) commands which fail will cause the shell script to exit immediately, otherwise the job will fail silently.
set -e

brew list p7zip || brew install p7zip
cmake --version || brew install cmake
python ./build.py --stevedore --verbose --clean --yamato --fbxsdk "fbxsdk_to_upload"
mv build build-mac