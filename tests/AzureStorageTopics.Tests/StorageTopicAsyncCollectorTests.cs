using Azure.Storage.Queues;
using NSubstitute;

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

        [Fact]
        public async Task AddAsync_should_enqueue_message_to_all_registered_subscriptions()
        {
            var attribute = new StorageTopicAttribute("mytopic");

            var subscriptions = new[]
            {
                Substitute.ForPartsOf<QueueClient>(),
                Substitute.ForPartsOf<QueueClient>(),
                Substitute.ForPartsOf<QueueClient>(),
                Substitute.ForPartsOf<QueueClient>()
            };
            foreach(var sub in subscriptions)
                sub.WhenForAnyArgs(s => s.SendMessageAsync(default, default))
                    .DoNotCallBase();

            var subscriptionsProvider = Substitute.For<ISubscriptionsProvider>();
            subscriptionsProvider.GetSubscriptionsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                                 .Returns(subscriptions);

            var connectionStringProvider = Substitute.For<IConnectionStringProvider>();
            connectionStringProvider.GetConnectionString(Arg.Any<string>()).Returns("connectionString");

            var sut = new StorageTopicAsyncCollector(attribute, subscriptionsProvider, connectionStringProvider);
            await sut.AddAsync("lorem ipsum");

            foreach (var sub in subscriptions)
                await sub.Received(1)
                         .SendMessageAsync("lorem ipsum", Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task AddAsync_should_not_fail_when_at_least_one_error_occurs()
        {
            var attribute = new StorageTopicAttribute("mytopic");

            var subscriptions = new[]
            {
                Substitute.ForPartsOf<QueueClient>(),
                Substitute.ForPartsOf<QueueClient>(),
                Substitute.ForPartsOf<QueueClient>(),
                Substitute.ForPartsOf<QueueClient>()
            };
            foreach (var sub in subscriptions)
                sub.WhenForAnyArgs(s => s.SendMessageAsync(default, default))
                    .DoNotCallBase();

            subscriptions[1].WhenForAnyArgs(s => s.SendMessageAsync(default, default))
                            .Throw(new Exception("error"));

            var subscriptionsProvider = Substitute.For<ISubscriptionsProvider>();
            subscriptionsProvider.GetSubscriptionsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                                 .Returns(subscriptions);

            var connectionStringProvider = Substitute.For<IConnectionStringProvider>();
            connectionStringProvider.GetConnectionString(Arg.Any<string>()).Returns("connectionString");

            var sut = new StorageTopicAsyncCollector(attribute, subscriptionsProvider, connectionStringProvider);
            await sut.AddAsync("lorem ipsum");

            foreach (var sub in subscriptions)
                await sub.Received(1)
                         .SendMessageAsync("lorem ipsum", Arg.Any<CancellationToken>());
        }
    }
}