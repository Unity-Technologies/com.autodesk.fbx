using Bee.NativeProgramSupport.Building.FluentSyntaxHelpers;
using Unity.BuildSystem.CSharpSupport;
using Unity.BuildSystem.NativeProgramSupport;

class Build
{
    static void Main()
    {
        //Uncomment calls below to play around with how to write build code. The programs the example build code will try to build do not exist on disk,
        //so you'd have to make them.

        //SetupCSharpHelloWorld();
        //SetupCppHelloWorld();
    }

    private static void SetupCppHelloWorld()
    {
        //setup a platform agnostic native program description
        var np = new NativeProgram("helloworld");

        //add all native sourcefiles in a directory called helloworld_cpp
        np.Sources.Add("helloworld_cpp");

        //maybe add a define?
        np.Defines.Add("VERY_IMPORTANT=1");

        //now that we have a nativeprogram, we need to build in into a specific "shape". We need to specify an exact toolchain, architecture,
        //"format", codegen and target directory.  Let's start:

        //let's pick whatever is a good toolchain for the computer we're running on
        var toolchain = ToolChain.Store.Host();

        //Lets create a configuration for this nativeprogram that builds with Debug Codegen, the toolchain we just created, and uses lumping.
        var nativeProgramConfiguration = new NativeProgramConfiguration(CodeGen.Debug, toolchain, lump: true);

        //we want to build the nativeprogram into an executable. each toolchain can provide different ways of "linking" (static, dynamic, executable are default ones)
        var format = toolchain.ExecutableFormat;

        //and here the magic happens, the nativeprogram gets setup for the specific configuration we just made.
        np.SetupSpecificConfiguration(nativeProgramConfiguration, format).DeployTo("build");
    }

    private static void SetupCSharpHelloWorld()
    {
        // Declare a CSharpProgram (a managed assembly - executable or dll)
        var program = new CSharpProgram
        {
            //build into here please:
            Path = "build/helloworld_csharp.exe",

            //with these sources (they'll be globbed if they're directories")
            Sources = {"helloworld_csharp"}
        };

        //and set it up to build in the default configuration (debug, with an auto-selected compiler)
        program.SetupDefault();

        //that's it, building csharp is too easy.
    }
}
