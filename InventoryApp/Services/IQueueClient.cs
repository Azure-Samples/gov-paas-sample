using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficCaseApp.Models;

namespace TrafficCaseApp.Services
{
    public interface IQueueClient
    {
        Task AddCaseToQueue(TrafficCase trafficCase);
        Task<List<TrafficCase>> GetClosedCases();
        Task InitializeQueue();
    }
}
