using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.Azure.Documents.Client;
using System;
using TrafficCaseApp.Models;

namespace TrafficCaseApp.Services
{
    public class ServiceFactory
    {
        private TCConfig config;

        public ServiceFactory(TCConfig config)
        {
            this.config = config;
        }
        public BlobServiceClient CreateCloudStorageAccount() => 
            new BlobServiceClient(new Uri("endpoint"),
                new StorageSharedKeyCredential(config.StorageConfig.AccountName, config.StorageConfig.AccountKey), null);

        public DocumentClient CreateDocumentClient() => new DocumentClient(new Uri(this.config.CosmosConfig.Uri), this.config.CosmosConfig.Key);

    }
}
