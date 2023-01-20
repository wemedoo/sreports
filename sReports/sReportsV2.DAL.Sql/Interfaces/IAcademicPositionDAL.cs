using sReportsV2.Domain.Sql.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IAcademicPositionDAL
    {
        void Insert(AcademicPositionType academicPositionType);
        int Count();
    }
}
