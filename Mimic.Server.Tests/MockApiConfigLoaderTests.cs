using Mimic.Server.Config;
using Shouldly;
using Xunit;

namespace Mimic.Server.Tests;

public sealed class MockApiConfigLoaderTests
{
    [Fact]
    public void Load_ThrowsWhenEndpointMethodIsNotSupported()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.yaml");

        File.WriteAllText(
            tempPath,
            """
            name: invalid-api

            endpoints:
              - method: FETCH
                path: /users/1
                respond:
                  status: 200
            """);

        try
        {
            var exception = Should.Throw<MimicConfigValidationException>(() => MockApiConfigLoader.Load(tempPath));
            exception.Message.ShouldContain(tempPath);
            exception.Message.ShouldContain("endpoint[0].method");
            exception.Message.ShouldContain("FETCH");
            exception.Message.ShouldContain("unsupported value");
            exception.Message.ShouldContain("Allowed values: DELETE, GET, HEAD, OPTIONS, PATCH, POST, PUT.");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Load_ThrowsWhenEndpointPathDoesNotStartWithSlash()
    {
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.yaml");

        File.WriteAllText(
            tempPath,
            """
            name: invalid-api

            endpoints:
              - method: GET
                path: users/1
                respond:
                  status: 200
            """);

        try
        {
            var exception = Should.Throw<MimicConfigValidationException>(() => MockApiConfigLoader.Load(tempPath));
            exception.Message.ShouldContain(tempPath);
            exception.Message.ShouldContain("endpoint[0].path");
            exception.Message.ShouldContain("users/1");
            exception.Message.ShouldContain("Paths must start with '/'.");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}
