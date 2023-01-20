using sReportsV2.Domain.Sql.Entities.CodeSystem;
using System.Collections.Generic;

namespace sReportsV2.DAL.Sql.Interfaces
{
    public interface ICodeSystemDAL
    {
        int GetAllCount();
        void InsertOrUpdate(CodeSystem codeSystem);
        void InsertMany(List<CodeSystem> codeSystems);
        List<CodeSystem> GetAll();
        CodeSystem GetBySAB(string SAB);
        CodeSystem GetByValue(string value);
        CodeSystem GetById(int id);
    }
}
