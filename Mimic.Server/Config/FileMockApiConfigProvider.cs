using Mimic.Core.Models;

namespace Mimic.Server.Config;

public sealed class FileMockApiConfigProvider(
    MockApiConfigLoader loader,
    IConfiguration configuration,
    IHostEnvironment environment)
    : IMockApiConfigProvider
{
    public IReadOnlyList<MockApiConfig> GetCurrent()
    {
        var configuredPath = configuration["Mimic:ConfigPath"] ?? "mocks.yaml";
        var fullPath = ResolvePath(configuredPath);

        if (File.Exists(fullPath))
        {
            return [loader.Load(fullPath)];
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

            return configFiles.Select(loader.Load).ToArray();
        }

        throw new FileNotFoundException($"Mock config path '{fullPath}' was not found.", fullPath);
    }

    private string ResolvePath(string configuredPath)
    {
        if (Path.IsPathRooted(configuredPath))
        {
            return configuredPath;
        }

        return Path.Combine(environment.ContentRootPath, configuredPath);
    }
}
