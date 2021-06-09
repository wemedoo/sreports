using Serilog;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Exceptions;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class CustomController : DiagnosticReportCommonController
    {
        public CustomController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL, IEncounterDAL encounterDAL, IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL) : base(patientDAL, episodeOfCareDAL, encounterDAL ,userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL) { }
        // GET: Custom
        public ActionResult CTCAE(int episodeOfCareId)
        {

            Form form = this.formDAL.GetForm(Request.Form["formDefinitionId"]);
            if (form == null)
            {
                Log.Warning(SReportsResource.FormNotExists, 404, Request.Form["formDefinitionId"]);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            FormInstance formInstance = GetFormInstanceSet(form);
            if (episodeOfCareId != 0)
            {
                formInstance.EncounterRef = GetEncounterFromRequestOrCreateDefault(episodeOfCareId);
                formInstance.EpisodeOfCareRef = episodeOfCareId;
                formInstance.PatientId = episodeOfCareDAL.GetById(episodeOfCareId).PatientId;
            }

            try
            {
                formInstanceDAL.InsertOrUpdate(formInstance);
            }
            catch (MongoDbConcurrencyException ex)
            {
                Log.Error(ex.Message);
                return new HttpStatusCodeResult(HttpStatusCode.Conflict, Resources.TextLanguage.ConcurrencyExEdit);
            }

            return Redirect($"{ConfigurationManager.AppSettings["ctcaeUrl"]}?patientId={formInstance.PatientId}&organizationId={userCookieData.ActiveLanguage}&formInstanceId={formInstance.Id}");
        }

    }
}