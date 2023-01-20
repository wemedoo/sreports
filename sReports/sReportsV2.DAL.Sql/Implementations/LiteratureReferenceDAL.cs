using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ChemotherapySchema;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class LiteratureReferenceDAL : ILiteratureReferenceDAL
    {
        private readonly SReportsContext context;

        public LiteratureReferenceDAL(SReportsContext context)
        {
            this.context = context;
        }

        public int? FindByPMID(int PMID)
        {
            return context.LiteratureReferences
                .FirstOrDefault(x => x.PubMedID == PMID)?.LiteratureReferenceId;
        }

        public LiteratureReference GetById(int id)
        {
            return context.LiteratureReferences
                .FirstOrDefault(x => x.LiteratureReferenceId == id);
        }

        public void InsertOrUpdate(LiteratureReference literatureReference)
        {
            if (literatureReference.LiteratureReferenceId == 0)
            {
                context.LiteratureReferences.Add(literatureReference);
            }
            context.SaveChanges();
        }
    }
}
