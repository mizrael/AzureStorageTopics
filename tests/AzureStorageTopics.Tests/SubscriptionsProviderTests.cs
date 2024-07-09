using FluentAssertions;
using NSubstitute;

namespace AzureStorageTopics.Tests
{
    public class SubscriptionsProviderTests
    {
        [Fact]
        public async Task GetSubscriptionsAsync_should_fail_when_topic_name_null()
        {
            var factory = Substitute.For<ISubscriptionFactory>();
            var sut = new SubscriptionsProvider(new TopicsConfig(), factory);
            Func<Task> act = async () => await sut.GetSubscriptionsAsync(null, "connectionString");
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("'topicName' cannot be null or whitespace. (Parameter 'topicName')");
        }

        [Fact]
        public async Task GetSubscriptionsAsync_should_fail_when_connection_string_null()
        {
            var factory = Substitute.For<ISubscriptionFactory>();
            var sut = new SubscriptionsProvider(new TopicsConfig(), factory);
            Func<Task> act = async () => await sut.GetSubscriptionsAsync("topicName", null);
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("'connectionString' cannot be null or whitespace. (Parameter 'connectionString')");
        }

        [Fact]
        public async Task GetSubscriptionsAsync_should_fail_when_invalid_topic_name()
        {
            var factory = Substitute.For<ISubscriptionFactory>();
            var sut = new SubscriptionsProvider(new TopicsConfig(), factory);
            Func<Task> act = async () => await sut.GetSubscriptionsAsync("topicName", "connectionString");
            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("invalid topic name: topicName");
        }

        [Fact]
        public async Task GetSubscriptionsAsync_should_fail_when_no_subscriptions()
        {
            var config = new TopicsConfig();
            config.Topics["topicName"] = new TopicConfig();
            var factory = Substitute.For<ISubscriptionFactory>();
            var sut = new SubscriptionsProvider(config, factory);
            Func<Task> act = async () => await sut.GetSubscriptionsAsync("topicName", "connectionString");
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("no subscriptions found for topic: topicName");
        }
    }
}