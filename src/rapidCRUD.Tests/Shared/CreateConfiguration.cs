using Microsoft.Extensions.Configuration;

namespace rapidCRUD.Tests.Shared;

public static class SharedMethods
{
    public static IConfiguration CreateConfiguration(Dictionary<string, string> configValues)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();
    }
}
