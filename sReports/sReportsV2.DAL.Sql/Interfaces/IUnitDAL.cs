using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IUnitDAL
    {
        void InsertMany(List<Unit> units);
        int GetAllCount();
        List<Unit> GetAll();
        Unit GetById(int id);
        int? GetByNameId(string name);
        IQueryable<Unit> FilterByName(string name);
    }
}
