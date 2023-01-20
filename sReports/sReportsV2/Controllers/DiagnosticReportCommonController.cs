using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Patient;
using sReportsV2.SqlDomain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class DiagnosticReportCommonController : FormCommonController
    {

        public DiagnosticReportCommonController(IPatientDAL patientDAL, IEpisodeOfCareDAL episodeOfCareDAL,IEncounterDAL encounterDAL, IUserBLL userBLL, IOrganizationBLL organizationBLL, ICustomEnumBLL customEnumBLL, IFormInstanceBLL formInstanceBLL, IFormBLL formBLL) : base(patientDAL, episodeOfCareDAL,encounterDAL, userBLL, organizationBLL, customEnumBLL, formInstanceBLL, formBLL) { }
        protected ActionResult GetEdit(FormInstance formInstance, int episodeOfCareId)
        {
            EpisodeOfCareDataOut episodeOfCareDataOut = Mapper.Map<EpisodeOfCareDataOut>(episodeOfCareDAL.GetById(episodeOfCareId));
            DiagnosticReportCreateDataOut diagnosticReportCreateDataOut = new DiagnosticReportCreateDataOut()
            {
                EpisodeOfCare = episodeOfCareDataOut,
                Patient = Mapper.Map<PatientDataOut>(patientDAL.GetById(episodeOfCareDataOut.PatientId)),
                Referrals = formInstance.Referrals
            };

            List<FormInstance> referrals = formInstanceBLL.GetByIds(formInstance.Referrals);
            diagnosticReportCreateDataOut.Form = formBLL.GetFormDataOut(formInstance, referrals, userCookieData);

            ViewBag.FormInstanceId = formInstance.Id;
            ViewBag.EncounterId = formInstance.EncounterRef;
            ViewBag.LastUpdate = formInstance.LastUpdate;


            return View("Create", diagnosticReportCreateDataOut);
        }

        protected EpisodeOfCareListFormsDataOut GetEpisodeOfCareListFormsDataOut(int episodeOfCareId, List<string> referrals, string encounterId)
        {
            EpisodeOfCareDataOut episodeOfCareDataOut = Mapper.Map<EpisodeOfCareDataOut>(episodeOfCareDAL.GetById(episodeOfCareId));

            EpisodeOfCareListFormsDataOut episodeOfCareListFormsDataOut = new EpisodeOfCareListFormsDataOut()
            {
                EpisodeOfCare = episodeOfCareDataOut,
                Forms = Mapper.Map<List<FormEpisodeOfCareDataOut>>(formDAL.GetAllByOrganizationAndLanguage(userCookieData.ActiveOrganization, userCookieData.ActiveLanguage)),
                Patient = Mapper.Map<PatientDataOut>(patientDAL.GetById(episodeOfCareDataOut.PatientId)),
                Referrals = referrals,
                EncounterId = encounterId

            };

            return episodeOfCareListFormsDataOut;
        }

        protected List<Form> GetRefeerals(List<string> referrals)
        {
            List<FormInstance> formInstancesReferrals = referrals != null ? formInstanceDAL.GetByIds(referrals).ToList() : new List<FormInstance>();
            return formBLL.GetFormsFromReferrals(formInstancesReferrals);
            
        }
    }
}