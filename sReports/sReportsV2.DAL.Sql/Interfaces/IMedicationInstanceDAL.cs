using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IMedicationInstanceDAL
    {
        void InsertOrUpdate(MedicationInstance medication);
        MedicationInstance GetById(int id);
        byte[] GetRowVersion(int id);
        int Delete(int id);
        IQueryable<MedicationInstance> FilterByNameAndChemotherapySchemaInstanceAndType(string name, int chemotherapySchemaInstanceId, bool isSupportiveMedicaiton);
        List<string> GetNameByIds(List<int> ids);
    }
}
