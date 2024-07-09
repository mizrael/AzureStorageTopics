using FluentAssertions;

namespace AzureStorageTopics.Tests
{
    public class TopicsConfigValidatorTests
    {
        [Fact]
        public void Validate_should_fail_when_input_null()
        {
            var sut = new TopicsConfigValidator();
            var result = sut.Validate(null, null);
            result.Failed.Should().BeTrue();
            result.FailureMessage.Should().Be("The configuration is null.");
        }

        [Fact]
        public void Validate_should_fail_when_no_topics_provided()
        {
            var config = new TopicsConfig();
            var sut = new TopicsConfigValidator();
            var result = sut.Validate(null, config);
            result.Failed.Should().BeTrue();
            result.FailureMessage.Should().Be("The configuration does not contain any topics.");
        }

        [Fact]
        public void Validate_should_fail_when_no_subscriptions_provided()
        {
            var config = new TopicsConfig();
            config.Topics["topic1"] = new TopicConfig();

            var sut = new TopicsConfigValidator();
            var result = sut.Validate(null, config);
            result.Failed.Should().BeTrue();
            result.FailureMessage.Should().Be("The topic 'topic1' does not contain any subscriptions.");
        }

        [Fact]
        public void Validate_should_fail_when_subscriptions_duplicated()
        {
            var config = new TopicsConfig();
            config.Topics["topic1"] = new TopicConfig()
            {
                Subscriptions = new[]
                {
                    new SubscriptionConfig() { Name = "sub1" },
                    new SubscriptionConfig() { Name = "sub1" }
                }
            };

            var sut = new TopicsConfigValidator();
            var result = sut.Validate(null, config);
            result.Failed.Should().BeTrue();
            result.FailureMessage.Should().Be("The topic 'topic1' contains a duplicate subscription 'sub1'.");
        }

        [Fact]
        public void Validate_should_succeed_when_input_valid()
        {
            var config = new TopicsConfig();
            config.Topics["topic1"] = new TopicConfig()
            {
                Subscriptions = new[]
                {
                    new SubscriptionConfig() { Name = "sub1" },
                    new SubscriptionConfig() { Name = "sub2" }
                }
            };

            var sut = new TopicsConfigValidator();
            var result = sut.Validate(null, config);
            result.Failed.Should().BeFalse();
        }
    }
}