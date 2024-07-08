using NSubstitute;
using AzureStorageTopics;
using Azure.Storage.Queues;
using NSubstitute.Extensions;

namespace AzureStorageTopics.Tests
{
    public class StorageTopicAsyncCollectorTests
    {
        [Fact]
        public async Task AddAsync_should_enqueue_message()
        {
            var attribute = new StorageTopicAttribute("mytopic");

            var sub = Substitute.ForPartsOf<QueueClient>();
            sub.WhenForAnyArgs(s => s.SendMessageAsync(default, default))
                .DoNotCallBase();
            var subscriptions = new[]
            {
                sub
            };

            var subscriptionsProvider = Substitute.For<ISubscriptionsProvider>();
            subscriptionsProvider.GetSubscriptionsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                                 .Returns(subscriptions);
            
            var connectionStringProvider = Substitute.For<IConnectionStringProvider>();
            connectionStringProvider.GetConnectionString(Arg.Any<string>()).Returns("connectionString");

            var sut = new StorageTopicAsyncCollector(attribute, subscriptionsProvider, connectionStringProvider);
            await sut.AddAsync("lorem ipsum");

            await sub.Received(1)
                .SendMessageAsync("lorem ipsum", Arg.Any<CancellationToken>());
        }
    }
}