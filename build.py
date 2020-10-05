#!/usr/bin/python -B
import argparse
import shutil
import os
import subprocess
import sys

parser = argparse.ArgumentParser(description='Parse the options')
parser.add_argument('--swig', type=str, dest='swig_location', help='Root location of the swig executable')
parser.add_argument('--fbxsdk', type=str, dest='fbxsdk_location', help='location of the FBX SDK')
parser.add_argument('-s', '--stevedore', action='store_true', dest='use_stevedore', help='Use stevedore (used for internal builds)')
parser.add_argument('-n', '--ninja', action='store_true', dest='use_ninja', help='Generate Ninja build files')
parser.add_argument('-t', '--build_type', default='Release', dest='build_type', help='Build type to do (Release, Debug, ...)')
parser.add_argument('-z', '--zap', '-c', '--clean', action='store_true', dest='clean_build', help='Removes the build directory')
parser.add_argument('-v', '--verbose', action='store_true', dest='verbose_build', help='Make CMake verbose')
parser.add_argument('--yamato', action='store_true', dest='yamato_build', help='Used internally for CI')
args = parser.parse_args()

curdir = os.path.dirname(os.path.abspath(__file__))
builddir = os.path.join(curdir, 'build')

# Clean the build?
if args.clean_build and os.path.exists(builddir):
    print("Deleting build directory..")
    shutil.rmtree(builddir)

if not os.path.exists(builddir):
    os.mkdir(builddir)

# Set the executable name
if sys.platform.startswith('win'):
    shell = True
    cmake_exe = 'cmake.exe'
else:
    # check if we should use cmake or cmake3
    cmake_version = subprocess.run("cmake --version", capture_output=True, text=True, shell=False, check=True, cwd=builddir)
    if cmake_version.startswith("cmake3"):
        # cmake is CMake2 on centos, need to be explicit.
        cmake_exe = 'cmake3'
    else:
        cmake_exe = 'cmake'
    shell = False

# Minimal configuration
config_args = [
    cmake_exe,
    '..', # because the working directory is the "build" directory, go back
    '-DCMAKE_SOURCE_DIR={}'.format(curdir),
    '-DCMAKE_BUILD_TYPE={}'.format(args.build_type), 
    '-DCMAKE_INSTALL_PREFIX={}'.format(os.path.join(builddir, 'install')),
    ]

# Where to find swig if not standard install
if args.swig_location is not None:
    config_args.append('-DSWIG_EXECUTABLE={}'.format(args.swig_location))
    # config_args.append(args.swig_location)
if args.fbxsdk_location is not None:
    config_args.append('-DFBXSDK_ROOT_PATH={}'.format(args.fbxsdk_location))

# Use Stevedore?
config_args.append('-DUSE_STEVEDORE' + ('=ON' if args.use_stevedore else '=OFF'))

# Is a CI build?
config_args.append('-DYAMATO' + ('=ON' if args.yamato_build else '=OFF'))

# Generator selection
config_args.append('-G')
if args.use_ninja:
    generator = 'Ninja'
else:
    if sys.platform.startswith('win'):
        generator = 'Visual Studio 15 2017 Win64'
    else:
        generator = 'Unix Makefiles'
config_args.append(generator)   

# Remove this if you're a build system dev
config_args.append('-Wno-dev')

# Do the config
print(' '.join(config_args))
retcode = subprocess.check_call(config_args, stderr=subprocess.STDOUT, shell=shell, cwd=builddir)

if retcode != 0:
    sys.exit(retcode)

# And do the build
build_args= [
    cmake_exe,
    '--build',
    '.',
    '--target',
    'install',
    '--config',
    args.build_type
]

if args.verbose_build and (args.use_ninja or sys.platform.startswith('win')):
    build_args.append('--verbose')

env = None
# Mac and Linux SWIG were compiled and have hard coded paths to swig.swg.
# Set the correct location in the environment for the build, since the
# configure is able to set it for itself.
if args.use_stevedore and not sys.platform.startswith('win'):

    env = os.environ
    def find(name, path):
        '''
        https://stackoverflow.com/a/1724723
        '''
        for root, dirs, files in os.walk(path):
            if name in files:
                # we need only the directory
                return root
    env["SWIG_LIB"] = find('swig.swg', builddir)


print(build_args)
retcode = subprocess.check_call(build_args, stderr=subprocess.STDOUT, shell=shell, cwd=builddir, env=env)

sys.exit(retcode)