using Microsoft.Extensions.Configuration;

namespace Data.Infrastructure
{
    public static class ConfigurationLoader
    {
        public static IConfiguration Load()
        {
            return new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
    }
}
