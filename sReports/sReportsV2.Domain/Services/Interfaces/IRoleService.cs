using sReportsV2.Domain.Entities.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IRoleService
    {
        List<Roles> GetAll();
    }
}
