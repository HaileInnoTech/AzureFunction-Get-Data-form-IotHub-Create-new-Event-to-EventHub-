using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public class Function1
    {
        string eventHubName = "myeventhub";
        string namespaceConnectionString = "Endpoint=sb://iotdata.servicebus.windows.net/;SharedAccessKeyName=sendandreceive;SharedAccessKey=5N1yF9EO8NYcuW/ZGqZUN1iOwJX+eUotO+AEhFgAvKk=;EntityPath=myeventhub";

        [FunctionName("Function1")]
        public async Task Run(
            [IoTHubTrigger("messages/events", Connection = "connectionString")] string message,
            ILogger log)
        {
            EventHubProducerClient producerClient = new EventHubProducerClient(namespaceConnectionString, eventHubName);

            // Create a batch of events 
            using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

            // Add a new event to the batch
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            string eventDataBody = Encoding.UTF8.GetString(messageBytes);
            log.LogInformation($"Event Data: {eventDataBody}");
            eventBatch.TryAdd(new EventData(messageBytes));

            // Send the batch of events
            await producerClient.SendAsync(eventBatch);

            log.LogInformation($"C# IoT Hub trigger function processed a message: {message}");
        }
    }
}
