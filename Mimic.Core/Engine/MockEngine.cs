using Mimic.Core.Models;
using Mimic.Core.Routing;

namespace Mimic.Core.Engine;

public class MockEngine(PathMatcher pathMatcher)
{
    public MockResponse? Handle(IEnumerable<MockApiConfig> configs, string method, string path)
    {
        var normalizedMethod = method.Trim().ToUpperInvariant();

        foreach (var config in configs)
        {
            foreach (var endpoint in config.Endpoints)
            {
                if (endpoint.Method != normalizedMethod)
                {
                    continue;
                }

                var match = pathMatcher.Match(endpoint.Path, path);
                if (!match.IsMatch)
                {
                    continue;
                }

                return endpoint.Respond;
            }
        }

        return null;
    }
}
