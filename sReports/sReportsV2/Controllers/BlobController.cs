using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class BlobController : BaseController
    {
        public BlobController()
        {
            
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file)
        {
            MemoryStream stream = new MemoryStream();
            file = Ensure.IsNotNull(file, nameof(file));
            file.InputStream.CopyTo(stream);
            CloudStorageAccount account = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["AccountStorage"]);
            CloudBlobClient serviceClient = account.CreateCloudBlobClient();
            CloudBlobContainer container = serviceClient.GetContainerReference("sreportscontainer");

            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference($"{Guid.NewGuid().ToString()}{file.FileName}");
            stream.Position = 0;//Move the pointer to the start of stream.

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = stream)
            {
                cloudBlockBlob.UploadFromStream(stream);
            }

            return Content(cloudBlockBlob.StorageUri.PrimaryUri.AbsoluteUri);
        }
    }
}