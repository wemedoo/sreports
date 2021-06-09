using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Common.Extensions;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.CustomEnum;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class CustomEnumBLL : ICustomEnumBLL
    {
        private readonly ICustomEnumDAL customEnumDAL;

        public CustomEnumBLL(ICustomEnumDAL customEnumDAL)
        {
            this.customEnumDAL = customEnumDAL;
        }

        public void Delete(CustomEnumDataIn customEnumDataIn)
        {
            try
            {
                customEnumDAL.Delete(Mapper.Map<CustomEnum>(customEnumDataIn));
            }
            catch(DbUpdateConcurrencyException ex)
            {

            }
        }

        public void Insert(CustomEnumDataIn enumDataIn, int activeOrganization)
        {
            enumDataIn = Ensure.IsNotNull(enumDataIn, nameof(enumDataIn));

            CustomEnum entry = Mapper.Map<CustomEnum>(enumDataIn);
            entry.OrganizationId = activeOrganization;
            customEnumDAL.Insert(entry);
        }

        public Dictionary<string, List<EnumData>> GetDocumentPropertiesEnums()
        {
            Dictionary<string, List<EnumData>> result = new Dictionary<string, List<EnumData>>();
            result["Classes"] = GetDocumentClassesList();
            result["GeneralPurpose"] = GetDocumentGeneralPurposesList();
            result["ExplicitPurpose"] = GetDocumentExplicitPurposesList();
            result["ContextDependent"] = GetDocumentContextDependentsList();
            result["ScopeOfValidity"] = GetDocumentScopeOfValidityList();
            result["ClinicalDomain"] = GetDocumentClinicalDomainList();
            result["ClinicalContext"] = GetDocumentClinicalContextList();
            result["AdministrativeContext"] = GetDocumentAdministrativeContextList();
            result["FollowUp"] = GetDocumentFollowUpList();
            return result;
        }

        private List<EnumData> GetDocumentClassesList()
        {
            return Enum.GetValues(typeof(DocumentClassEnum)).Cast<DocumentClassEnum>()
                .Select(x =>
                new EnumData
                {
                    Value = x.ToString(),
                    Label = x.ToString()
                })
                .ToList();
        }

        private List<EnumData> GetDocumentGeneralPurposesList()
        {
            return Enum.GetValues(typeof(DocumentGeneralPurposeEnum)).Cast<DocumentGeneralPurposeEnum>()
                .Select(x =>
                new EnumData
                {
                    Value = x.ToString(),
                    Label = x.ToString()
                })
                .ToList();
        }

        private List<EnumData> GetDocumentExplicitPurposesList()
        {
            return Enum.GetValues(typeof(DocumentExplicitPurpose)).Cast<DocumentExplicitPurpose>()
                .Select(x =>
                new EnumData
                {
                    Value = x.ToString(),
                    Label = x.ToString()
                })
                .ToList();
        }

        private List<EnumData> GetDocumentContextDependentsList()
        {
            return Enum.GetValues(typeof(ContextDependent)).Cast<ContextDependent>()
                .Select(x =>
                new EnumData
                {
                    Value = x.ToString(),
                    Label = x.ToString()
                })
                .ToList();
        }

        private List<EnumData> GetDocumentScopeOfValidityList()
        {
            return Enum.GetValues(typeof(DocumentScopeOfValidityEnum)).Cast<DocumentScopeOfValidityEnum>()
                .Select(x =>
                new EnumData
                {
                    Value = x.ToString(),
                    Label = x.ToString()
                })
                .ToList();
        }

        private List<EnumData> GetDocumentClinicalDomainList()
        {
            return Enum.GetValues(typeof(DocumentClinicalDomain)).Cast<DocumentClinicalDomain>()
                .Select(x =>
                new EnumData
                {
                    Value = x.ToString(),
                    Label = x.ToString()
                })
                .ToList();
        }

        private List<EnumData> GetDocumentClinicalContextList()
        {
            return Enum.GetValues(typeof(DocumentClinicalContextEnum)).Cast<DocumentClinicalContextEnum>()
                .Select(x =>
                new EnumData
                {
                    Value = x.ToString(),
                    Label = x.ToString()
                })
                .ToList();
        }

        private List<EnumData> GetDocumentAdministrativeContextList()
        {
            return Enum.GetValues(typeof(AdministrativeContext)).Cast<AdministrativeContext>()
                .Select(x =>
                new EnumData
                {
                    Value = x.ToString(),
                    Label = x.ToString()
                })
                .ToList();
        }

        private List<EnumData> GetDocumentFollowUpList()
        {
            return Enum.GetValues(typeof(FollowUp)).Cast<FollowUp>()
                .Select(x =>
                new EnumData
                {
                    Value = x.ToString(),
                    Label = x.ToString()
                })
                .ToList();
        }


    }
}
