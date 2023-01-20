using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using System;
using System.IO;
using System.Web;
using System.Web.Configuration;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class CloudStorageBLL : IBlobStorageBLL
    {
        public string Create(HttpPostedFileBase file)
        {
            MemoryStream stream = new MemoryStream();
            file = Ensure.IsNotNull(file, nameof(file));
            file.InputStream.CopyTo(stream);
            CloudBlobContainer container = GetCloudBlobContainer();

            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference($"{Guid.NewGuid()}_{file.FileName}");
            stream.Position = 0;//Move the pointer to the start of stream.

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = stream)
            {
                cloudBlockBlob.UploadFromStream(stream);
            }

            return cloudBlockBlob.StorageUri.PrimaryUri.AbsoluteUri;
        }

        public Stream Download(string resourceId)
        {
            if (!string.IsNullOrWhiteSpace(resourceId))
            {
                CloudBlobContainer container = GetCloudBlobContainer();
                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(resourceId);
                Stream blobStream = cloudBlockBlob.OpenRead();
                return blobStream;
            }
            else
                return null;    
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(WebConfigurationManager.AppSettings["AccountStorage"]);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            return serviceClient.GetContainerReference("sreportscontainer");
        }
    }
}