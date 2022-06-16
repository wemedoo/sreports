using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IPatientDAL
    {
        void InsertOrUpdate(Patient patient);
        int GetAllEntriesCount(PatientFilter filter);
        List<Patient> GetAll(PatientFilter filter);
        Patient GetById(int id);
        Patient GetByIdentifier(Identifier identifier);
        bool ExistsPatientByIdentifier(Identifier identifier);
        bool ExistsPatient(int id);
        bool Delete(int patientId, DateTime lastUpdate);
    }
}
