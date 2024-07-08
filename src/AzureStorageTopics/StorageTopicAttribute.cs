using Microsoft.Azure.WebJobs.Description;
using System;

namespace AzureStorageTopics
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class StorageTopicAttribute : Attribute
    {
        public StorageTopicAttribute(string topicName)
        {
            if (string.IsNullOrWhiteSpace(topicName))
                throw new ArgumentException($"'{nameof(topicName)}' cannot be null or whitespace.", nameof(topicName));
            
            TopicName = topicName;
        }

        /// <summary>
        /// name of the Topic to use.
        /// </summary>
        public string TopicName { get; }

        /// <summary>
        /// Gets or sets the app setting name that contains the Azure Storage connection string.
        /// </summary>
        public string? ConnectionSettingName { get; set; }
    }
}