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
    cmake_exe = 'cmake'
    shell = False

# Minimal configuration
config_args = [
    cmake_exe,
    '..', # because the working directory is the "build" directory, go back
    '-DCMAKE_SOURCE_DIR={}'.format(curdir),
    '-DCMAKE_BUILD_TYPE={}'.format(args.build_type), 
    '-DCMAKE_INSTALL_PREFIX={}'.format(os.path.join(builddir, 'install')),
    '-DCMAKE_OSX_ARCHITECTURES=arm64',
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

if retcode != 0:
    sys.exit(retcode)

if sys.platform.startswith('darwin'):
    # On Mac build two binaries (one that works on arm and one x86_64 that works on 10.13+).
    # The arm binary is already built, here we build the second one and combine the two with lipo
    
    # use a different build directory
    builddir_legacy = os.path.join(curdir, 'build_legacy_mac')
    
    if args.clean_build and os.path.exists(builddir_legacy):
        shutil.rmtree(builddir_legacy)

    if not os.path.exists(builddir_legacy):
        os.mkdir(builddir_legacy)
    
    install_prefix = '-DCMAKE_INSTALL_PREFIX={}'.format(os.path.join(builddir_legacy, 'install'))
    
    # use all the same config args except the install prefix
    config_args = [a for a in config_args if not (a.startswith("-DCMAKE_INSTALL_PREFIX") or a.startswith("-DCMAKE_OSX_ARCHITECTURES"))] 
    config_args.append(install_prefix)
    config_args.append('-DCMAKE_OSX_ARCHITECTURES=x86_64')
    retcode = subprocess.check_call(config_args, stderr=subprocess.STDOUT, shell=shell, cwd=builddir_legacy)

    if retcode != 0:
        sys.exit(retcode)

    retcode = subprocess.check_call(build_args, stderr=subprocess.STDOUT, shell=shell, cwd=builddir_legacy, env=env)
    if retcode != 0:
        sys.exit(retcode)

    # combine the arm build and the legacy build with lipo
    bundle_path = "install/com.autodesk.fbx/Editor/Plugins/UnityFbxSdkNative.bundle/Contents/MacOS/UnityFbxSdkNative"
    bundle_name = "UnityFbxSdkNative"
    arm_bundle = os.path.join(builddir, bundle_path)
    legacy_bundle = os.path.join(builddir_legacy, bundle_path)
    lipo_call = ["lipo", "-create", "-output", bundle_name, arm_bundle, legacy_bundle]
    retcode = subprocess.check_call(lipo_call, stderr=subprocess.STDOUT, shell=shell, cwd=curdir, env=env)
    if retcode != 0:
        sys.exit(retcode)
    
    # replace the arm bundle with the universal binary
    src = os.path.join(curdir, bundle_name)
    dst = arm_bundle
    shutil.copyfile(src, dst)

sys.exit(retcode)