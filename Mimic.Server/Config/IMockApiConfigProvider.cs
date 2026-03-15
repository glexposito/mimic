using Mimic.Core.Models;

namespace Mimic.Server.Config;

public interface IMockApiConfigProvider
{
    IReadOnlyList<MockApiConfig> GetCurrent();
}
