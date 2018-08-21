using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrafficCaseApp.Services
{
    public interface ICacheClient
    {
        Task<string> GetStatus(string key);
        Task WriteStatus(string key, string val);

    }
}
