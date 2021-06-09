using sReportsV2.Domain.Sql.Entities.CodeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Interfaces
{
    public interface ICodeSystemDAL
    {
        int GetAllCount();
        void InsertMany(List<CodeSystem> codeSystems);
        List<CodeSystem> GetAll();
    }
}
