using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Implementations
{
    public class CustomEnumDAL : ICustomEnumDAL
    {
        private SReportsContext context;
        public CustomEnumDAL(SReportsContext context)
        {
            this.context = context;
        }

        public void Delete(CustomEnum customEnum)
        {
            context.Entry(customEnum).Entity.IsDeleted = true;
            context.SaveChanges();
        }

        public IQueryable<CustomEnum> GetAll()
        {
            return context.CustomEnums.Where(x => !x.IsDeleted)
                .Include(x => x.ThesaurusEntry)
                .Include(x => x.ThesaurusEntry.Translations)
                .Include(x => x.Organization);
        }

        public void Insert(CustomEnum customEnum)
        {
            if (customEnum.Id == 0)
            {
                customEnum.EntryDatetime = DateTime.Now;
                context.CustomEnums.Add(customEnum);
            }
            customEnum.LastUpdate = DateTime.Now;
            context.SaveChanges();
        }
    }
}
