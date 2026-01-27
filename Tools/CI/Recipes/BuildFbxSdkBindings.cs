using FbxSdk.Cookbook.Settings;
using RecipeEngine.Api.Dependencies;
using RecipeEngine.Api.Extensions;
using RecipeEngine.Api.Jobs;
using RecipeEngine.Api.Platforms;
using RecipeEngine.Api.Recipes;
using RecipeEngine.Platforms;

namespace FBXSdk.Cookbook.Recipes;

public class BuildFbxSdkBindings: RecipeBase
{
    protected override ISet<Job> LoadJobs()
        => Combine.Collections(GetJobs()).SelectJobs();

    public string GetJobName(Agent agent)
        => $"Build plugins - {agent.Image.Split(new[] { '/', ':' })[1]}";

    public IEnumerable<Dependency> AsDependencies()
        => this.Jobs.ToDependencies(this);

    public IEnumerable<IJobBuilder> GetJobs()
    {
        var settings = FbxSdkSettings.Instance;

        List<Agent> buildAgents = new();

        //Build agents
        Agent winArm64 = new Agent("package-ci/win11-arm64:default", FlavorType.BuildLarge, ResourceType.Azure, "arm");
        Agent win = new Agent("package-ci/win10:default", FlavorType.BuildLarge, ResourceType.Vm);
        Agent ubuntu = new Agent("package-ci/ubuntu-22.04:default", FlavorType.BuildLarge, ResourceType.Vm);
        Agent mac = new Agent("package-ci/macos-12:default", FlavorType.MacDefault, ResourceType.VmOsx);

        buildAgents.Add(winArm64);
        buildAgents.Add(win);
        buildAgents.Add(ubuntu);
        buildAgents.Add(mac);

        List<IJobBuilder> builders = new();
        foreach (var agent in buildAgents)
        {
            var builder = JobBuilder.Create(GetJobName(agent))
                .WithPlatform(new Platform(agent, SystemType.Unknown))
                .WithDescription(GetJobName(agent));

            if (agent.Image.Contains("win"))
            {
                builder.WithCommands(c => c
                    .Add("build_win.cmd"))
                    .WithArtifact("build", "build-win/install/**");

            }
            else if (agent.Image.Contains("mac"))
            {
                builder.WithCommands(c => c
                        .Add("./build_mac.sh"))
                    .WithArtifact("build", "build-mac/install/**");
            }
            else
            {
                builder.WithCommands(c => c
                        .Add("./build_linux.sh"))
                    .WithArtifact("build", "build-ubuntu/install/**");
            }
            builders.Add(builder);
        }

        return builders;
    }

}