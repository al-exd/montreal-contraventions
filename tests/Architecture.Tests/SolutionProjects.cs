using System.Xml.Linq;

namespace Architecture.Tests;

public static class SolutionProjects
{
    private const string SolutionFile = "MontrealFoodViolations.sln";

    public static IReadOnlyList<ProjectInfo> Load()
    {
        var src = Path.Combine(FindRepositoryRoot(), "src");
        if (!Directory.Exists(src))
        {
            return [];
        }

        return Directory.EnumerateFiles(src, "*.csproj", SearchOption.AllDirectories)
            .Select(Parse)
            .ToList();
    }

    private static ProjectInfo Parse(string path)
    {
        var references = XDocument.Load(path)
            .Descendants("ProjectReference")
            .Select(r => Path.GetFileNameWithoutExtension(((string)r.Attribute("Include")!).Replace('\\', '/')))
            .ToList();

        return new ProjectInfo(Path.GetFileNameWithoutExtension(path), references);
    }

    private static string FindRepositoryRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, SolutionFile)))
            {
                return dir.FullName;
            }
        }

        throw new InvalidOperationException($"{SolutionFile} not found above {AppContext.BaseDirectory}");
    }
}
