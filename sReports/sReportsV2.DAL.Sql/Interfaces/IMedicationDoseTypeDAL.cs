using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IMedicationDoseTypeDAL
    {
        List<MedicationDoseType> GetAll();
        MedicationDoseType GetById(int id);
        MedicationDoseType GetByName(string name);
        int GetAllCount();
        void InsertOrUpdate(MedicationDoseType medicationDoseType);
        void InsertMany(List<MedicationDoseType> bodySurfaceCalculationFormulas);
    }
}
