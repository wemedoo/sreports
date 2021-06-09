using sReportsV2.DTOs.ClinicalTrials;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DAL.Sql.Implementations;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using AutoMapper;

namespace sReportsV2.Controllers
{
    public class ClinicalTrialsController : Controller
    {
        //public IThesaurusService thesaurusService;
        //public IThesaurusTranslationService thesaurusTranslationService;
        //public ISimilarTermService similarTermService;
        //public ICodeableConceptService codeableConceptService;


        public ClinicalTrialsController()
        {
            //thesaurusService = new ThesaurusService();
            //thesaurusTranslationService = new ThesaurusTranslationService();
            //similarTermService = new SimilarTermService();
            //codeableConceptService = new CodeableConceptService();

        }
        // GET: ClinicalTrials
        [System.Web.Mvc.HttpPost]
        public ActionResult GetSimilarTermsAndIcd10Codes(List<string> terms, bool includeIcd10)
        {
            Icd10ResultDataOut result = new Icd10ResultDataOut();
            //List<SimilarTermSearch> simTerms = similarTermService.GetTermFiltered(terms);
            //result.SimilarTerms =  Mapper.Map<List<string>>(simTerms.Select(x => x.Name).Distinct().ToList());

            //result.Icd10Codes = includeIcd10 ? GetIcd10Codes(simTerms) : new List<string>();

            JsonResult jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;

            return jsonResult;

        }

        private List<string> GetIcd10Codes(List<SimilarTermSearch> simTerms) 
        {
            List<string> result = new List<string>();

            int transactionSize = 10000;
            using (var context = new SReportsContext())
            {
                if (simTerms != null && simTerms.Count > 0)
                {

                    var translationsIds = simTerms.Select(x => x.ThesaurusEntryTranslationId).Distinct().ToList();
                    for (int i = 0; i < translationsIds.Count / transactionSize + 1; i++)
                    {
                        List<int> listIds = translationsIds.Skip(i * transactionSize).Take(transactionSize).ToList();
                        var thesaurusIds = context.ThesaurusEntryTranslations.Where(x => listIds.Contains(x.Id)).Select(x => x.ThesaurusEntryId).Distinct().ToList();
                        var codes = context.O4CodeableConcept.Where(x => thesaurusIds.Contains(x.ThesaurusEntryId) && x.System.Label.ToLower().Contains("icd10")).Select(x => x.Code).ToList();
                        result.AddRange(codes);
                    }
                    result = result.Distinct().ToList();
                }
            }

            return result;
        }
    }
}