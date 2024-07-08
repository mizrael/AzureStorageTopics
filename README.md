# Azure Storage Topics

This project aims to provide a **very simple** implementation of Topics over Azure Storage Queues.
The underlying idea is similar to [Topics](https://learn.microsoft.com/en-us/azure/service-bus-messaging/service-bus-queues-topics-subscriptions#topics-and-subscriptions) in Azure Service Bus. Messages can be sent to a virtual "topic", which in turn would forward it to the registered subscriptions:

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

### TODO
- more tests
- publish to Nuget
