using Mimic.Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Mimic.Server.Config;

public sealed class MockApiConfigLoader
{
    private const string ValidMethodsDisplay = "DELETE, GET, HEAD, OPTIONS, PATCH, POST, PUT";

    private static readonly HashSet<string> ValidMethods =
    [
        "DELETE",
        "GET",
        "HEAD",
        "OPTIONS",
        "PATCH",
        "POST",
        "PUT"
    ];

    public static MockApiConfig Load(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Mock configuration file was not found.", path);
        }

        var yaml = File.ReadAllText(path);
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithAttemptingUnquotedStringTypeDeserialization()
            .IgnoreUnmatchedProperties()
            .Build();

        var config = deserializer.Deserialize<MockApiConfig>(yaml);
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
            endpoint.Path = endpoint.Path.Trim();
            endpoint.Respond.Headers = NormalizeHeaders(endpoint.Respond.Headers);
        }
    }

    private static void Validate(MockApiConfig config, string path)
    {
        if (string.IsNullOrWhiteSpace(config.Name))
        {
            throw new MimicConfigValidationException(
                $"Mock configuration '{path}' is invalid: 'name' is required.");
        }

        for (var i = 0; i < config.Endpoints.Count; i++)
        {
            var endpoint = config.Endpoints[i];

            if (string.IsNullOrWhiteSpace(endpoint.Method))
            {
                throw new MimicConfigValidationException(
                    $"Mock configuration '{path}' is invalid: endpoint[{i}].method is required.");
            }

            if (!ValidMethods.Contains(endpoint.Method))
            {
                throw new MimicConfigValidationException(
                    $"Mock configuration '{path}' is invalid: endpoint[{i}].method has unsupported value '{endpoint.Method}'. Allowed values: {ValidMethodsDisplay}.");
            }

            if (string.IsNullOrWhiteSpace(endpoint.Path))
            {
                throw new MimicConfigValidationException(
                    $"Mock configuration '{path}' is invalid: endpoint[{i}].path is required.");
            }

            if (!endpoint.Path.StartsWith('/'))
            {
                throw new MimicConfigValidationException(
                    $"Mock configuration '{path}' is invalid: endpoint[{i}].path has invalid value '{endpoint.Path}'. Paths must start with '/'.");
            }
        }
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
