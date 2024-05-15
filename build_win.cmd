choco -v -y install 7zip
py -3 build.py --stevedore --verbose --clean --yamato --target %PROCESSOR_ARCHITECTURE%
IF %ERRORLEVEL% NEQ 0 (
    echo Build command failed on Windows with error code: %ERRORLEVEL%
    exit %ERRORLEVEL%
)
ren build build-win
