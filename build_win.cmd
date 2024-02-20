choco -v -y install 7zip
py -3 build.py --stevedore --verbose --clean --yamato
ren build build-win
