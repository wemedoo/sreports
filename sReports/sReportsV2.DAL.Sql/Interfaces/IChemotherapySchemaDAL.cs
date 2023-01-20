using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.SqlDomain.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IChemotherapySchemaDAL
    {
        void InsertOrUpdate(ChemotherapySchema chemotherapySchema);
        void InsertMany(IEnumerable<ChemotherapySchema> chemotherapySchemas);
        void Delete(int id);
        ChemotherapySchema GetById(int id);
        ChemotherapySchema GetSchemaDefinition(int id);
        List<ChemotherapySchema> GetAll(ChemotherapySchemaFilter chemotherapySchemaFilter);
        long GetAllFilteredCount(ChemotherapySchemaFilter chemotherapySchemaFilter);
        int GetAllCount();
        IQueryable<ChemotherapySchema> FilterByName(string name);
        byte[] GetRowVersion(int id);
    }
}
