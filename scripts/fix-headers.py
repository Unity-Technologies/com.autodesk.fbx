#! /usr/bin/python

# Read the FBX header files and fix the bit that swig can't parse.
#
# To fix a new header file that's giving you grief:
# 1- figure out what doesn't parse
# 2- instead of %include "fbxsdk/core/whatever.h" do %include "fixed-headers/whatever.h"
# 3- add "fbxsdk/core/whatever.h" to the fix-headers command in CMakeLists.txt
# 4- add re.sub lines as needed to the main loop below.
#
# See e.g. https://github.com/swig/swig/issues/965

import os
import re
import sys

if len(sys.argv) < 3:
  sys.stderr.write("USAGE: {} outdir input/foo1.h [input/foo2.h ...]\n".format(sys.argv[0]))
  sys.stderr.write("Munges foo1.h and foo2.h for swig to parse correctly, puts the results in outdir/.\n")
  sys.exit(1)

outdir = sys.argv[1]
if not os.path.isdir(outdir):
    os.makedirs(outdir)

for inname in sys.argv[2:]:
    outname = os.path.join(outdir, os.path.basename(inname))
    print ("{} => {}".format(inname, outname))
    with open(inname) as filein:
     with open(outname, 'w') as fileout:
      for line in filein:
        # this appears in fbxpropertytypes.h
        line = re.sub('unsigned const short', 'unsigned short', line)

        # remember to write it out!
        fileout.write(line)
