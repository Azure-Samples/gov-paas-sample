using Azure.Storage.Queues;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficCaseApp.Models;

namespace TrafficCaseApp.Services
{
    public class queueClient : IQueueClient
    {
        private QueueServiceClient queueclient;
        private string queueName = CosmosInfo.QueueName;
        public queueClient(QueueServiceClient queueClient)
        {
            this.queueclient = queueClient;
        }

        public async Task InitializeQueue()
        {
            QueueClient queue = this.queueclient.GetQueueClient(queueName);
            await queue.CreateIfNotExistsAsync();
        }
        
        public async Task AddCaseToQueue(TrafficCase trafficCase)
        {
            QueueClient queue = this.queueclient.GetQueueClient(queueName);

            if (trafficCase.Status == "Closed")
            {

                await queue.SendMessageAsync(JsonConvert.SerializeObject(trafficCase));

            }
        }

        public async Task<List<TrafficCase>> GetClosedCases()
        {
            QueueClient queue = this.queueclient.GetQueueClient(queueName);
            var batch = await queue.ReceiveMessageAsync();
            List<TrafficCase> closedCaseList = new List<TrafficCase>();
            closedCaseList = batch.Value.MessageId.Select(msg => JsonConvert.DeserializeObject<TrafficCase>(msg.ToString())).ToList();
            return closedCaseList;
        }
    }
}
