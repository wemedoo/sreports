using ExcelImporter.Importers;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.CustomAttributes;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Singleton;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.Enum.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.Enum.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.HistorySheet.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataOut;
using sReportsV2.DTOs.Pagination;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut;

namespace sReportsV2.Controllers
{
    public class SmartOncologyController : BaseController
    {
        private readonly ISmartOncologyPatientBLL smartOncologyPatientBLL;
        private readonly IChemotherapySchemaBLL chemotherapySchemaBLL;
        private readonly IChemotherapySchemaInstanceBLL chemotherapySchemaInstanceBLL;
        private readonly IUserBLL userBLL;

        public SmartOncologyController(ISmartOncologyPatientBLL smartOncologyPatientBLL, IChemotherapySchemaBLL chemotherapySchemaBLL, IChemotherapySchemaInstanceBLL chemotherapySchemaInstanceBLL, IUserBLL userBLL)
        {
            this.smartOncologyPatientBLL = smartOncologyPatientBLL;
            this.chemotherapySchemaBLL = chemotherapySchemaBLL;
            this.chemotherapySchemaInstanceBLL = chemotherapySchemaInstanceBLL;
            this.userBLL = userBLL;
        }

        [Authorize]
        public ActionResult HistorySheet(int? patientId)
        {
            var patient = patientId != null ? smartOncologyPatientBLL.GetById(patientId.Value) : null;
            return View(new HistorySheetDataOut() { Patient = patient});
        }

        [Authorize]
        public ActionResult PatientOncologyDiseaseHistory(ChemotherapySchemaInstanceFilterDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            var data = smartOncologyPatientBLL.GetPreviewById(dataIn.PatientId);
            ViewBag.FilterData = dataIn;
            return View("Patient/OncologyDiseasesHistory", data);
        }

        [Authorize]
        public ActionResult BasicPatientDataOncology(SmartOncologyPatientFilterDataIn filterDataIn)
        {
            PaginationDataOut<SmartOncologyPatientPreviewDataOut, SmartOncologyPatientFilterDataIn> result = smartOncologyPatientBLL.GetAllFiltered(filterDataIn);

            return View("Patient/BasicPatientDataOncology", result);
        }

        public ActionResult ReloadPatientTable(SmartOncologyPatientFilterDataIn filterDataIn)
        {
            PaginationDataOut<SmartOncologyPatientPreviewDataOut, SmartOncologyPatientFilterDataIn> result = smartOncologyPatientBLL.GetAllFiltered(filterDataIn);

            return PartialView("Patient/PatientEntriesTable", result);
        }

        public ActionResult CreatePatientData()
        {
            SetFormEnumViewbags();
            return PartialView("Patient/EditPatientData");
        }

        public ActionResult ViewPatientData(int id)
        {
            var data = smartOncologyPatientBLL.GetById(id);
            return PartialView("Patient/ViewPatientData", data);
        }

        public ActionResult EditPatientData(int id)
        {
            var data = smartOncologyPatientBLL.GetById(id);
            SetFormEnumViewbags();
            return PartialView("Patient/EditPatientData", data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditPatientData(SmartOncologyPatientDataIn smartOncologyPatientDataIn)
        {
            smartOncologyPatientDataIn = Ensure.IsNotNull(smartOncologyPatientDataIn, nameof(smartOncologyPatientDataIn));
            ResourceCreatedDTO resourceCreatedDTO = smartOncologyPatientBLL.InsertOrUpdate(smartOncologyPatientDataIn, userCookieData);

            return new JsonResult()
            {
                Data = resourceCreatedDTO
            };
        }

        public ActionResult ReloadClinicalTrial(string name)
        {
            var data = userBLL.GetlClinicalTrialsByName(name);
            return PartialView("Patient/ClinicalTrialValues", data);
        }

        public ActionResult GetAutoCompleteEnumData(SmartOncologyEnumAutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            var filtered = SingletonDataContainer.Instance.GetSmartOncologyEnums(dataIn.EnumType)
                .Where(e => e.Name.ToLower().Contains(dataIn.Term.ToLower()));
            var enumDataOuts = filtered
                .OrderBy(x => x.Name).Skip(dataIn.Page * 15).Take(15)
                .Select(e => new AutocompleteDataOut()
                {
                    id = e.Name,
                    text = e.Name
                })
                .Where(x => string.IsNullOrEmpty(dataIn.ExcludeId) || !x.id.Equals(dataIn.ExcludeId))
                .ToList()
                ;

            AutocompleteResultDataOut result = new AutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filtered.Count() / 15.00) > dataIn.Page,
                },
                results = enumDataOuts
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAutoCompletePatientData(AutocompleteDataIn dataIn)
        {
            AutocompleteResultDataOut result = smartOncologyPatientBLL.GetAutocompletePatientData(dataIn);

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetAutoCompleteSchemaData(AutocompleteDataIn dataIn)
        {
            var result = chemotherapySchemaBLL.GetDataForAutocomplete(dataIn);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult CreateNewSchema()
        {
            return View("SchemaDefinition/EditSchema");
        }

        [Authorize]
        public ActionResult EditSchema(int id)
        {
            var data = chemotherapySchemaBLL.GetById(id);
            return View("SchemaDefinition/EditSchema", data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditSchema(ChemotherapySchemaDataIn chemotherapySchemaDataIn)
        {
            var data = chemotherapySchemaBLL.InsertOrUpdate(chemotherapySchemaDataIn, userCookieData);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSchemaGeneralProperties(EditGeneralPropertiesDataIn chemotherapySchemaDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateGeneralProperties(chemotherapySchemaDataIn, userCookieData);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSchemaIndications(EditIndicationsDataIn indicationsDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateIndications(indicationsDataIn, userCookieData);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSchemaName(EditNameDataIn nameDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateName(nameDataIn, userCookieData);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchemaReference(int id)
        {
            var data = chemotherapySchemaBLL.GetReference(id);
            return PartialView("SchemaDefinition/EditReferenceModal", data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSchemaReference(EditLiteratureReferenceDataIn referenceDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateReference(referenceDataIn, userCookieData);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchemaMedication(int id)
        {
            var data = chemotherapySchemaBLL.GetMedication(id);
            ViewBag.BodyCalculationFormulas = chemotherapySchemaBLL.GetFormulas();
            ViewBag.SchemaDefinitionMedication = true;
            return PartialView("SchemaDefinition/EditMedication", data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSchemaMedication(MedicationDataIn medicationDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateMedication(medicationDataIn);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditMedicationDoseContent(MedicationDoseDataOut dataIn)
        {
            ViewBag.MedicationDoseTypes = chemotherapySchemaBLL.GetMedicationDoseTypes();
            return PartialView("SchemaDefinition/EditMedicationDoseContent", dataIn);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSchemaMedicationDose(MedicationDoseDataIn medicationDoseDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateMedicationDose(medicationDoseDataIn);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSchemaMedicationDoseInBatch(EditMedicationDoseInBatchDataIn editMedicationDoseInBatchDataIn)
        {
            var data = chemotherapySchemaBLL.UpdateMedicationDoseInBatch(editMedicationDoseInBatchDataIn);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteMedicationDose(EditMedicationDoseDataIn dataIn)
        {
            chemotherapySchemaBLL.DeleteDose(dataIn);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult EditMedicationDoseTimes(int interval)
        {
            var data = chemotherapySchemaBLL.GetMedicationDoseType(interval);
            return PartialView("SchemaDefinition/EditMedicationDoseTimes", data.IntervalsList);
        }

        public ActionResult GetAutocompleteRouteOfAdministrationData(AutocompleteDataIn dataIn)
        {
            var result = chemotherapySchemaBLL.GetRouteOfAdministrationDataForAutocomplete(dataIn);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRouteOfAdministration(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                RouteOfAdministrationDTO routeOfAdministration = chemotherapySchemaBLL.GetRouteOfAdministration(id.Value);
                return Json(new { id = routeOfAdministration.Id, text = routeOfAdministration.Name }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetAutocompleteUnitData(AutocompleteDataIn dataIn)
        {
            var result = chemotherapySchemaBLL.GetUnitDataForAutocomplete(dataIn);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUnit(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                UnitDTO unit = chemotherapySchemaBLL.GetUnit(id.Value);
                return Json(new { id = unit.Id, text = unit.Name }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }

        }

        [Authorize]
        public ActionResult PreviewSchema(int id)
        {
            var data = chemotherapySchemaBLL.GetById(id);
            return View("SchemaDefinition/PreviewSchema", data);
        }

        [Authorize]
        public ActionResult BrowseSchemas(ChemotherapySchemaFilterDataIn dataIn)
        {
            ViewBag.FilterData = dataIn;
            return View("SchemaDefinition/GetAll");
        }

        public ActionResult ReloadSchemas(ChemotherapySchemaFilterDataIn dataIn)
        {
            var result = chemotherapySchemaBLL.ReloadTable(dataIn);
            return PartialView("SchemaDefinition/SchemaDefinitionEntryTable", result);
        }

        [Authorize]
        [System.Web.Http.HttpDelete]
        [SReportsAuditLog]
        public ActionResult DeleteSchema(int id)
        {
            chemotherapySchemaBLL.Delete(id);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize]
        public ActionResult ProgressNote(int? schemaInstanceId)
        {
            var data = schemaInstanceId.HasValue ? chemotherapySchemaInstanceBLL.GetSchemaInstance(schemaInstanceId.Value) : new ChemotherapySchemaInstanceDataOut();
            return View("ProgressNote/ProgressNote", data);
        }

        public ActionResult ViewPatientDataProgressNote(int? id, string viewDisplayType)
        {
            var data = id != null ? smartOncologyPatientBLL.GetById(id.Value) : null;
            return PartialView("ProgressNote/ViewPatientData" + viewDisplayType, new ChemotherapySchemaInstanceDataOut() {Patient = data});
        }

        public ActionResult ViewSchema(int id, DateTime? schemaStartDate, string viewDisplayType)
        {
            var schema = chemotherapySchemaBLL.GetSchemaDefinition(id, schemaStartDate);
            ViewBag.ViewDisplayType = viewDisplayType;

            return PartialView("ProgressNote/ViewSchemaData", schema);
        }

        public ActionResult ViewSchemaInstance(int id)
        {
            var schemaInstance = chemotherapySchemaInstanceBLL.GetById(id);

            return Json(schemaInstance, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewSchemaInstanceTableData(int id)
        {
            var data = chemotherapySchemaInstanceBLL.GetSchemaTableData(id);

            return PartialView("ProgressNote/ViewSchemaData", data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditSchemaInstance(ChemotherapySchemaInstanceDataIn chemotherapySchemaInstanceDataIn)
        {
            var data = chemotherapySchemaInstanceBLL.InsertOrUpdate(chemotherapySchemaInstanceDataIn, userCookieData);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSchemaMedicationInstance(int id)
        {
            var data = chemotherapySchemaBLL.GetMedication(id);
            ViewBag.BodyCalculationFormulas = chemotherapySchemaBLL.GetFormulas();
            ViewBag.SchemaDefinitionMedication = false;
            return PartialView("SchemaDefinition/EditMedication", data);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSchemaMedicationInstance(MedicationInstanceDataIn medicationDataIn)
        {
            medicationDataIn = Ensure.IsNotNull(medicationDataIn, nameof(medicationDataIn));
            chemotherapySchemaInstanceBLL.UpdateMedicationInstance(medicationDataIn, userCookieData);
            return ViewSchemaInstanceTableData(medicationDataIn.ChemotherapySchemaInstanceId);
        }

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteMedicationInstance(DeleteMedicationInstanceDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            chemotherapySchemaInstanceBLL.DeleteMedicationInstance(dataIn, userCookieData);
            return ViewSchemaInstanceTableData(dataIn.ChemotherapySchemaInstanceId);
        }

        public ActionResult GetAutoCompleteMedicationData(MedicationInstanceAutocompleteDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            var data = chemotherapySchemaInstanceBLL.GetMedicationInstanceDataForAutocomplete(dataIn);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewMedicationReplacements(int medicationInstanceId)
        {
            var data = chemotherapySchemaInstanceBLL.GetReplacementHistoryForMedication(medicationInstanceId);
            return PartialView("SchemaInstance/ViewMedicationReplacementContent", data);
        }

        public ActionResult EditMedicationDoseInstanceContent(MedicationDoseInstanceDataOut dataIn)
        {
            ViewBag.MedicationDoseTypes = chemotherapySchemaBLL.GetMedicationDoseTypes();
            return PartialView("SchemaDefinition/EditMedicationDoseContent", dataIn);
        }

        [Authorize]
        [HttpPost]
        public ActionResult UpdateSchemaMedicationDoseInstance(MedicationDoseInstanceDataIn medicationDoseInstanceDataIn)
        {
            var data = chemotherapySchemaInstanceBLL.UpdateMedicationDoseInstance(medicationDoseInstanceDataIn, userCookieData);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpDelete]
        public ActionResult DeleteMedicationDoseInstance(EditMedicationDoseDataIn dataIn)
        {
            chemotherapySchemaInstanceBLL.DeleteDose(dataIn, userCookieData);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        [Authorize]
        [HttpDelete]
        [SReportsAuditLog]
        public ActionResult DeleteSchemaInstance(int id)
        {
            chemotherapySchemaInstanceBLL.Delete(id);
            return new HttpStatusCodeResult(HttpStatusCode.NoContent);
        }

        public ActionResult ReloadSchemaInstances(ChemotherapySchemaInstanceFilterDataIn dataIn)
        {
            var result = chemotherapySchemaInstanceBLL.ReloadTable(dataIn);
            return PartialView("SchemaInstance/SchemaInstanceEntryTable", result);
        }

        public ActionResult DelayDose(DelayDoseDataIn dataIn)
        {
            var data = chemotherapySchemaInstanceBLL.DelayDose(dataIn, userCookieData);

            return PartialView("ProgressNote/ViewSchemaData", data);
        }

        public ActionResult ViewHistoryOfDayDose(DelayDoseHistoryDataIn dataIn)
        {
            var data = chemotherapySchemaInstanceBLL.ViewHistoryOfDayDose(dataIn);

            return PartialView("SchemaInstance/ViewHistoryOfDayDoseContent", data);
        }

        public ActionResult ParseChemotherapySchemaV2()
        {
            chemotherapySchemaBLL.ParseExcelDataAndInsert(userCookieData.Id);


            return new JsonResult()
            {
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                Data = null
            };
        }

        private void SetFormEnumViewbags()
        {
            ViewBag.PresentationState = SingletonDataContainer.Instance.GetSmartOncologyEnums(SmartOncologyEnumNames.PresentationStage);
            ViewBag.Anatomy = SingletonDataContainer.Instance.GetSmartOncologyEnums(SmartOncologyEnumNames.Anatomy);
            ViewBag.Morphology = SingletonDataContainer.Instance.GetSmartOncologyEnums(SmartOncologyEnumNames.Morphology);
            ViewBag.TherapeuticContext = SingletonDataContainer.Instance.GetSmartOncologyEnums(SmartOncologyEnumNames.TherapeuticContext);
            ViewBag.ChemotherapyType = SingletonDataContainer.Instance.GetSmartOncologyEnums(SmartOncologyEnumNames.ChemotherapyType);
        }
    }
}