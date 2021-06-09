using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Interfaces
{
    public interface ICustomEnumDAL
    {
        void Insert(CustomEnum customEnum);
        void Delete(CustomEnum customEnum);
        IQueryable<CustomEnum> GetAll();
    }
}
