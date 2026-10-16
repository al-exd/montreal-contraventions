namespace Architecture.Tests;

public class SolutionArchitectureTests
{
    private static readonly string[] Modules = ["Registry", "Ingestion"];
    private static readonly string[] Layers = ["Domain", "Application", "Infrastructure", "Contracts"];

    private static readonly IReadOnlyList<ProjectInfo> Projects = SolutionProjects.Load();

    [Fact]
    public void Solution_Always_ContainsSharedKernelAndHost()
    {
        var names = Projects.Select(p => p.Name).ToHashSet();

        Assert.Contains("SharedKernel", names);
        Assert.Contains("MontrealFoodViolations.Api", names);
    }

    [Theory]
    [MemberData(nameof(ModuleLayers))]
    public void Solution_Always_ContainsEveryLayerOfEveryModule(string module, string layer)
    {
        Assert.Contains(Projects, p => p.Name == $"{module}.{layer}");
    }

    [Fact]
    public void Solution_Always_RespectsDependencyRules()
    {
        var violations = DependencyRules.Check(Projects);

        Assert.True(violations.Count == 0, string.Join(Environment.NewLine, violations));
    }

    public static TheoryData<string, string> ModuleLayers()
    {
        var data = new TheoryData<string, string>();
        foreach (var module in Modules)
        {
            foreach (var layer in Layers)
            {
                data.Add(module, layer);
            }
        }

        return data;
    }
}
