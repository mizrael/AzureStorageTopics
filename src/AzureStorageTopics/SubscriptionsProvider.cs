using Azure.Storage.Queues;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageTopics
{
    internal sealed class SubscriptionsProvider : ISubscriptionsProvider
    {
        private readonly TopicsConfig _topicsConfig;
        private readonly ISubscriptionFactory _subscriptionFactory;

        public SubscriptionsProvider(TopicsConfig topicsConfig, ISubscriptionFactory subscriptionFactory)
        {
            _topicsConfig = topicsConfig ?? throw new System.ArgumentNullException(nameof(topicsConfig));
            _subscriptionFactory = subscriptionFactory ?? throw new System.ArgumentNullException(nameof(subscriptionFactory));
        }

        public async ValueTask<IEnumerable<QueueClient>> GetSubscriptionsAsync(
            string topicName,
            string connectionString,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(topicName))
            {
                throw new System.ArgumentException($"'{nameof(topicName)}' cannot be null or whitespace.", nameof(topicName));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new System.ArgumentException($"'{nameof(connectionString)}' cannot be null or whitespace.", nameof(connectionString));
            }

            if (_topicsConfig.Topics == null || !_topicsConfig.Topics.TryGetValue(topicName, out var config) || config == null)
            {
                throw new KeyNotFoundException($"invalid topic name: {topicName}");
            }

            if (config.Subscriptions is null || config.Subscriptions.Length == 0)
            {
                throw new System.InvalidOperationException($"no subscriptions found for topic: {topicName}");
            }

            var queues = new List<QueueClient>(config.Subscriptions.Length);
            foreach(var subscription in config.Subscriptions)
            {
                var queueClient = await _subscriptionFactory.CreateAsync(
                    topicName, 
                    subscription.Name, 
                    connectionString, 
                    useCache: _topicsConfig.CacheClient,
                    cancellationToken: cancellationToken).ConfigureAwait(false);
                queues.Add(queueClient);
            }

            return queues;
        }
    }
}