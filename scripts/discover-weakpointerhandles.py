#! /usr/bin/python

# ***********************************************************************
# Copyright (c) 2017 Unity Technologies. All rights reserved.
#
# Licensed under the ##LICENSENAME##.
# See LICENSE.md file in the project root for full license information.
# ***********************************************************************

import sys
import re

# Input:
# 1- the output .i file
# 2- the result of swig -debug-typedef
# 3- the name of the base class we care about
# Output:
# - a swig .i file to be %included at the start of the fbxsdk.i file
# If the "base class" is actually derived from something we give an error.

# This should normally be integrated as part of the build system.
output_filename = sys.argv[1]
typedefs_filename = sys.argv[2]
rootclasses = sys.argv[3:]

# For each derived class, a list of classes it inherits from. If a class isn't
# in this dict it's not a derived class (it inherits from nothing).
baseclasses = dict()

with open(typedefs_filename) as typedef_file:
    current_scope = None
    bases = []
    def store():
        if current_scope and bases:
            baseclasses[current_scope] = bases
    for line in typedef_file:
        m = re.search("Type scope '(.*)'", line)
        if m:
            # changing scope; store the old one, clear the accumulating list
            store()
            current_scope = m.group(1)
            bases = []
        m = re.search("Inherits from '(.*)'", line)
        if m:
            bases.append(m.group(1))
    # end of file; remember the last block we read
    if current_scope and bases:
        store()


# Verify that each of the supposed root classes is actually a base class. If it
# derives from anything, we'll crash because some functions will be taking a
# handle and others a bare pointer.
for rootclass in rootclasses:
    ok = True
    if rootclass in baseclasses:
      print ("Error: {} is not a base class. Derives from {}.".format(
            baseclass, ', '.join(baseclasses[baseclass])))
      ok = False
    if not ok:
      sys.exit(1)

# Find all the classes that derive from the root classes.
handleclasses = set()
for rootclass in rootclasses:
  handleclasses.add(rootclass)
  for cls in baseclasses:
      if rootclass in baseclasses[cls]:
          handleclasses.add(cls)

# Emit the magic code: weakpointerhandle(X) for each class
with open(output_filename, 'w') as output:
    for cls in sorted(handleclasses):
        output.write("weakpointerhandle({});\n".format(cls))
