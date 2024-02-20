#! /usr/bin/python

# USAGE:
#    zip-fbxsdk.py /path/to/fbxsdk
# Will create a .7z file in the appropriate place for the present platform,
# including just the files it needs.

import os
import subprocess
import sys

fbxsdk_root = sys.argv[1]

def find_files():
    """
    Return all the files, in top-down order.

    Do *not* include directory names or else 7z will grab all the contents.

    Write the names relative to fbxsdk_root, i.e. remove fbxsdk_root from the `root`.
    """
    # Include all the include files.
    for root, dirs, files in os.walk(fbxsdk_root + "/include"):
        relative_root = root[len(fbxsdk_root)+1:]
        for name in files:
            yield (f"{relative_root}/{name}")

    # We only use stevedore for release builds (and using /MD on windows).
    # Discard other libraries since they won't be needed.
    for root, dirs, files in os.walk(fbxsdk_root + "/lib"):
        relative_root = root[len(fbxsdk_root)+1:]
        # Don't upload debug versions.
        # Removing them from the list prevents walk from searching them.
        if 'debug' in dirs:
            dirs.remove('debug')

        # On windows, don't upload /MT versions, we only use /MD
        # On other platforms, there's no -mt.lib files so this just wastes a couple ms.
        for name in filter(lambda x: not x.endswith("-mt.lib"), files):
            yield (f"{relative_root}/{name}")

def find_archive_name():
    if sys.platform == 'darwin':
        return "fbxsdk_to_upload/fbxsdk-mac-x64.7z"
    elif sys.platform.startswith('win'):
        return "fbxsdk_to_upload/fbxsdk-win-x64.7z"
    else:
        return "fbxsdk_to_upload/fbxsdk-linux-x64.7z"


curdir = os.getcwd()
outfile = curdir + "/" + find_archive_name()
with open("file-list.txt", "w") as f:
    for name in find_files():
        print(name, file=f)
retcode = subprocess.check_call(["7z", "a", "-spf", "-bb", outfile, f"@{curdir}/file-list.txt"], stderr=subprocess.STDOUT, cwd=fbxsdk_root)
sys.exit(retcode)
