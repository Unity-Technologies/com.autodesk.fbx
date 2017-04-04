#! /usr/bin/python

import sys
import re

# Input:
# 1- the output .i file
# 2- the name of the base class we care about
# 3- the result of swig -debug-typedef
# Output:
# - a swig .i file to be %included at the start of the fbxsdk.i file
# If the "base class" is actually derived from something we give an error.

# This should normally be integrated as part of the build system.
output_filename = sys.argv[1]
baseclass = sys.argv[2]
typedefs_filename = sys.argv[3]

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


# Verify that the base class is actually a base class. If it derives from
# anything, we'll crash.
if baseclass in baseclasses:
  print ("Error: {} is not a base class. Derives from {}.".format(
        baseclass, ', '.join(baseclasses[baseclass])))
  sys.exit(1)

# Find all the classes that derive from the base class.
derivedclasses = set()
for cls in baseclasses:
  if baseclass in baseclasses[cls]:
    derivedclasses.add(cls)
# Also add the class itself.
derivedclasses.add(baseclass)

# Emit the magic code.
with open(output_filename, 'w') as output:
    for cls in sorted(derivedclasses):
        output.write("weakpointerhandle({});\n".format(cls))
