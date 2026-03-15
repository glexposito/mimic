namespace Mimic.Core.Models;

public sealed class PathMatchResult
{
    public bool IsMatch { get; init; }
    public Dictionary<string, string> Parameters { get; init; } = new();
}