choco -v -y install 7zip
py -3 build.py --stevedore --verbose --clean --yamato --fbxsdk "fbxsdk_to_upload"
ren build build-win
