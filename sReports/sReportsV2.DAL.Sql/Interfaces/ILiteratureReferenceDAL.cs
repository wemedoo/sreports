using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface ILiteratureReferenceDAL
    {
        void InsertOrUpdate(LiteratureReference literatureReference);
        LiteratureReference GetById(int id);
        int? FindByPMID(int PMID);

    }
}
