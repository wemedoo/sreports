using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IFormDistributionDAL
    {
        IQueryable<FormDistribution> GetAll(int page, int pageSize);
        int GetAllCount();

        FormDistribution GetById(string id);
        FormDistribution GetByThesaurusIdAndVersion(int id, string versionId);
        FormDistribution GetByThesaurusId(int id);
        FormDistribution InsertOrUpdate(FormDistribution formDistribution);
        List<FormDistribution> GetAll();
        List<FormDistribution> GetAllVersionAndThesaurus();
        FormFieldDistribution GetFormFieldDistribution(string formDistributionId, string fieldId);
        bool ThesaurusExist(int thesaurusId);

    }
}
