using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IMedicationDoseInstanceDAL
    {
        void InsertOrUpdate(MedicationDoseInstance medicationDoseInstance);
        MedicationDoseInstance GetById(int id);
        int Delete(int id);
    }
}
