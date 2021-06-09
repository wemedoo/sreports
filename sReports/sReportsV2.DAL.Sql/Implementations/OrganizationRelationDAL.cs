using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class OrganizationRelationDAL : IOrganizationRelationDAL
    {
        private readonly SReportsContext context;
        public OrganizationRelationDAL(SReportsContext context)
        {
            this.context = context;
        }
        public OrganizationRelation GetRelationByChildId(int childId)
        {
            return this.context.OrganizationRelation.FirstOrDefault(x => !x.IsDeleted && x.ChildId == childId);
        }

        public List<OrganizationRelation> GetOrganizationHierarchies()
        {
            return context.OrganizationRelation.Where(x => !x.IsDeleted)
                .Include(x => x.Child)
                .Include(x => x.Parent)
                .ToList();
        }

        public void UnLinkOrganization(int organizationId, int oldParentId)
        {
            OrganizationRelation organizationRelation = context.OrganizationRelation.FirstOrDefault(x =>!x.IsDeleted && x.ChildId == organizationId && x.ParentId == x.ParentId);
            if(organizationRelation != null)
            {
                organizationRelation.IsDeleted = true;
            }

            context.SaveChanges();
        }

        public void InsertOrUpdate(OrganizationRelation organizationRelation)
        {
            if(organizationRelation.Id == 0)
            {
                organizationRelation.EntryDatetime = DateTime.Now;
                context.OrganizationRelation.Add(organizationRelation);
            }
            else
            {
                context.Entry(organizationRelation).State = EntityState.Modified;
            }
            organizationRelation.LastUpdate = DateTime.Now;
            context.SaveChanges();
        }
    }
}
