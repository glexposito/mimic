using Mimic.Core.Models;

namespace Mimic.Core.Routing;

public class PathMatcher
{
    public PathMatchResult Match(string template, string actualPath)
    {
        var templateSegments = SplitPath(template);
        var actualSegments = SplitPath(actualPath);

        if (templateSegments.Length != actualSegments.Length)
        {
            return new PathMatchResult { IsMatch = false };
        }

        var parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < templateSegments.Length; i++)
        {
            var templateSegment = templateSegments[i];
            var actualSegment = actualSegments[i];

            if (IsParameter(templateSegment))
            {
                var parameterName = templateSegment[1..^1];
                parameters[parameterName] = actualSegment;
                continue;
            }

            if (!string.Equals(templateSegment, actualSegment, StringComparison.OrdinalIgnoreCase))
            {
                return new PathMatchResult { IsMatch = false };
            }
        }

        return new PathMatchResult
        {
            IsMatch = true,
            Parameters = parameters
        };
    }

    private static string[] SplitPath(string path)
    {
        return path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
    }

    private static bool IsParameter(string segment)
    {
        return segment.StartsWith('{') && segment.EndsWith('}') && segment.Length > 2;
    }
}
