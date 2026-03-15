using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Shouldly;
using Xunit;

namespace Mimic.Server.Tests;

public sealed class MockServerFolderConfigTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MockServerFolderConfigTests(WebApplicationFactory<Program> factory)
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
    public async Task GetHello_ReturnsMessageBody()
    {
        var response = await _client.GetAsync("/hello");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        body.ShouldNotBeNull();
        body["message"].ShouldBe("hello from folder");
    }

    [Fact]
    public async Task GetUserById_ReturnsUserWithAddresses()
    {
        var response = await _client.GetAsync("/users/1");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var user = body.GetProperty("user");
        var addresses = user.GetProperty("addresses");

        user.GetProperty("id").GetInt32().ShouldBe(1);
        user.GetProperty("profile").GetProperty("name").GetString().ShouldBe("Alice");
        user.GetProperty("profile").GetProperty("active").GetBoolean().ShouldBeTrue();
        addresses.GetArrayLength().ShouldBe(2);
        addresses[0].GetProperty("type").GetString().ShouldBe("home");
        addresses[0].GetProperty("city").GetString().ShouldBe("Auckland");
        addresses[1].GetProperty("type").GetString().ShouldBe("work");
        addresses[1].GetProperty("city").GetString().ShouldBe("Wellington");
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
