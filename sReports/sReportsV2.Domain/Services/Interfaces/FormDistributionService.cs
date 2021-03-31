using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IFormDistributionService
    {
        IQueryable<FormDistribution> GetAll(int page, int pageSize);
        int GetAllCount();

        FormDistribution GetById(string id);
        FormDistribution GetByThesaurusId(string id);
        FormDistribution InsertOrUpdate(FormDistribution formDistribution);
    }
}
