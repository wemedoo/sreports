using sReportsV2.Domain.Sql.Entities.SmartOncologyPatient;
using sReportsV2.SqlDomain.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface ISmartOncologyPatientDAL
    {
        void InsertOrUpdate(SmartOncologyPatient patient);
        int GetAllEntriesCount(SmartOncologyPatientFilter filter);
        List<SmartOncologyPatient> GetAll(SmartOncologyPatientFilter filter);
        List<SmartOncologyPatient> GetPatientsByName(string name);
        SmartOncologyPatient GetById(int id);
        void Delete(int patientId);
    }
}
