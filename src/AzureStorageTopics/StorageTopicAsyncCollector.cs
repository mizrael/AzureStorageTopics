using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AzureStorageTopics
{
    internal sealed class StorageTopicAsyncCollector : IAsyncCollector<string>
    {
        private readonly ISubscriptionsProvider _subscriptionsProvider;
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly ILogger? _logger;
        private readonly StorageTopicAttribute _topicAttribute;

        public StorageTopicAsyncCollector(
            StorageTopicAttribute topicAttribute,
            ISubscriptionsProvider subscriptionsProvider,
            IConnectionStringProvider connectionStringProvider,
            ILogger? logger = null)
        {
            _topicAttribute = topicAttribute ?? throw new System.ArgumentNullException(nameof(topicAttribute));
            _subscriptionsProvider = subscriptionsProvider ?? throw new System.ArgumentNullException(nameof(subscriptionsProvider));
            _connectionStringProvider = connectionStringProvider ?? throw new System.ArgumentNullException(nameof(connectionStringProvider));
            _logger = logger;
        }

        public async Task AddAsync(string message, CancellationToken cancellationToken = default)
        {
            var connectionStringSettingName = _topicAttribute.ConnectionSettingName ?? Constants.DefaultConnectionStringSettingsName;
            var connectionString = _connectionStringProvider.GetConnectionString(connectionStringSettingName);
           
            var subscriptions = await _subscriptionsProvider.GetSubscriptionsAsync(
                _topicAttribute.TopicName,
                connectionString, 
                cancellationToken).ConfigureAwait(false);

            foreach (var subscription in subscriptions)
            {
                try
                {
                    await subscription.SendMessageAsync(message, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, $"Error while sending message to subscription '{subscription.Name}'.");
                }
            }
        }

        public Task FlushAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}