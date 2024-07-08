using AzureStorageTopics;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

[assembly: WebJobsStartup(typeof(AzureStorageTopicsStartup))]

namespace AzureStorageTopics
{
    public class AzureStorageTopicsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            var sp = builder.Services.BuildServiceProvider();
            var config = sp.GetService<IConfiguration>();

            builder.Services
                .Configure<TopicsConfig>(config.GetSection("AzureFunctionsJobHost:extensions:storageTopics"))
                .AddSingleton<IValidateOptions<TopicsConfig>, TopicsConfigValidator>()
                .AddSingleton<IConnectionStringProvider>(ctx =>
                {
                    var config = ctx.GetRequiredService<IConfiguration>();
                    return new ConnectionStringProvider(config);
                })
                .AddSingleton<ISubscriptionsProvider, SubscriptionsProvider>(ctx =>
                {
                    var topicsConfig = ctx.GetRequiredService<IOptions<TopicsConfig>>();
                    return new SubscriptionsProvider(topicsConfig.Value);
                });

            builder.AddExtension<StorageTopicConfigProvider>();
        }
    }
}