#! /usr/bin/python

import re
import sys

# Usage:
#   python il2cpp-fix-swighelpers.py path/to/PINVOKE.cs
# Will add MonoPInvokeCallback attribute to all the swig-generated helper delegates

# attribute to add to delegate functions
mono_pinvoke_callback_attr = "    [AOT.MonoPInvokeCallback(typeof({0}))]\n{1}"

# pattern to parse delegate and function name from delegate declaration
# e.g. 'static SWIGStringDelegate stringDelegate = new SWIGStringDelegate(CreateString);'
# gives delegate = SWIGStringDelegate and function = CreateString
delegate_pattern = re.compile(r'static +(?P<delegate>[^ ]+Delegate) [^=]*= *new *[^ \(]+Delegate\((?P<function>[^ ]+)\)')

# pattern to parse function name
# e.g. 'static string CreateString(string cString) {'
# gives func_name = CreateString
function_pattern = re.compile(r'static [^ ]+ (?P<func_name>[^ \(]+)\(')

def il2cpp_fix_swighelpers(filename):
    # gather all the delegates to add the attribute to
    func_to_delegate_map = {} # map name of function to name of delegate
    outdata = []
    with open(filename) as infile:
        # On each line check if there is a new delegate, and add it to the map if there is.
        # Also check if there is a function in the map and add the attribute if there is.
        for line in infile.readlines():
            match = re.search(delegate_pattern, line)
            if match:
                # found a new delegate/function pair
                delegate = match.group('delegate')
                function = match.group('function')
                if delegate and function:
                    func_to_delegate_map[function] = delegate
            else:
                # found a function to add the attribute to
                match = re.search(function_pattern, line)
                if match:
                    func_name = match.group('func_name')
                    if func_name:
                        delegate_name = func_to_delegate_map.get(func_name)
                        if delegate_name:
                            line = mono_pinvoke_callback_attr.format(delegate_name, line)
                    
            outdata.append(line)
            
    with open(filename, 'w') as outfile:
        for line in outdata:
            outfile.write(line)

if __name__ == "__main__":
    if len(sys.argv) < 2:
      print("USAGE: python il2cpp-fix-swighelpers.py path/to/PINVOKE.cs\n" +
            "\n" +
            "Adds MonoPInvokeCallback attribute to all the swig-generated helper delegates so that they run when compiled with IL2CPP.")
      sys.exit(1)
    il2cpp_fix_swighelpers(sys.argv[1])
