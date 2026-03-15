namespace Mimic.Core.Models;

public sealed class MockResponse
{
    public int Status { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public object? Body { get; set; }
}