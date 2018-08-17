@echo off

SET installdir=%cd%\build\install

if exist build (
    rd /s /q build
)
md build
cd build
cmake .. -DCMAKE_BUILD_TYPE=Release ^
    -DCMAKE_INSTALL_PREFIX=${installdir} ^
    -G "Visual Studio 14 2015 Win64"
cmake --build . --target install --config Release
cd ..
