using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using TrafficCaseApp.Models;
using Microsoft.Azure.Documents;
using System.Net;
using Newtonsoft.Json;

namespace TrafficCaseApp.Services
{
    public class TrafficCaseRepository : ITrafficCaseRepository
    {
        private ICacheClient cacheClient;
        private DocumentClient docClient;

        public TrafficCaseRepository(DocumentClient docClient, ICacheClient cacheClient)
        {
            this.docClient = docClient;
            this.cacheClient = cacheClient;
        }

        public async Task Initialize()
        {
            await this.docClient.CreateDatabaseIfNotExistsAsync(new Database { Id = CosmosInfo.DbName });
            await this.docClient.CreateDocumentCollectionIfNotExistsAsync(GetDatabaseUri(), new DocumentCollection { Id = CosmosInfo.CasesCollection });
            var response = await this.docClient.CreateDocumentCollectionIfNotExistsAsync(GetDatabaseUri(), new DocumentCollection { Id = CosmosInfo.StatusCollection });
            if (response.StatusCode == HttpStatusCode.Created)
            {
                // we need to seed data
                var statusList = new List<Status> {
                    new Status { Id = "Filed", Name = "Filed" },
                    new Status { Id = "Pending", Name = "Pending" },
                    new Status { Id = "Dropped", Name = "Dropped" },
                    new Status { Id = "Closed", Name = "Closed" },
                };
                foreach (var item in statusList)
                {
                    await this.docClient.CreateDocumentAsync(GetDocCollectionUri(CosmosInfo.StatusCollection), item);
                }
            }
        }

        public List<TrafficCase> GetCases()
        {
            var collectionUri = GetDocCollectionUri(CosmosInfo.CasesCollection);
            IQueryable<TrafficCase> trafficQuery = this.docClient.CreateDocumentQuery<TrafficCase>(collectionUri);
            return trafficQuery.ToList();
        }

        public async Task<List<Status>> GetStatuses()
        {
            const string cacheKey = "statuses";
            var statuses = await this.cacheClient.GetStatus(cacheKey);
            if (statuses == null)
            {
                var statusList = this.docClient.CreateDocumentQuery<Status>(GetDocCollectionUri(CosmosInfo.StatusCollection)).ToList();
                statuses = JsonConvert.SerializeObject(statusList);
                await this.cacheClient.WriteStatus(cacheKey, statuses);
            }
            return JsonConvert.DeserializeObject<List<Status>>(statuses);
        }

        public async Task<String> CreateCase(TrafficCase trafficCase)
        {
            await this.docClient.CreateDocumentAsync(GetDocCollectionUri(CosmosInfo.CasesCollection), trafficCase);
            return ("Successfully created Case");
        }

        public async Task EditCase(TrafficCase trafficCase)
        {
            var doc = this.docClient.CreateDocumentQuery<Status>(GetDocCollectionUri(CosmosInfo.CasesCollection)).Where(d => d.Id == trafficCase.Id.ToString()).AsEnumerable().SingleOrDefault();
            await this.docClient.ReplaceDocumentAsync(GetDocumentUri(CosmosInfo.CasesCollection, doc.Id), trafficCase);
        }
        
        public async Task DeleteCase(string id)
        {
            await this.docClient.DeleteDocumentAsync(GetDocumentUri(CosmosInfo.CasesCollection, id));
        }

        public async Task<TrafficCase> GetCase(string id)
        {
            var doc = await this.docClient.ReadDocumentAsync(GetDocumentUri(CosmosInfo.CasesCollection, id));
            return (TrafficCase)(dynamic)doc.Resource;
        }

        #region Private Methods

        private static Uri GetDocCollectionUri(string collectionName)
        {
            return UriFactory.CreateDocumentCollectionUri(CosmosInfo.DbName, collectionName);
        }

        private static Uri GetDocumentUri(string collectionName, string docId)
        {
            return UriFactory.CreateDocumentUri(CosmosInfo.DbName, collectionName, docId);
        }

        private static Uri GetDatabaseUri()
        {
            return UriFactory.CreateDatabaseUri(CosmosInfo.DbName);
        }

        #endregion
    }
}