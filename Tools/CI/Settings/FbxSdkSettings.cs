using RecipeEngine.Api.Commands;
using RecipeEngine.Api.Dependencies;
using RecipeEngine.Api.Settings;
using RecipeEngine.Modules.Wrench.Models;
using RecipeEngine.Modules.Wrench.Settings;

namespace FbxSdk.Cookbook.Settings;

public class FbxSdkSettings : AnnotatedSettingsBase
{
    // Path from the root of the repository where packages are located.
    readonly string[] PackagesRootPaths = {"./com.autodesk.fbx"};
    
    public static readonly string FbxSdkPackageName = "com.autodesk.fbx";
    
    //A singleton instance of FBXSdkSettings
    static FbxSdkSettings _instance;

    // update this to list all packages in this repo that you want to release.
    Dictionary<string, PackageOptions> PackageOptions = new()
    {
        {
            FbxSdkPackageName,
            new PackageOptions()
            {
                ReleaseOptions = new ReleaseOptions() { IsReleasing = true },
                PackJobOptions = new PackJobOptions()
                {
                    PrePackCommands = 
                    [
                        new Command("cp -vrfp build-ubuntu/install/* ."),
                        new Command("cp -vrfp build-mac/install/* ."),
                        new Command("cp -vrfp build-win/install/* ."),
                    ],
                    Dependencies = 
                    [
                        new Dependency("BuildFbxSdkBindings", "build_plugins_-_ubuntu-22_04"),
                        // On Mac and Windows, code signing on binaries is required before packing
                        // Windows x64 signing job can sign both x64 and arm64 binaries because the signing tool are not architecture specific
                        new Dependency("CodeSigning", "sign_binaries_for_fbx_on_macos"),
                        new Dependency("CodeSigning", "sign_binaries_for_fbx_on_windows"),
                    ]
                }
            }
        }
    };

    public FbxSdkSettings()
    {
        Wrench = new WrenchSettings(
            PackagesRootPaths,
            PackageOptions,
            wrenchCsProjectPath: "Tools/CI/FbxSdk.Cookbook.csproj"
        );
        
        Wrench.BranchNamingPattern = BranchPatterns.ReleaseSlash;
        // Add "rme", "supported" to upm-pvp check profiles together with an exemption file
        // PVP-41-1 check is skipped because of "Unreleased" entry in CHANGELOG.md
        Wrench.PvpProfilesToCheck = new HashSet<string>() { "rme", "supported", "-PVP-41-1", ".yamato/wrench/pvp-exemptions.json" };
    }
    
    public static FbxSdkSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new FbxSdkSettings();
            }
            return _instance;
        }
    }

    public WrenchSettings Wrench { get; private set; }
}
