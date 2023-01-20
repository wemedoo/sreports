using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchema.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncology.ChemotherapySchemaInstance.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IChemotherapySchemaInstanceBLL
    {
        ResourceCreatedDTO InsertOrUpdate(ChemotherapySchemaInstanceDataIn dataIn, UserCookieData userCookieData);
        void Delete(int id);
        void DeleteDose(EditMedicationDoseDataIn dataIn, UserCookieData userCookieData);
        void DeleteMedicationInstance(DeleteMedicationInstanceDataIn dataIn, UserCookieData userCookieData);
        ChemotherapySchemaInstanceDataOut GetById(int id);
        ChemotherapySchemaInstanceDataOut GetSchemaInstance(int id);
        SchemaTableDataOut GetSchemaTableData(int chemotherapySchemaInstanceId);
        ChemotherapySchemaResourceCreatedDTO UpdateMedicationInstance(MedicationInstanceDataIn dataIn, UserCookieData userCookieData);
        MedicationDoseInstanceDataOut UpdateMedicationDoseInstance(MedicationDoseInstanceDataIn dataIn, UserCookieData userCookieData);
        PaginationDataOut<ChemotherapySchemaInstancePreviewDataOut, DataIn> ReloadTable(ChemotherapySchemaInstanceFilterDataIn dataIn);
        SchemaTableDataOut DelayDose(DelayDoseDataIn dataIn, UserCookieData userCookieData);
        ChemotherapySchemaInstanceHistoryDataOut ViewHistoryOfDayDose(DelayDoseHistoryDataIn dataIn);
        AutocompleteResultDataOut GetMedicationInstanceDataForAutocomplete(MedicationInstanceAutocompleteDataIn dataIn);
        MedicationReplacementHistoryDataOut GetReplacementHistoryForMedication(int medicationId);
    }
}
