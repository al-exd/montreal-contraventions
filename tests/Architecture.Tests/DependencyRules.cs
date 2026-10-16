namespace Architecture.Tests;

/// <summary>
/// Project-reference rules from docs/05-architecture.md. Rules work on csproj references, not compiled
/// metadata, because the compiler drops references that are never used.
/// </summary>
public static class DependencyRules
{
    public static IReadOnlyList<string> Check(IReadOnlyCollection<ProjectInfo> projects)
    {
        var byName = projects.ToDictionary(p => p.Name);
        var violations = new List<string>();

        foreach (var project in projects)
        {
            foreach (var reference in project.References)
            {
                var target = byName.GetValueOrDefault(reference) ?? new ProjectInfo(reference, []);
                if (!IsAllowed(project, target))
                {
                    violations.Add($"{project.Name} must not reference {reference}");
                }
            }
        }

        return violations;
    }

    private static bool IsAllowed(ProjectInfo from, ProjectInfo to)
    {
        if (from.IsHost)
        {
            return !to.IsHost;
        }

        if (to.IsHost)
        {
            return false;
        }

        if (from.IsSharedKernel)
        {
            return false;
        }

        if (to.IsSharedKernel)
        {
            return true;
        }

        var sameModule = from.Module == to.Module;
        if (!sameModule)
        {
            return from.Layer is "Application" or "Infrastructure" && to.Layer == "Contracts";
        }

        return from.Layer switch
        {
            "Application" => to.Layer is "Domain" or "Contracts",
            "Infrastructure" => to.Layer is "Domain" or "Application" or "Contracts",
            _ => false,
        };
    }
}
