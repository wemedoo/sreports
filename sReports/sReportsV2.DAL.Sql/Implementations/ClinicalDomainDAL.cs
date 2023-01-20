using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Implementations
{
    public class ClinicalDomainDAL : IClinicalDomainDAL
    {
        private readonly SReportsContext context;

        public ClinicalDomainDAL(SReportsContext context)
        {
            this.context = context;
        }

        public int Count()
        {
            return context.ClinicalDomain.Count();
        }

        public void Insert(DocumentClinicalDomain documentClinicalDomain)
        {
            ClinicalDomain clinicalDomainDB = GetById((int)documentClinicalDomain);
            if (clinicalDomainDB == null)
            {
                ClinicalDomain clinicalDomain = new ClinicalDomain()
                {
                    ClinicalDomainId = (int)documentClinicalDomain,
                    Name = documentClinicalDomain.ToString()
                };
                Insert(clinicalDomain);
            }
            else
            {
                clinicalDomainDB.Name = documentClinicalDomain.ToString();
                this.Save();
            }
        }

        private void Insert(ClinicalDomain clinicalDomain)
        {
            context.ClinicalDomain.Add(clinicalDomain);
            this.Save();
        }

        private ClinicalDomain GetById(int id)
        {
            return context.ClinicalDomain.FirstOrDefault(x => x.ClinicalDomainId == id);
        }

        private void Save()
        {
            context.SaveChanges();
        }
    }
}
