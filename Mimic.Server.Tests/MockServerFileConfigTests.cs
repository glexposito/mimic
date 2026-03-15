using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace Mimic.Server.Tests;

public sealed class MockServerFileConfigTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MockServerFileConfigTests(WebApplicationFactory<Program> factory)
    {
        var testConfigPath = Path.Combine(AppContext.BaseDirectory, "TestConfigs", "hello.yaml");

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
    public async Task GetHello_ReturnsMessageBody()
    {
        var response = await _client.GetAsync("/hello");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        body.ShouldNotBeNull();
        body["message"].ShouldBe("hello from folder");
    }

    [Fact]
    public async Task GetUnknownPath_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/missing");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var body = await response.Content.ReadAsStringAsync();
        body.ShouldBe("No mock matched");
    }
}
