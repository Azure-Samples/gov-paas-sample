using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficCaseApp.Models;

namespace TrafficCaseApp.Services
{
    public class QueueClient : IQueueClient
    {
        private CloudQueueClient queueClient;
        private string queueName = CosmosInfo.QueueName;
        public QueueClient(CloudQueueClient queueClient)
        {
            this.queueClient = queueClient;
        }

        public async Task InitializeQueue()
        {
            CloudQueue queue = this.queueClient.GetQueueReference(queueName);
            await queue.CreateIfNotExistsAsync();
        }
        
        public async Task AddCaseToQueue(TrafficCase trafficCase)
        {
            CloudQueue queue = this.queueClient.GetQueueReference(queueName);

            if (trafficCase.Status == "Closed")
            {
                var queueMsg = new CloudQueueMessage(JsonConvert.SerializeObject(trafficCase));
                await queue.AddMessageAsync(queueMsg);
            }
        }

        public async Task<List<TrafficCase>> GetClosedCases()
        {
            CloudQueue queue = this.queueClient.GetQueueReference(queueName);
            var batch = await queue.GetMessagesAsync(3);
            List<TrafficCase> closedCaseList = new List<TrafficCase>();
            closedCaseList = batch.Select(msg => JsonConvert.DeserializeObject<TrafficCase>(msg.AsString)).ToList();
            return closedCaseList;
        }
    }
}
