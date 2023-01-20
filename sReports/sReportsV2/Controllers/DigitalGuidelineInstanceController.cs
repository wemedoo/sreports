using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataIn;
using sReportsV2.DTOs.DigitalGuidelineInstance.DataOut;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Patient;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class DigitalGuidelineInstanceController : BaseController
    {
        private readonly IDigitalGuidelineInstanceBLL digitalGuidelineInstanceBLL;
        private readonly IDigitalGuidelineBLL digitalGuidelineBLL;
        private readonly IFormInstanceBLL formInstanceBLL;

        public DigitalGuidelineInstanceController(IFormInstanceBLL formInstanceBLL, IDigitalGuidelineInstanceBLL digitalGuidelineInstanceBLL, IDigitalGuidelineBLL digitalGuidelineBLL)
        {
            this.formInstanceBLL = formInstanceBLL;
            this.digitalGuidelineInstanceBLL = digitalGuidelineInstanceBLL;
            this.digitalGuidelineBLL = digitalGuidelineBLL;
        }

        [SReportsAuthorize]
        public ActionResult GuidelineInstance(int episodeOfCareId)
        {
            PatientDataOut data = digitalGuidelineInstanceBLL.GetGuidelineInstance(episodeOfCareId);
            ViewBag.EocId = episodeOfCareId;
            return View("GuidelineInstance", data);
        }

        [SReportsAuthorize]
        public ActionResult LoadGraph(string guidelineInstanceId, string guidelineId)
        {
            ViewBag.GuidelineInstanceId = guidelineInstanceId;
            GuidelineDataOut data = digitalGuidelineInstanceBLL.GetGraph(guidelineInstanceId, guidelineId);
            return PartialView("DigitalGuidelineInstanceGraph", data);
        }

        [SReportsAuthorize]
        public ActionResult GuidelineInstanceTable(int episodeOfCareId)
        {
            List<GuidelineInstanceDataOut> data = digitalGuidelineInstanceBLL.GetGuidelineInstancesByEOC(episodeOfCareId);
            return PartialView(data);
        }

        [SReportsAuthorize]
        public ActionResult Create()
        {
            return View();
        }

        [SReportsAuthorize]
        [SReportsAuditLog]
        [HttpPost]
        public ActionResult Create(GuidelineInstanceDataIn guidelineInstance)
        {
            digitalGuidelineInstanceBLL.InsertOrUpdate(guidelineInstance);
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [SReportsAuthorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult Delete(string guidelineInstanceId)
        {
            digitalGuidelineInstanceBLL.Delete(guidelineInstanceId);

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult ListDigitalGuidelines(int? episodeOfCareId)
        {
            if (episodeOfCareId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Please choose episode of care!");
            }

            GuidelineInstanceViewDataOut data = digitalGuidelineInstanceBLL.ListDigitalGuidelines(episodeOfCareId);
            return PartialView(data);
        }

        public ActionResult FilterDigitalGuidelines(string title)
        {
            List<GuidelineDataOut> data = digitalGuidelineBLL.SearchByTitle(title);
            return PartialView(data);
        }

        public ActionResult ListGuidelineDocuments(int episodeOfCareId)
        {
            GuidelineInstanceViewDataOut data = digitalGuidelineInstanceBLL.ListDigitalGuidelineDocuments(episodeOfCareId, userCookieData.ActiveOrganization);
            return PartialView(data);
        }

        public ActionResult FilterGuidelineDocuments(int episodeOfCareId, string title)
        {
            List<FormInstanceDataOut> data = formInstanceBLL.SearchByTitle(episodeOfCareId, title);
            return PartialView(data);
        }

        [HttpPost]
        public ActionResult PreviewInstanceNode(GuidelineElementDataDataIn dataIn, string guidelineInstanceId, string guidelineId)
        {
            GuidelineElementDataDataOut data = digitalGuidelineInstanceBLL.PreviewInstanceNode(dataIn);
            ViewBag.NodeGuidelineInstanceId = guidelineInstanceId;
            ViewBag.NodeGuidelineId = guidelineId;
            return PartialView(data);
        }

        [HttpPost]
        public ActionResult PreviewInstanceDecisionNode(GuidelineElementDataDataIn dataIn, string guidelineInstanceId, string guidelineId)
        {
            GuidelineElementDataDataOut data = digitalGuidelineInstanceBLL.PreviewInstanceNode(dataIn);
            ViewBag.NodeGuidelineInstanceId = guidelineInstanceId;
            ViewBag.NodeGuidelineId = guidelineId;
            return PartialView(data);
        }

        [SReportsAuthorize]
        public string GetValueFromDocument(string formInstanceId, int thesaurusId)
        {
            return digitalGuidelineInstanceBLL.GetValueFromDocument(formInstanceId, thesaurusId);
        }

        [SReportsAuthorize]
        public ActionResult MarksAsCompleted(string value, string nodeId, string guidelineInstanceId, string guidelineId)
        {
            digitalGuidelineInstanceBLL.MarksAsCompleted(value, nodeId, guidelineInstanceId);
            ViewBag.GuidelineInstanceId = guidelineInstanceId;

            GuidelineDataOut data = digitalGuidelineInstanceBLL.GetGraph(guidelineInstanceId, guidelineId);
            return PartialView("DigitalGuidelineInstanceGraph", data);
        }

        [SReportsAuthorize]
        public ActionResult GetConditions(string nodeId, string digitalGuidelineId, string guidelineInstanceId)
        {
            ViewBag.Conditions = digitalGuidelineInstanceBLL.GetConditions(nodeId, digitalGuidelineId);
            ViewBag.GuidelineId = digitalGuidelineId;
            ViewBag.GuidelineInstanceId = guidelineInstanceId;
            ViewBag.NodeId = nodeId;
            return PartialView();
        }

        [SReportsAuthorize]
        public ActionResult SaveCondition(string condition, string nodeId, string guidelineInstanceId, string digitalGuidelineId)
        {
            digitalGuidelineInstanceBLL.SaveCondition(condition, nodeId, guidelineInstanceId, digitalGuidelineId);
            ViewBag.GuidelineInstanceId = guidelineInstanceId;

            GuidelineDataOut data = digitalGuidelineInstanceBLL.GetGraph(guidelineInstanceId, digitalGuidelineId);
            return PartialView("DigitalGuidelineInstanceGraph", data);
        }
    }
}