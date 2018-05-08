#! /usr/bin/python

import re
import sys
import time

# Usage:
#   python replace-dllimport.py path/to/PINVOKE.cs
# Will replace all the swig-generated [DllImport(...)] statements with [DllImport(DllImportName)]

def replace_dllimport(filename):
    pattern = re.compile(r'DllImport\("[^"]*"')
    replace = 'DllImport(DllImportName'
    with open(filename) as infile:
        outdata = [ re.sub(pattern, replace, line) for line in infile.readlines() ]
    with open(filename, 'w') as outfile:
        outfile.write("// Converted DllImport statements using {} on {}\n\n".format(sys.argv[0], time.asctime()))
        for line in outdata:
            outfile.write(line)

if __name__ == "__main__":
    if len(sys.argv) < 2:
      print("USAGE: python replace-dllimport.py path/to/PINVOKE.cs\n" +
            "\n" +
            "Replaces all the swig-generated DllImport statements with a dll name compatible with the hack in fbxsdk.i")
      sys.exit(1)
    replace_dllimport(sys.argv[1])
