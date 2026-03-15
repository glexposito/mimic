using Mimic.Core.Models;

namespace Mimic.Server.Config;

public sealed class FileMockApiConfigProvider : IMockApiConfigProvider
{
    private readonly IReadOnlyList<MockApiConfig> _configs;

    public FileMockApiConfigProvider(
        MockApiConfigLoader loader,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var configuredPath = configuration["Mimic:ConfigPath"] ?? "mocks.yaml";
        var fullPath = ResolvePath(configuredPath, environment);
        _configs = LoadConfigs(loader, fullPath);
    }

    public IReadOnlyList<MockApiConfig> GetCurrent() => _configs;

    private static IReadOnlyList<MockApiConfig> LoadConfigs(MockApiConfigLoader loader, string fullPath)
    {
        if (File.Exists(fullPath))
        {
            return [MockApiConfigLoader.Load(fullPath)];
        }

        if (Directory.Exists(fullPath))
        {
            var configFiles = Directory
                .EnumerateFiles(fullPath, "*.yml")
                .Concat(Directory.EnumerateFiles(fullPath, "*.yaml"))
                .OrderBy(path => path, StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (configFiles.Length == 0)
            {
                throw new InvalidOperationException($"No YAML config files were found in '{fullPath}'.");
            }

            return [.. configFiles.Select(MockApiConfigLoader.Load)];
        }

        throw new FileNotFoundException($"Mock config path '{fullPath}' was not found.", fullPath);
    }

    private static string ResolvePath(string configuredPath, IHostEnvironment environment)
    {
        if (Path.IsPathRooted(configuredPath))
        {
            return configuredPath;
        }

        return Path.Combine(environment.ContentRootPath, configuredPath);
    }
}
