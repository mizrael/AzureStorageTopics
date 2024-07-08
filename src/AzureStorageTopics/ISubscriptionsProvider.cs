using Azure.Storage.Queues;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageTopics
{
    public interface ISubscriptionsProvider
    {
        ValueTask<IEnumerable<QueueClient>> GetSubscriptionsAsync(string topicName, string connectionString, CancellationToken cancellationToken = default);
    }
}