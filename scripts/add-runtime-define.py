#! /usr/bin/python

import os
import sys

# Usage:
#   python add-runtime-define.py path/to/cs_files
# Will add a #if UNITY_EDITOR || FBXSDK_RUNTIME to the top of each generated CS file

define = "#if UNITY_EDITOR || FBXSDK_RUNTIME\n"
endif = "\n#endif // UNITY_EDITOR || FBXSDK_RUNTIME"

def add_runtime_define(cs_file_path):
    for filename in os.listdir(cs_file_path):
        if not filename.endswith(".cs"):
            continue
        filename = os.path.join(cs_file_path, filename)
        with open(filename) as infile:
            outdata = infile.read()
            outdata = define + outdata + endif

        with open(filename, 'w') as outfile:
            outfile.write(outdata)

if __name__ == "__main__":
    if len(sys.argv) < 2:
      print("USAGE: python add-runtime-define.py path/to/cs_files\n" +
            "\n" +
            "Will add a #define UNITY_EDITOR || FBXSDK_RUNTIME to the top of each generated CS file")
      sys.exit(1)
    add_runtime_define(sys.argv[1])
