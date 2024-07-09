
using System.Collections.Generic;

namespace AzureStorageTopics
{
    internal sealed class TopicsConfig
    {
        public TopicsConfig()
        {
            Topics = new Dictionary<string, TopicConfig>();
            CacheClient = true;
        }

        public Dictionary<string, TopicConfig> Topics { get; }
        
        public bool CacheClient { get; set; }
    }
}