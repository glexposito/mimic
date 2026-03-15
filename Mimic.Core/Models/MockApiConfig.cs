namespace Mimic.Core.Models;

public sealed class MockApiConfig
{
    public string Name { get; set; } = string.Empty;
    public List<MockEndpoint> Endpoints { get; set; } = [];
}