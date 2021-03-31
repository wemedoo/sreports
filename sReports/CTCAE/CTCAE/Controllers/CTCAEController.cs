using System;
using System.Collections.Generic;
using System.Linq;
using CTCAE.Common;
using CTCAE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.Extensions.Configuration;

namespace CTCAE.Controllers
{
    public class CTCAEController : Controller
    {
        private IConfiguration configuration;
        public CTCAEController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public ActionResult Index(Patient patient)
        {
            return View("Index", patient);
        }

        public ActionResult GetPatient(Patient patient, string[] chosen, int selectedValue, string[] deleted, string selectId, string fromIndex, string indicator)
        {
            ViewBag.Models = SingletonData.Instance.GetCTCAEModels();

            var session = HttpContext.Session.GetString("patient");
            if (session != null && fromIndex == null)
                patient = JsonConvert.DeserializeObject<Patient>(session);

            patient.SetSelectedValue(selectedValue, ViewBag.Models, indicator);
            patient.SetCheckedValues(selectedValue, indicator);
            patient.SetCTCAETerms(chosen, deleted, selectId, ViewBag.Models, selectedValue);

            HttpContext.Session.SetString("patient", JsonConvert.SerializeObject(patient));
            return View("GetPatient", patient);
        }

        public ActionResult GetSymptoms(char checkedLetter, int selectedValue, string[] terms, string[] chosen, string[] deleted, string[] selectedItem, string[] grades)
        {
            Patient patient = new Patient();
            ViewBag.Models = SingletonData.Instance.GetCTCAEModels();
            var session = HttpContext.Session.GetString("patient");
            patient = JsonConvert.DeserializeObject<Patient>(session);
            patient.SelectItems[selectedValue].Id = selectedValue;
            patient.SelectId = selectedValue;
            patient.FirstLetter = checkedLetter;

            patient.SetCheckedModel(selectedItem, terms, grades);
            patient.AddTerm(chosen, ViewBag.Models, selectedValue);
            patient.RemoveTerm(deleted, null, selectedValue);

            HttpContext.Session.SetString("patient", JsonConvert.SerializeObject(patient));
            return View("GetSymptoms", patient);
        }

        public ActionResult GetReview(string[] grades, string[] description, string[] terms, string[] codes)
        {
            Patient patient = new Patient();
            var session = HttpContext.Session.GetString("patient");
            patient = JsonConvert.DeserializeObject<Patient>(session);
            patient.ReviewModels.Clear();
            patient.SetReviewModel(grades, description, terms, codes);

            HttpContext.Session.SetString("patient", JsonConvert.SerializeObject(patient));
            return View("GetReview", patient);
        }

        public ActionResult GetSummary()
        {
            var session = HttpContext.Session.GetString("patient");
            Patient patient = JsonConvert.DeserializeObject<Patient>(session);

            SendToSReports(patient);

            return View("GetSummary", patient);
        }

        private void SendToSReports(Patient patient)
        {
            string path = configuration.GetSection("sReports").Value;
            var client = new RestClient(path);
            var request = new RestRequest("resource", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(patient);
            client.Execute(request);
        }

    }
}