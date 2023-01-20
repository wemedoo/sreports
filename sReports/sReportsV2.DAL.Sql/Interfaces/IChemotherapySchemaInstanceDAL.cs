using sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance;
using sReportsV2.SqlDomain.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IChemotherapySchemaInstanceDAL
    {
        void InsertOrUpdate(ChemotherapySchemaInstance chemotherapySchemaInstance);
        void Delete(int id);
        ChemotherapySchemaInstance GetById(int id);
        string GetName(int id);
        ChemotherapySchemaInstance GetSchemaInstance(int id);
        List<ChemotherapySchemaInstance> GetAll(ChemotherapySchemaInstanceFilter chemotherapySchemaFilter);
        long GetAllFilteredCount(ChemotherapySchemaInstanceFilter chemotherapySchemaFilter);
        byte[] GetRowVersion(int id);
    }
}
