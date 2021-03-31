using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class EnumService : IEnumService
    {
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
                    Label = x.ToString()})
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
