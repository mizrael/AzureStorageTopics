namespace AzureStorageTopics
{
    public interface IConnectionStringProvider
    {
        string GetConnectionString(string connectionSettingName);
    }
}