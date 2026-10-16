namespace Architecture.Tests;

public sealed record ProjectInfo(string Name, IReadOnlyList<string> References)
{
    public const string SharedKernelName = "SharedKernel";
    public const string HostName = "MontrealFoodViolations.Api";

    public bool IsSharedKernel => Name == SharedKernelName;

    public bool IsHost => Name == HostName;

    public string? Module => Segments is [var module, _] ? module : null;

    public string? Layer => Segments is [_, var layer] ? layer : null;

    private string[] Segments => Name.Split('.');
}
