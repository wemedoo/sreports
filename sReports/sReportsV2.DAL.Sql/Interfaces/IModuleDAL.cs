using sReportsV2.Domain.Sql.Entities.AccessManagment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IModuleDAL
    {
        void InsertMany(List<Module> modules);
        int Count();
        List<Module> GetAll();
        Module GetByName(string name);
    }
}
