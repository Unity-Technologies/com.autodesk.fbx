using FbxSdk.Cookbook.Recipes.Extensions;
using FbxSdk.Cookbook.Settings;
using CaseExtensions;
using RecipeEngine.Api;
using RecipeEngine.Api.Artifacts;
using RecipeEngine.Api.Dependencies;
using RecipeEngine.Api.Extensions;
using RecipeEngine.Api.Jobs;
using RecipeEngine.Api.Platforms;
using RecipeEngine.Api.Recipes;
using RecipeEngine.Platforms;

namespace FbxSdk.Cookbook.Recipes;

public class CodeSigning: RecipeBase
{
    string fbxSdkCodeSignListFileWindows = "windows_codesign_list.txt";

    string[] fbxSdkBinariesToSignOnMac =
    [
        "build-mac/install/com.autodesk.fbx/Editor/Plugins/UnityFbxSdkNative.bundle"
    ];
    string[] fbxSdkBinariesToSignOnWin =
    [
        "build-win/install/com.autodesk.fbx/Editor/Plugins/WinX64/UnityFbxSdkNative.dll",
        "build-win/install/com.autodesk.fbx/Editor/Plugins/WinARM64/UnityFbxSdkNative.dll"
    ];

    protected override ISet<Job> LoadJobs()
        => Combine.Collections(GetJobs()).SelectJobs();

    public string GetJobName(Platform platform, string packageName)
    {
        return $"Sign binaries for {packageName} on {platform.System.AsString()}";
    }

    public IEnumerable<Dependency> AsDependencies()
        => this.Jobs.ToDependencies(this);

    public IEnumerable<IJobBuilder> GetJobs()
    {
        var settings = FbxSdkSettings.Instance;
        var jobBuilders = new List<IJobBuilder>();
        var platforms = settings.Wrench.Packages[FbxSdkSettings.FbxSdkPackageName].EditorPlatforms;
        var packages = settings.Wrench.Packages.Where(p => p.Value.ReleaseOptions.IsReleasing);
        foreach (var package in packages)
        {
            var packageName = package.Value.ShortName;
            foreach (var platform in platforms)
            {
                if(platform.Key != SystemType.MacOS && platform.Key != SystemType.Windows)
                {
                    continue;
                }

                var signJob = CreateCodeSigningJob(platform.Value, packageName);
                if (signJob != null)
                {
                    jobBuilders.Add(signJob);
                }
            }
        }

        return jobBuilders;
    }

    IJobBuilder CreateCodeSigningJob(Platform platform, string packageName)
    {
        var packageShortName = packageName.Split(".").Last().ToPascalCase();
        return ProduceCodeSigningJob(platform, packageShortName);
    }

    IJobBuilder ProduceCodeSigningJob(Platform platform, string packageName)
    {
        IJobBuilder job = JobBuilder.Create(GetJobName(platform, packageName))
            .WithDescription(GetJobName(platform, packageName))
            .WithPlatform(platform);

        switch (platform.System)
        {
            case SystemType.MacOS:
                job.WithCodeSigningCommands(platform, string.Join(" ", fbxSdkBinariesToSignOnMac))
                    .WithDependencies(
                        new Dependency("BuildFbxSdkBindings", "build_plugins_-_macos-12")
                        )
                    .WithArtifact(new Artifact($"{packageName}_SignedBinariesOnMac", fbxSdkBinariesToSignOnMac));
                break;
            case SystemType.Windows:
                job.WithCodeSigningCommands(platform, fbxSdkCodeSignListFileWindows)
                    .WithDependencies(
                        new Dependency("BuildFbxSdkBindings", "build_plugins_-_win10"),
                        new Dependency("BuildFbxSdkBindings", "build_plugins_-_win11-arm64")
                        )
                    .WithArtifact(new Artifact($"{packageName}_SignedBinariesOnWindows", fbxSdkBinariesToSignOnWin));
                break;
            default:
                return null;
        }
        return job;
    }
}
