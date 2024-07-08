using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace AzureStorageTopics
{
    internal class TopicsConfigValidator : IValidateOptions<TopicsConfig>
    {
        public ValidateOptionsResult Validate(string? name, TopicsConfig options)
        {
            if(options == null)
            {
                return ValidateOptionsResult.Fail("The configuration is null.");
            }

            if(options.Topics == null || options.Topics.Count == 0)
            {
                return ValidateOptionsResult.Fail("The configuration does not contain any topics.");
            }

            foreach (var (key, topic) in options.Topics)
            {
                if (topic.Subscriptions is null || topic.Subscriptions.Length == 0)
                {
                    return ValidateOptionsResult.Fail($"The topic '{key}' does not contain any subscriptions.");
                }

                var topicsDict = new HashSet<string>();
                foreach (var subscription in topic.Subscriptions)
                {
                    if (topicsDict.Contains(subscription.Name))
                    {
                        return ValidateOptionsResult.Fail($"The topic '{key}' contains a duplicate subscription '{subscription}'.");
                    }

                    topicsDict.Add(subscription.Name);
                }
            }

            return ValidateOptionsResult.Success;
        }
    }
}