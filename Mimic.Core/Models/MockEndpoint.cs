namespace Mimic.Core.Models;

public sealed class MockEndpoint
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public MockResponse Respond { get; set; } = new();
}