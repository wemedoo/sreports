using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IOutsideUserService
    {
        string InsertOrUpdate(OutsideUser user);
        bool Delete(string id);
        OutsideUser GetById(string id);
        List<OutsideUser> GetAllByIds(List<int> ids);
    }
}
