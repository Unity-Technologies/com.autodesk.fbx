using RecipeEngine.Api.Commands;
using RecipeEngine.Api.Dependencies;
using RecipeEngine.Api.Platforms;
using RecipeEngine.Api.Settings;
using RecipeEngine.Modules.Wrench.Models;
using RecipeEngine.Modules.Wrench.Platforms;
using RecipeEngine.Modules.Wrench.Settings;
using RecipeEngine.Platforms;

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

        // The native plugin is built on Ubuntu 22.04 and does not load on older Ubuntu images
        foreach (var editor in Wrench.Packages[FbxSdkPackageName].UnityEditors)
        {
            var platforms = editor.EditorPlatforms.Items
                .Where(p => p.Key != EditorPlatformType.Ubuntu1804 && p.Key != EditorPlatformType.Ubuntu2004)
                .ToDictionary(p => p.Key, p => p.Value);
            if (platforms.Count != editor.EditorPlatforms.Items.Count)
            {
                platforms[EditorPlatformType.Ubuntu2204] = new EditorPlatform(EditorPlatformType.Ubuntu2204);
                editor.EditorPlatforms = new EditorPlatformSet(platforms);
            }
        }

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
