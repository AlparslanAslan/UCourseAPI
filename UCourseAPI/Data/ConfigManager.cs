using Microsoft.Extensions.Configuration;

namespace WebApi.Data
{
    public static class ConfigManager
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string GetConnectionString()
        {
            return _configuration.GetConnectionString("Test");
        }
    }
}
