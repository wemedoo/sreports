using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class DigitalGuidelineController : BaseController
    {
        private readonly IDigitalGuidelineBLL digitalGuidelineBLL;

        public DigitalGuidelineController(IDigitalGuidelineBLL digitalGuidelineBLL)
        {
            this.digitalGuidelineBLL = digitalGuidelineBLL;
        }

        [SReportsAuditLog]
        [SReportsAuthorize]
        public ActionResult GetAll(GuidelineFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;

            return View();
        }

        [SReportsAuthorize]
        public ActionResult ReloadTable(GuidelineFilterDataIn dataIn)
        {
            return PartialView("DigitalGuidelineTable", digitalGuidelineBLL.GetAll(dataIn));
        }

        [SReportsAuthorize]
        public async Task<ActionResult> Edit(string id)
        {
            return View("Index", await digitalGuidelineBLL.GetById(id).ConfigureAwait(false));
        }

        [HttpPost]
        public async Task<ActionResult> Create([ModelBinder(typeof(JsonNetModelBinder))]GuidelineDataIn dataIn) 
        {
            ResourceCreatedDTO resourceCreatedDTO =  await digitalGuidelineBLL.InsertOrUpdate(dataIn).ConfigureAwait(false);
            return new JsonResult()
            {
                Data = resourceCreatedDTO,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult PreviewNode(GuidelineElementDataDataIn dataIn)
        {            
            return PartialView(digitalGuidelineBLL.PreviewNode(dataIn));
        }

        [SReportsAuthorize]
        public ActionResult Create()
        {
            return View("Index");
        }

        [SReportsAuthorize]
        [SReportsAuditLog]
        [System.Web.Http.HttpDelete]
        public ActionResult Delete(string id, DateTime lastUpdate)
        {
            digitalGuidelineBLL.Delete(id, lastUpdate);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}