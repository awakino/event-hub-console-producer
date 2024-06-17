using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;

// The Event Hubs client types are safe to cache and use as a singleton for the lifetime
// of the application, which is best practice when events are being published or read regularly.
// TODO: Replace the <CONNECTION_STRING> and <HUB_NAME> placeholder values
EventHubProducerClient producerClient = new EventHubProducerClient(
    "<event hub connection string",
    "hub name");

Console.Write("> ");
var input = Console.ReadLine();
while (input?.ToLower() != "exit")
{
    try
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new Exception("Message data is null or whitespace");
        }

        // Create a batch of events 
        using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();
        if (!eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(input))))
        {
            throw new Exception($"Event is too large for the batch and cannot be sent.");
        }

        // Use the producer client to send the batch of events to the event hub
        await producerClient.SendAsync(eventBatch);
        Console.WriteLine($"A batch of events has been published.");

        Console.Write("> ");
        input = Console.ReadLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error publishing messages - {ex.Message}");
    }
}

Console.Write("exiting...");
await producerClient.DisposeAsync();