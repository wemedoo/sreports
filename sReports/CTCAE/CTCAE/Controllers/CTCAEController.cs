using CTCAE.Common;
using CTCAE.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System;
using System.Web;

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
            ViewBag.Env = configuration.GetSection("Env").Value;
            return View("Index", patient);
        }

        public ActionResult SendPatientInfo(string patientId, string formInstanceId, string organizationId)
        {
            Patient patient = new Patient()
            {
                PatientId = patientId,
                FormInstanceId = formInstanceId,
                OrganizationRef = organizationId

            };
            return View("Index", patient);
        }

        public ActionResult GetPatient(Patient patient, string[] chosen, string selectedValue, string[] deleted, string selectId, string fromIndex, string indicator, string title)
        {
            ViewBag.Models = SingletonData.Instance.GetCTCAEModels();
            ViewBag.Templates = SingletonData.Instance.GetTemplateList();
            patient.Templates = SingletonData.Instance.GetTemplateList();

            var session = HttpContext.Session.GetString("patient");

            if (session != null && fromIndex == null)
                patient = JsonConvert.DeserializeObject<Patient>(session);
            if (fromIndex == "yes" || (!patient.SelectItems.Any(x => x.Id == patient.SelectId) && fromIndex == null))
                patient.SelectId = patient.Templates.FirstOrDefault().Id;


            if (indicator == "admin")
            {
                patient.SetTemplateList(chosen, deleted, ViewBag.Models, title);
                WriteInJsonFile(patient.Templates);
            }
            else
            {
                patient.SetSelectedValue(selectedValue, ViewBag.Templates);
                patient.SetCheckedValues(selectedValue, indicator);
                patient.SetCTCAETerms(chosen, deleted, selectId, ViewBag.Models, selectedValue, ViewBag.Templates);
            }

            HttpContext.Session.SetString("patient", JsonConvert.SerializeObject(patient));
            return View("GetPatient", patient);
        }

        public ActionResult GetSymptoms(char checkedLetter, string selectedValue, string[] terms, string[] chosen, string[] deleted, string[] selectedItem, string[] grades)
        {
            ViewBag.Models = SingletonData.Instance.GetCTCAEModels();
            var session = HttpContext.Session.GetString("patient");
            Patient patient = JsonConvert.DeserializeObject<Patient>(session);
            patient.SelectItems.Find(x => x.Id == selectedValue).Id = selectedValue;
            patient.SelectId = selectedValue;
            patient.FirstLetter = checkedLetter;

            patient.SetCheckedModel(selectedItem, terms, grades);
            patient.AddTerm(chosen, ViewBag.Models, selectedValue, "");
            patient.RemoveTerm(deleted, null, selectedValue, "");

            HttpContext.Session.SetString("patient", JsonConvert.SerializeObject(patient));
            return View("GetSymptoms", patient);
        }
        public ActionResult GetAdmin(char checkedLetter, string indicator, string[] chosen, string[] deleted, string title)
        {
            ViewBag.Models = SingletonData.Instance.GetCTCAEModels();
            ViewBag.Templates = SingletonData.Instance.GetTemplateList();
            ViewBag.TemplateTitle = title;
            var session = HttpContext.Session.GetString("patient");
            Patient patient = JsonConvert.DeserializeObject<Patient>(session);
            patient.FirstLetter = checkedLetter;
            if (indicator != "existingTemplate")
                patient.SetTemplatePatientInfo();

            patient.AddTerm(chosen, ViewBag.Models, patient.SelectId, "admin");
            patient.RemoveTerm(deleted, null, patient.SelectId, "admin");
            HttpContext.Session.SetString("patient", JsonConvert.SerializeObject(patient));
            return View("Admin", patient);
        }

        public ActionResult TemplateTable()
        {
            ViewBag.Templates = SingletonData.Instance.GetTemplateList();
            return View("TemplateTable");
        }

        public ActionResult GetReview(string[] grades, string[] description, string[] terms, string[] codes)
        {
            var session = HttpContext.Session.GetString("patient");
            Patient patient = JsonConvert.DeserializeObject<Patient>(session);
            patient.ReviewModels.Clear();
            patient.SetReviewModel(grades, description, terms, codes);
            patient.Title = patient.SelectItems.Find(x => x.Id == patient.SelectId).Label;

            HttpContext.Session.SetString("patient", JsonConvert.SerializeObject(patient));
            return View("GetReview", patient);
        }

        public ActionResult GetSummary()
        {
            var session = HttpContext.Session.GetString("patient");
            Patient patient = JsonConvert.DeserializeObject<Patient>(session);

            var result = SendToSReports(patient);
            if(result != null)
            {
                return result;
            }

            return View("GetSummary", patient);
        }

        public ActionResult Edit(string templateId)
        {
            ViewBag.Models = SingletonData.Instance.GetCTCAEModels();
            ViewBag.Templates = SingletonData.Instance.GetTemplateList();
            var session = HttpContext.Session.GetString("patient");
            Patient patient = JsonConvert.DeserializeObject<Patient>(session);
            patient.EditInformation(ViewBag.Templates, templateId);
            ViewBag.TemplateTitle = patient.Title;

            HttpContext.Session.SetString("patient", JsonConvert.SerializeObject(patient));
            return View("Admin", patient);
        }

        public ActionResult Delete(string templateId)
        {
            var session = HttpContext.Session.GetString("patient");
            Patient patient = JsonConvert.DeserializeObject<Patient>(session);
            RemoveFromJsonFile(patient, SingletonData.Instance.GetTemplateList(), templateId);

            ViewBag.Templates = SingletonData.Instance.GetTemplateList();
            HttpContext.Session.SetString("patient", JsonConvert.SerializeObject(patient));
            return View("TemplateTable");
        }

        private ActionResult SendToSReports(Patient patient)
        {
            string path = configuration.GetSection("sReportsCreateCTCAE").Value;
            var client = new RestClient(path);
            var request = new RestRequest("resource", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(patient);
            IRestResponse response = client.Execute(request);
            if(response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                if(!string.IsNullOrEmpty(patient.FormInstanceId))
                {
                    return Redirect(string.Format(configuration.GetSection("sReportsPatientUrl").Value,patient.PatientId, patient.FormInstanceId));
                }
                TempData["Message"] = "Successfully sent to sReports";
            }

            return null;
        }

        private void WriteInJsonFile(List<SelectItemModel> templates)
        {
            List<SelectJsonModel> templatesForSave = MappingToJsonObject(templates);
            WriteJson(templatesForSave);
        }

        private void RemoveFromJsonFile(Patient patient, List<SelectItemModel> templateList, string templateId)
        {
            string json = System.IO.File.ReadAllText($"{AppDomain.CurrentDomain.GetData("DataDirectory")}\\templates.json");
            List<SelectJsonModel> savedTemplates = JsonConvert.DeserializeObject<List<SelectJsonModel>>(json);
            int index = 0;
            foreach (SelectItemModel item in templateList)
            {
                if (item.Id == templateId)
                {
                    savedTemplates.RemoveAt(index);
                    patient.SelectItems.RemoveAt(index);
                    patient.Templates.RemoveAt(index);
                }
                index++;
            }
            WriteJson(savedTemplates);
        }

        private void WriteJson(List<SelectJsonModel> savedTemplates) 
        {
            using (StreamWriter file = System.IO.File.CreateText($"{AppDomain.CurrentDomain.GetData("DataDirectory")}\\templates.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, savedTemplates);
            }
        }

        private List<SelectJsonModel> MappingToJsonObject(List<SelectItemModel> templates) 
        {
            List<SelectJsonModel> templatesForSave = new List<SelectJsonModel>();
            for (int i = 0; i < templates.Count; i++)
            {
                SelectJsonModel item = new SelectJsonModel();
                item.Id = templates[i].Id;
                item.Label = templates[i].Label;
                for (int j = 0; j < templates[i].DefaultList.Count; j++)
                    item.DefaultList.Add(templates[i].DefaultList[j].MedDraCode);

                templatesForSave.Add(item);
            }
            return templatesForSave;
        }

    }
}