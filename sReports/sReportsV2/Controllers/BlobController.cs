using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Exceptions;
using sReportsV2.Common.Extensions;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class BlobController : BaseController
    {
        private readonly IBlobStorageBLL blobStorageBLL;
        public BlobController(IBlobStorageBLL blobStorageBLL)
        {
            this.blobStorageBLL = blobStorageBLL;
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file)
        {
            return Content(blobStorageBLL.Create(file));
        }

        [SReportsAuthorize]
        public ActionResult Download(string resourceId)
        {
            return File(blobStorageBLL.Download(resourceId), MimeMapping.GetMimeMapping(resourceId));
        }

        [HttpPost]
        public ActionResult UploadLogo(HttpPostedFileBase file)
        {
            Ensure.IsNotNull(file, nameof(file));
            CheckLogoSize(file.ContentLength);
            return Content(blobStorageBLL.Create(file));
        }

        private void CheckLogoSize(int fileLength)
        {
            if (!IsLessThanLimit(fileLength))
            {
                throw new UserAdministrationException(System.Net.HttpStatusCode.Conflict, $"Your logo size is limited to {GetLogoLimitFromConfiguration()}MB. Please upload the appropriate logo size.");
            }
        }

        private bool IsLessThanLimit(int fileLength)
        {
            return fileLength < GetLogoLimit();
        }

        private double GetLogoLimit()
        {
            return GetLogoLimitFromConfiguration() * Math.Pow(10, 6);
        }

        private double GetLogoLimitFromConfiguration()
        {
            double logoLimitInMB = double.TryParse(ConfigurationManager.AppSettings["LogoSizeLimit"], out double logoSizeLimit) ? logoSizeLimit : 3;
            return logoLimitInMB;
        }
    }
}