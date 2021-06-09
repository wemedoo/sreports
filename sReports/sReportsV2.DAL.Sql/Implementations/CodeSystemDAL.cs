using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class CodeSystemDAL : ICodeSystemDAL
    {
        private SReportsContext context;
        public CodeSystemDAL(SReportsContext context)
        {
            this.context = context;
        }

        public List<CodeSystem> GetAll()
        {
            return context.CodeSystems.ToList();
        }

        public int GetAllCount()
        {
            return context.CodeSystems.Count();
        }

        public void InsertMany(List<CodeSystem> codeSystems)
        {
            foreach (var codeSystem in codeSystems) 
            {
                context.CodeSystems.Add(codeSystem);
            }
            context.SaveChanges();
        }

    }
}
