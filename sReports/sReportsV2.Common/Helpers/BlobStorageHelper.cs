using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace sReportsV2.Common.Helpers
{
    public static class BlobStorageHelper
    {
        public static List<string> GetUrls(string prefix) 
        {
            string connectionString = ConfigurationManager.AppSettings["AccountStorage"];
            BlobContainerClient blobContainerClient = new BlobContainerClient(connectionString, "sreportscontainer");
            List<BlobItem> blobs =  blobContainerClient.GetBlobs(prefix: prefix).ToList();
        
            List<string> listUrls = new List<string>();

            foreach (var item in blobs)
                listUrls.Add($"{blobContainerClient.Uri.AbsoluteUri}/{item.Name}");

            return listUrls;
        }

        public static string GetUrl(string prefix)
        {
            string connectionString = ConfigurationManager.AppSettings["AccountStorage"];
            BlobContainerClient blobContainerClient = new BlobContainerClient(connectionString, "sreportscontainer");
            List<BlobItem> blobs = blobContainerClient.GetBlobs(prefix: prefix).ToList();

            return $"{blobContainerClient.Uri.AbsoluteUri}/{blobs.FirstOrDefault().Name.Split('/')[0]}";
        }
    }
}