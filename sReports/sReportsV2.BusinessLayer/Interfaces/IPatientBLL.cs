using sReportsV2.Common.Entities.User;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Patient.DataIn;
using System;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPatientBLL
    {
        ResourceCreatedDTO InsertOrUpdate(PatientDataIn patientDataIn, UserData userData);
        PatientDataOut GetByIdentifier(Identifier identifier);
        PatientDataOut GetById(int id);
        PaginationDataOut<PatientDataOut, DataIn> GetAllFiltered(PatientFilterDataIn dataIn);
        void Delete(int patientId);
        bool ExistsPatientByIdentifier(Identifier identifier); 
        List<PatientDataOut> GetPatientsByName(string searchValue);
        List<PatientDataOut> GetPatientsByFirstAndLastName(PatientSearchFilter patientSearchFilter);
    }
}
