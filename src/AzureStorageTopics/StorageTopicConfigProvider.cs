using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;

namespace AzureStorageTopics
{
    [Extension(Constants.StorageTopicsExtensionName)]
    internal sealed class StorageTopicConfigProvider : IExtensionConfigProvider
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly ISubscriptionsProvider _subscriptionsProvider;
        private readonly ILogger? _logger;

        public StorageTopicConfigProvider(
            ISubscriptionsProvider subscriptionsProvider,
            IConnectionStringProvider connectionStringProvider,
            ILogger? logger)
        {
            _subscriptionsProvider = subscriptionsProvider ?? throw new System.ArgumentNullException(nameof(subscriptionsProvider));
            _connectionStringProvider = connectionStringProvider ?? throw new System.ArgumentNullException(nameof(connectionStringProvider));
            _logger = logger;
        }

        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<StorageTopicAttribute>()
                   .BindToCollector(attribute => new StorageTopicAsyncCollector(attribute, _subscriptionsProvider, _connectionStringProvider, _logger));
        }
    }
}