using Newtonsoft.Json;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.OrganizationEntities
{
    public class Organization : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }  
        [NotMapped]
        public List<int> Type { get; set; }
        public string TypesString
        {
            get
            {
                return this.Type == null || !this.Type.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.Type);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.Type = new List<int>();
                else
                    this.Type = JsonConvert.DeserializeObject<List<int>>(value);
            }

        }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string PrimaryColor { get; set; }
        public string SecondaryColor { get; set; }
        public string LogoUrl { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }
        public int NumOfUsers { get; set; }
        public int? AddressId { get; set; }
        public virtual int? OrganizationRelationId { get; set; }
        public virtual Address Address { get; set; }
        public virtual OrganizationRelation OrganizationRelation { get; set; }
        public virtual List<OrganizationClinicalDomain> ClinicalDomains { get; set; }
        public virtual List<Identifier> Identifiers { get; set; }
        public virtual List<Telecom> Telecoms { get; set; }
        
        public void CopyData(Organization copyFrom)
        {
            this.RowVersion = copyFrom.RowVersion;
            this.Alias = copyFrom.Alias;
            this.Description = copyFrom.Description;
            this.Email = copyFrom.Email;
            this.Id = copyFrom.Id;
            this.IsDeleted = copyFrom.IsDeleted;
            this.LogoUrl = copyFrom.LogoUrl;
            this.Name = copyFrom.Name;
            this.PrimaryColor = copyFrom.PrimaryColor;
            this.RowVersion = copyFrom.RowVersion;
            this.SecondaryColor = copyFrom.SecondaryColor;
            this.Type = copyFrom.Type;
            CopyAddress(copyFrom.Address);
        }

        public void SetRelation(int? parentId)
        {
            if (parentId == this.OrganizationRelation?.ParentId)
            {
                return;
            }

            if (parentId == null)
            {
                this.OrganizationRelation.IsDeleted = true;
                this.OrganizationRelationId = null;
                this.OrganizationRelation = null;
                return;
            }

            if(this.OrganizationRelation == null)
            {
                this.OrganizationRelation = new OrganizationRelation()
                {
                    EntryDatetime = DateTime.Now,
                    LastUpdate = DateTime.Now,
                    Child = this,
                    ParentId = parentId.GetValueOrDefault()
                };
            }
            else
            {
                this.OrganizationRelation.ParentId = parentId.GetValueOrDefault();
                this.OrganizationRelation.LastUpdate = DateTime.Now;
            }
        }

        public void SetClinicalDomains(List<DocumentClinicalDomain> clinicalDomains)
        {
            if(clinicalDomains == null || clinicalDomains.Count() == 0)
            {
                this.ClinicalDomains?.ForEach(x => x.IsDeleted = true);
                return;
            }

            foreach (OrganizationClinicalDomain organizationClinicalDomain in ClinicalDomains)
            {
                if (!clinicalDomains.Contains((DocumentClinicalDomain)organizationClinicalDomain.ClinicalDomainId))
                {
                    organizationClinicalDomain.IsDeleted = true;
                }
            }

            foreach (DocumentClinicalDomain clinicalDomain in clinicalDomains)
            {
                if(!this.ClinicalDomains.Any(x => !x.IsDeleted && x.ClinicalDomainId == (int)clinicalDomain))
                {
                    this.ClinicalDomains.Add(new OrganizationClinicalDomain()
                    {
                        ClinicalDomainId = (int)clinicalDomain,
                        EntryDatetime = DateTime.Now,
                        LastUpdate = DateTime.Now,
                        OrganizationId = this.Id
                    });
                }
            }
        }

        private void CopyAddress(Address copyFrom)
        {
            if (copyFrom == null)
            {
                this.Address = null;
                return;
            }

            if (this.Address == null)
            {
                this.Address = new Address();
            }

            this.Address.Id = copyFrom.Id;
            this.Address.City = copyFrom.City;
            this.Address.Country = copyFrom.Country;
            this.Address.PostalCode = copyFrom.PostalCode;
            this.Address.State = copyFrom.State;
            this.Address.Street = copyFrom.Street;
            this.Address.StreetNumber = copyFrom.StreetNumber;  
        }
    }
}
