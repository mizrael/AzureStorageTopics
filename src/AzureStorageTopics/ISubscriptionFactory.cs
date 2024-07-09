using Azure.Storage.Queues;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageTopics
{
    public interface ISubscriptionFactory
    {
        ValueTask<QueueClient> CreateAsync(string topicName, string subscriptionName, string connectionString, bool useCache = true, CancellationToken cancellationToken = default);
    }
}