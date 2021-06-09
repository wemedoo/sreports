using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IClinicalDomainDAL
    {
        void Insert(ClinicalDomain clinicalDomain);
        int Count();
    }
}
