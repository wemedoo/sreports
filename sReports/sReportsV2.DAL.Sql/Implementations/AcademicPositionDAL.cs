using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class AcademicPositionDAL : IAcademicPositionDAL
    {

        private readonly SReportsContext context;
        public AcademicPositionDAL(SReportsContext context)
        {
            this.context = context;
        }

        public int Count()
        {
            return context.AcademicPositionTypes.Count();
        }

        public void Insert(AcademicPositionType academicPositionType)
        {
            this.context.AcademicPositionTypes.Add(academicPositionType);
            context.SaveChanges();
        }
    }
}
