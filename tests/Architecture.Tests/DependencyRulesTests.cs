namespace Architecture.Tests;

public class DependencyRulesTests
{
    private static ProjectInfo Project(string name, params string[] references) => new(name, references);

    [Fact]
    public void Check_DomainReferencingApplication_ReportsViolation()
    {
        var projects = new[] { Project("Registry.Domain", "Registry.Application"), Project("Registry.Application") };

        var violations = DependencyRules.Check(projects);

        Assert.Single(violations);
    }

    [Fact]
    public void Check_DomainReferencingSharedKernel_IsAllowed()
    {
        var projects = new[] { Project("Registry.Domain", "SharedKernel"), Project("SharedKernel") };

        Assert.Empty(DependencyRules.Check(projects));
    }

    [Fact]
    public void Check_InfrastructureReferencingOwnApplicationAndDomain_IsAllowed()
    {
        var projects = new[]
        {
            Project("Registry.Infrastructure", "Registry.Application", "Registry.Domain"),
            Project("Registry.Application", "Registry.Domain"),
            Project("Registry.Domain"),
        };

        Assert.Empty(DependencyRules.Check(projects));
    }

    [Fact]
    public void Check_ApplicationReferencingAnotherModulesDomain_ReportsViolation()
    {
        var projects = new[] { Project("Registry.Application", "Ingestion.Domain"), Project("Ingestion.Domain") };

        var violations = DependencyRules.Check(projects);

        Assert.Single(violations);
    }

    [Fact]
    public void Check_ApplicationReferencingAnotherModulesContracts_IsAllowed()
    {
        var projects = new[] { Project("Registry.Application", "Ingestion.Contracts"), Project("Ingestion.Contracts") };

        Assert.Empty(DependencyRules.Check(projects));
    }

    [Fact]
    public void Check_ContractsReferencingDomain_ReportsViolation()
    {
        var projects = new[] { Project("Registry.Contracts", "Registry.Domain"), Project("Registry.Domain") };

        var violations = DependencyRules.Check(projects);

        Assert.Single(violations);
    }

    [Fact]
    public void Check_SharedKernelReferencingAModule_ReportsViolation()
    {
        var projects = new[] { Project("SharedKernel", "Registry.Domain"), Project("Registry.Domain") };

        var violations = DependencyRules.Check(projects);

        Assert.Single(violations);
    }

    [Fact]
    public void Check_ModuleReferencingTheHost_ReportsViolation()
    {
        var projects = new[] { Project("Registry.Infrastructure", "MontrealFoodViolations.Api"), Project("MontrealFoodViolations.Api") };

        var violations = DependencyRules.Check(projects);

        Assert.Single(violations);
    }

    [Fact]
    public void Check_HostReferencingModulesAndSharedKernel_IsAllowed()
    {
        var projects = new[]
        {
            Project("MontrealFoodViolations.Api", "Registry.Infrastructure", "Ingestion.Application", "SharedKernel"),
            Project("Registry.Infrastructure"),
            Project("Ingestion.Application"),
            Project("SharedKernel"),
        };

        Assert.Empty(DependencyRules.Check(projects));
    }
}
