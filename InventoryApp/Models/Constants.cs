using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrafficCaseApp.Models
{
    public static class CosmosInfo
    {
        public static string DbName = "trafficDb";
        public static string CasesCollection = "traffic-cases";
        public static string StatusCollection = "statuses";
        public static string QueueName = "closedcases";
    }
}
