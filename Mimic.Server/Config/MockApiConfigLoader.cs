using Mimic.Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Mimic.Server.Config;

public sealed class MockApiConfigLoader
{
    public MockApiConfig Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Mock configuration file was not found.", path);
        }

        var yaml = File.ReadAllText(path);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        var config = deserializer.Deserialize<MockApiConfig>(yaml) ?? new MockApiConfig();
        Normalize(config);
        Validate(config, path);
        return config;
    }

    private static void Normalize(MockApiConfig config)
    {
        config.Name = config.Name.Trim();

        foreach (var endpoint in config.Endpoints)
        {
            endpoint.Method = endpoint.Method.Trim().ToUpperInvariant();
            endpoint.Path = NormalizePath(endpoint.Path);
            endpoint.Respond.Headers = NormalizeHeaders(endpoint.Respond.Headers);
        }
    }

    private static void Validate(MockApiConfig config, string path)
    {
        if (string.IsNullOrWhiteSpace(config.Name))
        {
            throw new InvalidOperationException($"Mock configuration '{path}' must define a name.");
        }

        for (var i = 0; i < config.Endpoints.Count; i++)
        {
            var endpoint = config.Endpoints[i];

            if (string.IsNullOrWhiteSpace(endpoint.Method))
            {
                throw new InvalidOperationException($"Endpoint at index {i} in '{path}' must define a method.");
            }

            if (string.IsNullOrWhiteSpace(endpoint.Path))
            {
                throw new InvalidOperationException($"Endpoint at index {i} in '{path}' must define a path.");
            }
        }
    }

    private static string NormalizePath(string path)
    {
        var trimmedPath = path.Trim();
        if (string.IsNullOrEmpty(trimmedPath))
        {
            return "/";
        }

        trimmedPath = trimmedPath.Trim('/');
        return trimmedPath.Length == 0 ? "/" : $"/{trimmedPath}";
    }

    private static Dictionary<string, string> NormalizeHeaders(Dictionary<string, string> headers)
    {
        var normalizedHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var header in headers)
        {
            normalizedHeaders[header.Key.Trim()] = header.Value.Trim();
        }

        return normalizedHeaders;
    }
}
