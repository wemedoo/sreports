using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IMedicationDoseDAL
    {
        MedicationDose GetById(int id);
        void InsertOrUpdate(MedicationDose medicationDose);
        void Delete(int id);
    }
}
