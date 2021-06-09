using AutoMapper;
using Serilog;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.JsonModelBinder;
using sReportsV2.Domain.Entities.DigitalGuideline;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.DigitalGuideline.DataIn;
using sReportsV2.DTOs.DigitalGuideline.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class DigitalGuidelineController : BaseController
    {
        private IDigitalGuidelineService digitalGuidelineService;
        private readonly IThesaurusEntryBLL thesaurusEntryBLL;

        public DigitalGuidelineController(IThesaurusEntryBLL thesaurusEntryBLL)
        {
            this.digitalGuidelineService = new DigitalGuidelineService();
            this.thesaurusEntryBLL = thesaurusEntryBLL;
        }

        // GET: DigitalGuideline
        public ActionResult Index()
        {
            return View();
        }

        [SReportsAuditLog]
        [SReportsAutorize]
        public ActionResult GetAll(GuidelineFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;

            return View();
        }

        [SReportsAutorize]
        public ActionResult ReloadTable(GuidelineFilterDataIn dataIn)
        {
            GuidelineFilter filter = Mapper.Map<GuidelineFilter>(dataIn);

            PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn> result = new PaginationDataOut<GuidelineDataOut, GuidelineFilterDataIn>()
            {
                Count = (int)this.digitalGuidelineService.GetAllCount(filter),
                Data = Mapper.Map<List<GuidelineDataOut>>(this.digitalGuidelineService.GetAll(filter)),
                DataIn = dataIn
            };
            return PartialView("DigitalGuidelineTable", result);
        }

        public async Task<ActionResult> Edit(string id)
        {
            var data = await this.digitalGuidelineService.GetByIdAsync(id).ConfigureAwait(false);
            GuidelineDataOut dataOut = Mapper.Map<GuidelineDataOut>(data);
            return View("Index", dataOut);
        }


        [HttpPost]
        public async Task<ActionResult> Create([ModelBinder(typeof(JsonNetModelBinder))]GuidelineDataIn dataIn) 
        {
           Guideline data =  Mapper.Map<Guideline>(dataIn);

            await this.digitalGuidelineService.InsertOrUpdateAsync(data).ConfigureAwait(false);
            return new HttpStatusCodeResult(HttpStatusCode.Created);
        }

        [HttpPost]
        public async Task<ActionResult> PreviewNode(GuidelineElementDataDataIn dataIn)
        {            
            GuidelineElementDataDataOut data = Mapper.Map<GuidelineElementDataDataOut>(dataIn);
            if(dataIn.Thesaurus != null)
            {
                data.Thesaurus = Mapper.Map<ThesaurusEntryDataOut>(this.thesaurusEntryBLL.GetById(dataIn.Thesaurus.Id));
            }
            return PartialView(data);
        }

        public ActionResult Create()
        {
            return View("Index");
        }

        [SReportsAutorize]
        [SReportsAuditLog]
        [System.Web.Http.HttpDelete]
        public ActionResult Delete(string id, DateTime lastUpdate)
        {
            try
            {
                digitalGuidelineService.Delete(id, lastUpdate);
            }
            catch (MongoDbConcurrencyException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExDeleteEdit;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }
            catch (MongoDbConcurrencyDeleteException ex)
            {
                string message = Resources.TextLanguage.ConcurrencyExDelete;
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }
    }
}