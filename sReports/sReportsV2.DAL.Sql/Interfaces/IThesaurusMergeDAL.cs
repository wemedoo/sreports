using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Interfaces
{
    public interface IThesaurusMergeDAL
    {
        void InsertOrUpdate(ThesaurusMerge thesaurusMerge);
        List<ThesaurusMerge> GetAllByState(ThesaurusMergeState state);
    }
}
