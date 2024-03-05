#!/bin/bash

## Any subsequent(*) commands which fail will cause the shell script to exit immediately, otherwise the job will fail silently.
set -e

sudo add-apt-repository ppa:ubuntu-toolchain-r/test -y
sudo apt-get update
sudo apt-get -y install zlib1g-dev
sudo apt-get -y install cmake
sudo apt-get -y install libxml2-dev
sudo apt-get -y install gcc-9 g++-9
sudo apt-get -y install p7zip mono-devel
# Ensure correct version of gcc and g++ used
# https://stackoverflow.com/questions/17275348/how-to-specify-new-gcc-path-for-cmake
CC=`which gcc-9` CXX=`which g++-9` python ./build.py --stevedore --verbose --clean --yamato
mv build build-ubuntu