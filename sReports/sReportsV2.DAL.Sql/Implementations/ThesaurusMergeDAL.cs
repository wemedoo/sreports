using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ThesaurusMergeDAL : IThesaurusMergeDAL
    {
        private SReportsContext context;

        public ThesaurusMergeDAL(SReportsContext context)
        {
            this.context = context;
        }
        public List<ThesaurusMerge> GetAllByState(ThesaurusMergeState state)
        {
            return context.ThesaurusMerge.Where(x => !x.IsDeleted && x.State == state).ToList();
        }

        public void InsertOrUpdate(ThesaurusMerge thesaurusMerge)
        {
            if (thesaurusMerge.ThesaurusMergeId == 0)
            {
                context.ThesaurusMerge.Add(thesaurusMerge);
            }
            else
            {
                thesaurusMerge.SetLastUpdate();
            }
            context.SaveChanges();
        }
    }
}
