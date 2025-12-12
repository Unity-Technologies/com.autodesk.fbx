using RecipeEngine.Api.Settings;
using RecipeEngine.Modules.Wrench.Models;
using RecipeEngine.Modules.Wrench.Settings;

namespace FbxSdk.Cookbook.Settings;

public class FbxSdkSettings : AnnotatedSettingsBase
{
    // Path from the root of the repository where packages are located.
    readonly string[] PackagesRootPaths = {"./com.autodesk.fbx"};

    // update this to list all packages in this repo that you want to release.
    Dictionary<string, PackageOptions> PackageOptions = new()
    {
        {
            "com.autodesk.fbx",
            new PackageOptions() { ReleaseOptions = new ReleaseOptions() { IsReleasing = true } }
        }
    };

    public FbxSdkSettings()
    {
        Wrench = new WrenchSettings(
            PackagesRootPaths,
            PackageOptions,
            wrenchCsProjectPath: "Tools/CI/FbxSdk.Cookbook.csproj"
        );      
    }

    public WrenchSettings Wrench { get; private set; }
}
