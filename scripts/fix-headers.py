#! /usr/bin/python

# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************


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
        # this appears in fbxmatrix.h and in fbxaffinematrix.h (with different whitespace)
        line = re.sub(r'typedef const double\s*\(([A-Za-z0-9_]+)\)', r'typedef const double \1', line)
        # this appears in fbxlayer.h
        line = re.sub('const static', 'static const', line)
        line = re.sub(
            r'(class FBXSDK_DLL FbxLayerElementNormal : public FbxLayerElementTemplate<FbxVector4>)',
            r'%template(FbxLayerElementTemplateFbxVector4) FbxLayerElementTemplate<FbxVector4>; \
             %template(FbxLayerElementTemplateFbxVector2) FbxLayerElementTemplate<FbxVector2>; \
             %template(FbxLayerElementTemplateFbxColor) FbxLayerElementTemplate<FbxColor>; \
             %template(FbxLayerElementTemplateFbxSurfaceMaterial) FbxLayerElementTemplate<FbxSurfaceMaterial*>; \
             %template(FbxLayerElementArrayTemplateFbxSurfaceMaterial) FbxLayerElementArrayTemplate<FbxSurfaceMaterial*>; \
             \1',
             line
        )
        line = re.sub('FBX_DEPRECATED', '', line)
        
        # remember to write it out!
        fileout.write(line)
