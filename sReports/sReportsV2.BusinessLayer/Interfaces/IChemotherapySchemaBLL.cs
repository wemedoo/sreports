using ExcelImporter.Classes;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IChemotherapySchemaBLL
    {
        PaginationDataOut<ChemotherapySchemaDataOut, DataIn> ReloadTable(ChemotherapySchemaFilterDataIn dataIn);
        ResourceCreatedDTO InsertOrUpdate(ChemotherapySchemaDataIn dataIn, UserCookieData userCookieData);
        void Delete(int id);
        ResourceCreatedDTO UpdateName(EditNameDataIn nameDataIn, UserCookieData userCookieData);
        ResourceCreatedDTO UpdateGeneralProperties(EditGeneralPropertiesDataIn dataIn, UserCookieData userCookieData);
        ChemotherapySchemaDataOut UpdateIndications(EditIndicationsDataIn indicationsDataIn, UserCookieData userCookieData);
        ResourceCreatedDTO UpdateMedication(MedicationDataIn dataIn);
        ResourceCreatedDTO UpdateMedicationDose(MedicationDoseDataIn dataIn);
        EditMedicationDoseInBatchDataOut UpdateMedicationDoseInBatch(EditMedicationDoseInBatchDataIn dataIn);
        ChemotherapySchemaResourceCreatedDTO UpdateReference(EditLiteratureReferenceDataIn dataIn, UserCookieData userCookieData);
        ChemotherapySchemaDataOut GetById(int id);
        SchemaTableDataOut GetSchemaDefinition(int id, DateTime? firstDay);
        AutocompleteResultDataOut GetDataForAutocomplete(AutocompleteDataIn dataIn);
        MedicationDataOut GetMedication(int id);
        LiteratureReferenceDataOut GetReference(int id);
        List<BodySurfaceCalculationFormulaDTO> GetFormulas();
        AutocompleteResultDataOut GetRouteOfAdministrationDataForAutocomplete(AutocompleteDataIn dataIn);
        RouteOfAdministrationDTO GetRouteOfAdministration(int id);
        AutocompleteResultDataOut GetUnitDataForAutocomplete(AutocompleteDataIn dataIn);
        UnitDTO GetUnit(int id);
        List<MedicationPreviewDoseTypeDTO> GetMedicationDoseTypes();
        MedicationDoseTypeDTO GetMedicationDoseType(int id);
        void ParseExcelDataAndInsert(int creatorId);
        void DeleteDose(EditMedicationDoseDataIn deleteDoseDataIn);

    }
}
