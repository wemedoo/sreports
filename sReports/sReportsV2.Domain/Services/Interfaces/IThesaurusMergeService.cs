using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IThesaurusMergeService
    {
        void InsertOrUpdate(ThesaurusMerge thesaurus);
        List<ThesaurusMerge> GetAllByState(ThesaurusMergeState state);
    }
}
