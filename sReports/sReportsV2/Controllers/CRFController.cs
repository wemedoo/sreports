using AutoMapper;
using Serilog;
using sReportsV2.Common.Singleton;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.CRF.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace sReportsV2.Controllers
{
    public class CRFController : FormCommonController
    {

        private List<string> ApprovedLanguages = new List<string>() { "de", "fr", "sr", "sr-Cyrl-RS", "en","ru", "es", "pt" };
        // GET: CRF
        public ActionResult Create(string id, string language = "en")
        {
            if (string.IsNullOrWhiteSpace(language))
            {
                id = "14573";
                language = "en";
            }
            Form form = formService.GetFormByThesaurusAndLanguage(id, language);
            if (form == null)
            {
                Log.Warning(SReportsResource.FormNotExists, 404, id);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            FormDataOut data = Mapper.Map<FormDataOut>(form);
            List<FieldDataOut> fields = Mapper.Map<List<FieldDataOut>>(form.GetAllFields());
            foreach (FieldSelectableDataOut formFieldDataOut in data.GetAllFields().OfType<FieldSelectableDataOut>())
            {
                formFieldDataOut.GetDependablesData(fields, formFieldDataOut.Dependables);
            }
            Form form1 = form.ThesaurusId == "14573"? form:  formService.GetFormByThesaurusAndLanguage("14573", language);
            Form form2 = form.ThesaurusId == "14911" ? form : formService.GetFormByThesaurusAndLanguage("14911", language);
            Form form3 = form.ThesaurusId == "15112" ? form : formService.GetFormByThesaurusAndLanguage("15112", language);

            List<Form> formsForTree = new List<Form>();
            formsForTree.Add(form1);
            formsForTree.Add(form2);
            formsForTree.Add(form3);


            SetApprovedLanguages();
            ViewBag.Language = language;
            ViewBag.Tree = GetTreeJson(formsForTree);
            ViewBag.TreeForms = formsForTree;
            ViewBag.MainCreateAction = "crf/create?";
            return View(data);
        }

        private List<TreeJsonDataOut> GetTreeJson(List<Form> formsData)
        {
            List<TreeJsonDataOut> result = new List<TreeJsonDataOut>();
            foreach(Form formData in formsData)
            {
                TreeJsonDataOut treeJsonDataOut = new TreeJsonDataOut();
                treeJsonDataOut.text = formData.Title;
                treeJsonDataOut.nodes = formData.Chapters.Select(x => new TreeJsonDataOut() { text = x.Title, href = $"#@c.Id" }).ToList();

                result.Add(treeJsonDataOut);
            }

            return result;
        }


        public ActionResult Edit(FormInstanceFilterDataIn filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }
            if (string.IsNullOrWhiteSpace(filter.Language))
            {
                filter.ThesaurusId = "14573";
                filter.Language = "en";
            }

            FormInstance formInstance = formInstanceService.GetById(filter.FormInstanceId);
            if (formInstance == null)
            {
                Log.Warning(SReportsResource.FormInstanceNotExists, 404, filter.FormInstanceId);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            ViewBag.FormInstanceId = filter.FormInstanceId;
            ViewBag.Title = formInstance.Title;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            ViewBag.Language = filter.Language;
            ViewBag.ThesaurusId = formInstance.ThesaurusId;
            SetApprovedLanguages();
            return GetEdit(formInstance, filter);
        }

        [HttpPost]
        public ActionResult Create(string language)
        {
            Form form = this.formService.GetForm(Request.Form["formDefinitionId"]);
            if (form == null)
            {
                Log.Warning(SReportsResource.FormNotExists, 404, Request.Form["formDefinitionId"]);
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            FormInstance formInstance = GetFormInstanceSet(form);

            formInstanceService.InsertOrUpdate(formInstance);

            return RedirectToAction("GetAllByFormThesaurus", "FormInstance", new
            {
                thesaurusId = Request.Form["thesaurusId"],
                formId = form.Id,
                title = form.Title,
                IsSimplifiedLayout = true,
                Language = language
            });
        }

        public ActionResult Instructions(string language)
        {
            ViewBag.Language = language;

            return View();
        }

        private ActionResult GetEdit(FormInstance formInstance, FormInstanceFilterDataIn filter)
        {
            ViewBag.FormInstanceId = filter.FormInstanceId;
            ViewBag.EncounterId = formInstance.EncounterRef;
            ViewBag.FilterFormInstanceDataIn = filter;
            ViewBag.LastUpdate = formInstance.LastUpdate;
            Form form = formService.GetForm(formInstance.FormDefinitionId);
            form.SetFields(formInstance.Fields);
            FormDataOut data = Mapper.Map<FormDataOut>(form);
            List<FieldDataOut> fields = Mapper.Map<List<FieldDataOut>>(form.GetAllFields());

            foreach (FieldSelectableDataOut formFieldDataOut in data.GetAllFields().OfType<FieldSelectableDataOut>())
            {
                formFieldDataOut.GetDependablesData(fields, formFieldDataOut.Dependables);
            }

            return View("~/Views/CRF/Create.cshtml", data);
        }

        private void SetApprovedLanguages()
        {
            ViewBag.Languages = SingletonDataContainer.Instance.GetLanguages().Where(x => ApprovedLanguages.Contains(x.Value)).OrderBy(x => x.Label).ToList();
        }


    }
}