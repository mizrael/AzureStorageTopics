using Microsoft.Extensions.Configuration;

namespace AzureStorageTopics
{
    internal sealed class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly IConfiguration _funcAppConfig;

        public ConnectionStringProvider(IConfiguration funcAppConfig)
        {
            _funcAppConfig = funcAppConfig;
        }

        public string GetConnectionString(string connectionSettingName)
        {
            if (string.IsNullOrWhiteSpace(connectionSettingName))
            {
                throw new System.ArgumentException($"'{nameof(connectionSettingName)}' cannot be null or whitespace.", nameof(connectionSettingName));
            }

            return _funcAppConfig[connectionSettingName];
        }
    }
}