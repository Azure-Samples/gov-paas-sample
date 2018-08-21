using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficCaseApp.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace TrafficCaseApp.Services
{
    public class CacheClient : ICacheClient
    {
        private IDistributedCache cache;
        private TCConfig config;
        public List<string> statusKeys = new List<string>(new string[] { "new", "pending", "dropped", "closed" });

        public CacheClient(IDistributedCache cache, TCConfig config)
        {
            this.cache = cache;
            this.config = config;
        }

        public async Task<string> GetStatus(string key)
        {
            return await this.cache.GetStringAsync(key);
        }

        public async Task WriteStatus(string key, string val)
        {
            await this.cache.SetStringAsync(key, val);
        }
    }
}
