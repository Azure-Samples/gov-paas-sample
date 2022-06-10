using Azure.Storage.Queues;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficCaseApp.Models;

namespace TrafficCaseApp.Services
{
    public class ServiceClient : IQueueClient
    {
        private QueueServiceClient queueClient;
        private string queueName = CosmosInfo.QueueName;
        public ServiceClient(QueueServiceClient queueClient)
        {
            this.queueClient = queueClient;
        }

        public async Task InitializeQueue()
        {
            QueueClient queue = this.queueClient.GetQueueClient(queueName);
            await queue.CreateIfNotExistsAsync();
        }
        
        public async Task AddCaseToQueue(TrafficCase trafficCase)
        {
            QueueClient queue = this.queueClient.GetQueueClient(queueName);

            if (trafficCase.Status == "Closed")
            {
                await queue.SendMessageAsync(JsonConvert.SerializeObject(trafficCase));
            }
        }

        public async Task<List<TrafficCase>> GetClosedCases()
        {
            QueueClient queue = this.queueClient.GetQueueClient(queueName);
            var batch = await queue.ReceiveMessageAsync();
            List<TrafficCase> closedCaseList = new List<TrafficCase>();
            closedCaseList = batch.Value.MessageId.Select(msg => JsonConvert.DeserializeObject<TrafficCase>(msg.ToString())).ToList();
            return closedCaseList;
        }
    }
}
