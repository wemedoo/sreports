using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using System;
using System.Collections.Generic;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IPatientDAL
    {
        void InsertOrUpdate(Patient patient, int defaultIdentifierId = 0);
        int GetAllEntriesCount(PatientFilter filter);
        List<Patient> GetAll(PatientFilter filter);
        Patient GetById(int id);
        Patient GetByIdentifier(Identifier identifier);
        bool ExistsPatientByIdentifier(Identifier identifier);
        bool ExistsPatient(int id);
        void Delete(int patientId);
        List<Patient> GetAllByIds(List<int> ids);
        List<AutoCompletePatientData> GetPatientsFilteredByName(string searchValue);
        List<AutoCompletePatientData> GetPatientsFilteredByFirstAndLastName(PatientSearchFilter patientSearchFilter);
    }
}
