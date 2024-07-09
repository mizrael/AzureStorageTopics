# Azure Storage Topics
[![Tests](https://github.com/mizrael/AzureStorageTopics/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mizrael/AzureStorageTopics/actions/workflows/dotnet.yml)
[![Nuget](https://img.shields.io/nuget/v/AzureStorageTopics?style=plastic)](https://www.nuget.org/packages/AzureStorageTopics/)

This project aims to provide a **very simple** implementation of Topics over Azure Storage Queues, similar to [Topics](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions#topics-and-subscriptions) in Azure Service Bus.

### Why?
Simply because being able to forward the same message (more or less) to multiple queues in one go is very convenient. Single queues are useful when dealing with _Commands_, which would expect to be pulled and processed by a single Consumer. Moreover, the Producer is aware and is expecting that there's a Consumer waiting on the other side. But when we need to broadcast an _Event_ instead, the Producer doesn't know how many different Consumers there will be. Nor cares. The Producer's only goal is to inform the world that _something happened_.

### How does it work?

The underlying idea is  Messages can be sent to a virtual "topic", which in turn would forward it to the registered "subscriptions".

Simply add the [Nuget package](https://www.nuget.org/packages/AzureStorageTopics/) to your Azure Functions project, then add the Output binding: 

```csharp
[FunctionName(nameof(SendToTopic))]
public static async Task<IActionResult> SendToTopic(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    [StorageTopic("MyTopic")] IAsyncCollector<string> outputTopic)
{
    await outputTopic.AddAsync("hello world");

    return new OkObjectResult("wow");
}
```

Subscriptions can be configured directly in the `host.json` file:
```json
{
  "extensions": {
    "storageTopics": {
      "topics": {
        "MyTopic": {
          "subscriptions": [
            {
              "name": "Subscription1"
            },
            {
              "name": "Subscription2"
            }
          ]
        }
      }
    }
  }
}
```

The library will automatically create the queues if not existing, using this naming convention: `topic-subscription` , lowercase:

<img width="500" alt="image" src="https://github.com/mizrael/AzureStorageTopics/assets/1432872/2853a03d-d893-4248-b2c5-1ed9eeaa1272">


The setting containing the connection string to the Storage Account can be specified by providing the `ConnectionSettingName` property to the `StorageTopic` attribute:
```csharp
[FunctionName(nameof(SendToTopic))]
public static async Task<IActionResult> SendToTopic(
    [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
    [StorageTopic("MyTopic", ConnectionSettingName="")] IAsyncCollector<string> outputTopic)
```
The default value is `AzureWebJobsStorage`.

### TODO
- more tests
- support for Isolated mode

