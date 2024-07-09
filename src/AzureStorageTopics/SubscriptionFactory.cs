using Azure.Storage.Queues;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageTopics
{
    internal sealed class SubscriptionFactory : ISubscriptionFactory
    {
        private static readonly ConcurrentDictionary<string, Task<QueueClient>> _cache = new ConcurrentDictionary<string, Task<QueueClient>>();
       
        public async ValueTask<QueueClient> CreateAsync(
            string topicName,
            string subscriptionName,
            string connectionString,
            bool useCache = true,
            CancellationToken cancellationToken = default)
        {
            string queueName = BuildQueueName(topicName, subscriptionName);

            if (useCache)
            {
                var clientFactory = _cache.GetOrAdd(queueName, _ => CreateClient(connectionString, queueName, cancellationToken));
                var client = await clientFactory.ConfigureAwait(false);
                return client;
            }

            var queueClient = await CreateClient(connectionString, queueName, cancellationToken).ConfigureAwait(false);

            return queueClient;
        }

        private static async Task<QueueClient> CreateClient(string connectionString, string queueName, CancellationToken cancellationToken)
        {
            var queueClient = new QueueClient(
                connectionString,
                queueName);
            await queueClient.CreateIfNotExistsAsync(cancellationToken: cancellationToken)
                             .ConfigureAwait(false);
            return queueClient;
        }

        private static string BuildQueueName(string topicName, string subscriptionName)
        {
            return $"{topicName}-{subscriptionName}".ToLower();
        }
    }
}