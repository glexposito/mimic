using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Mimic.Server.Tests;

public sealed class MockServerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MockServerTests(WebApplicationFactory<Program> factory)
    {
        var testConfigPath = Path.Combine(AppContext.BaseDirectory, "TestConfigs");

        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Mimic:ConfigPath"] = testConfigPath
                });
            });
        }).CreateClient();
    }

    [Fact]
    public async Task GetHello_ReturnsConfiguredJsonResponse()
    {
        var response = await _client.GetAsync("/hello");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(body);
        Assert.Equal("hello from folder", body["message"]);
    }

    [Fact]
    public async Task GetStatus_ReturnsResponseFromAnotherYamlFile()
    {
        var response = await _client.GetAsync("/status");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        Assert.NotNull(body);
        Assert.Equal("ok", body["state"]);
    }

    [Fact]
    public async Task GetUnknownPath_Returns404()
    {
        var response = await _client.GetAsync("/missing");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal("No mock matched", body);
    }
}
