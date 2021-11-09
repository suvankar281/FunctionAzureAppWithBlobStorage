using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using System.Collections.Generic;

namespace FunctionAzureAppWithBlobStorage
{
    public class FunctionAppBlobStorageDemo
    {
        [FunctionName("TriggerBlob")]
        public void Run([BlobTrigger("filecontainer/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            BlobServiceClient blobServiceClient = new(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            BlobContainerClient containerSourceClient = blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("SourceContainerName"));
            BlobContainerClient containerDestinationClient = blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("DestinationContainerName"));

            var blobItems = containerSourceClient.GetBlobs();
           foreach (var blob in blobItems)
            {
                BlobClient sourceBlob = containerSourceClient.GetBlobClient(blob.Name);
                BlobClient destBlob = containerDestinationClient.GetBlobClient(blob.Name);
                destBlob.StartCopyFromUri(sourceBlob.Uri);
                sourceBlob.Delete();
            }         
        }
    }
}
