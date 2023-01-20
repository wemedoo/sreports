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
        [Column("OrganizationId")]
        public int OrganizationId { get; set; }  
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
        public string Impressum { get; set; }
        public int NumOfUsers { get; set; }
        public int? AddressId { get; set; }
        public virtual int? OrganizationRelationId { get; set; }
        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }
        [ForeignKey("OrganizationRelationId")]
        public virtual OrganizationRelation OrganizationRelation { get; set; }
        public virtual List<OrganizationClinicalDomain> ClinicalDomains { get; set; } = new List<OrganizationClinicalDomain>();
        public virtual List<Identifier> Identifiers { get; set; }
        public virtual List<Telecom> Telecoms { get; set; }

        
        //{
        //    get
        //    {
        //        return Impressum;
        //    }
        //    set
        //    {
        //        if (value.Length > 600)
        //            value = value.Substring(0, 600);
        //        Impressum = value;
        //    }
        //}

        public void CopyData(Organization copyFrom)
        {
            this.Alias = copyFrom.Alias;
            this.Description = copyFrom.Description;
            this.Email = copyFrom.Email;
            this.OrganizationId = copyFrom.OrganizationId;
            this.IsDeleted = copyFrom.IsDeleted;
            this.LogoUrl = copyFrom.LogoUrl;
            this.Name = copyFrom.Name;
            this.PrimaryColor = copyFrom.PrimaryColor;
            this.RowVersion = copyFrom.RowVersion;
            this.SecondaryColor = copyFrom.SecondaryColor;
            this.Type = copyFrom.Type;
            this.Impressum = copyFrom.Impressum;
            CopyAddress(copyFrom.Address);
            SetTelecoms(copyFrom.Telecoms);
            SetIdentifiers(copyFrom.Identifiers);
        }

        private void SetIdentifiers(List<Identifier> identifiers)
        {
            foreach (var identifier in identifiers.Where(x => x.IdentifierId == 0).ToList())
            {
                this.Identifiers.Add(identifier);
            }
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
                    Child = this,
                    ParentId = parentId.GetValueOrDefault()
                };
            }
            else
            {
                this.OrganizationRelation.ParentId = parentId.GetValueOrDefault();
                this.OrganizationRelation.SetLastUpdate();
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
                        OrganizationId = this.OrganizationId
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

            this.Address.Copy(copyFrom);
        }

        private void SetTelecoms(List<Telecom> telecoms)
        {
            foreach (var telecom in telecoms.Where(x => x.TelecomId == 0).ToList())
            {
                this.Telecoms.Add(telecom);
            }
        }
    }
}
