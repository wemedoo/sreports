using sReportsV2.BusinessLayer.Interfaces;
using System;
using System.IO;
using System.Web;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class FileStorageBLL : IBlobStorageBLL
    {
        private readonly string uploadFolderName;

        public FileStorageBLL()
        {
            this.uploadFolderName = AppDomain.CurrentDomain.BaseDirectory + $"\\UploadedFiles\\";
        }

        public string Create(HttpPostedFileBase file)
        {
            string resourceUrl = string.Empty;

            if (file != null && file.ContentLength > 0)
            {
                CreateDirectoryIfNotExist();
                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                file.SaveAs(uploadFolderName + fileName);

                string domain = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                resourceUrl = $"{domain}/Blob/Download/{fileName}";
            }

            return resourceUrl;
        }

        public Stream Download(string fileName)
        {
            try
            {
                CreateDirectoryIfNotExist();
                byte[] readedFile = File.ReadAllBytes(uploadFolderName + fileName);
                Stream stream = new MemoryStream(readedFile);
                return stream;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void CreateDirectoryIfNotExist()
        {
            if (!Directory.Exists(uploadFolderName))
            {
                Directory.CreateDirectory(uploadFolderName);
            }
        }
    }
}