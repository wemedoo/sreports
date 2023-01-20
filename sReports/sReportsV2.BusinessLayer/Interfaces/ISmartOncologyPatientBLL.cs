using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.SmartOncology.ProgressNote.DataOut;
using sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataIn;
using sReportsV2.DTOs.DTOs.SmartOncologyPatient.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DTO;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface ISmartOncologyPatientBLL
    {
        ResourceCreatedDTO InsertOrUpdate(SmartOncologyPatientDataIn patientDataIn, UserCookieData userCookieData);
        SmartOncologyPatientDataOut GetById(int id);
        SmartOncologyPatientPreviewDataOut GetPreviewById(int id);
        PaginationDataOut<SmartOncologyPatientPreviewDataOut, SmartOncologyPatientFilterDataIn> GetAllFiltered(SmartOncologyPatientFilterDataIn dataIn);
        AutocompleteResultDataOut GetAutocompletePatientData(AutocompleteDataIn dataIn);
        void Delete(int patientId);
    }
}
