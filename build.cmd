@echo off

SET installdir=%cd%\artifacts\install
setlocal enableextensions
md %installdir%
endlocal

if exist build (
    rd /s /q build
)
md build
cd build
cmake .. -DCMAKE_BUILD_TYPE=Release ^
    -DCMAKE_INSTALL_PREFIX=%installdir% ^
    -G "Visual Studio 14 2015 Win64"
REM can't find any other way to force SWIG to find its library
SET SWIG_LIB=%cd%\deps\swig-win-x64\Lib
cmake --build . --target install --config Release
cd ..
