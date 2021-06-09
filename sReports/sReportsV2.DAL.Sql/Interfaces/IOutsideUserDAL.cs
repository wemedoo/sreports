using sReportsV2.Domain.Sql.Entities.OutsideUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IOutsideUserDAL
    {
        int InsertOrUpdate(OutsideUser user);
        void Delete(int id);
        OutsideUser GetById(int id);
        List<OutsideUser> GetAllByIds(List<int> ids);
    }
}
