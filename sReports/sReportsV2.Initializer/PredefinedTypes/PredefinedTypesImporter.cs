using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace sReportsV2.Initializer.PredefinedTypes
{
    public class PredefinedTypesImporter
    {
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly ICustomEnumDAL customEnumDAL;
        public PredefinedTypesImporter() { }

        public PredefinedTypesImporter(ICustomEnumDAL customEnumDAL, IThesaurusDAL thesaurusDAL) 
        {
            this.thesaurusDAL = thesaurusDAL;
            this.customEnumDAL = customEnumDAL;
        }

        public void Import() 
        {
            if (!HostingEnvironment.IsDevelopmentEnvironment)
            {
                List<string> serviceTypes = ReadAllServiceTypes();
                UpdateCustomEnums(serviceTypes, CustomEnumType.ServiceType);
                UpdateCustomEnums(PredefinedTypesConstants.OrganizationType, CustomEnumType.OrganizationType);
                UpdateCustomEnums(PredefinedTypesConstants.EpisodeOfCareType, CustomEnumType.EpisodeOfCareType);
                UpdateCustomEnums(PredefinedTypesConstants.EncounterType, CustomEnumType.EncounterType);
                UpdateCustomEnums(PredefinedTypesConstants.EncounterStatus, CustomEnumType.EncounterStatus);
                UpdateCustomEnums(PredefinedTypesConstants.EncounterClassification, CustomEnumType.EncounterClassification);
                UpdateCustomEnums(PredefinedTypesConstants.DiagnosisRole, CustomEnumType.DiagnosisRole);
                UpdateCustomEnums(PredefinedTypesConstants.PatientIdentifierType, CustomEnumType.PatientIdentifierType);
                UpdateCustomEnums(PredefinedTypesConstants.OrganizationIdentifierType, CustomEnumType.OrganizationIdentifierType);
                UpdateCustomEnums(PredefinedTypesConstants.AddressType, CustomEnumType.AddressType);
                UpdateCustomEnums(PredefinedTypesConstants.Citizenship, CustomEnumType.Citizenship);
                UpdateCustomEnums(PredefinedTypesConstants.ReligiousAffiliationType, CustomEnumType.ReligiousAffiliationType);
            }
        }

        private void UpdateCustomEnums(List<string> predefinedTypesConstants, CustomEnumType enumType)
        {
            List<string> terms = new List<string>();
            foreach (var enumTypeConstant in predefinedTypesConstants)
            {
                if (customEnumDAL.GetAll().Where(x => x.ThesaurusEntry.Translations
                    .Select(m => m.PreferredTerm == enumTypeConstant).FirstOrDefault()).Count() == 0)
                {
                    terms.Add(enumTypeConstant);
                }
            }
            InsertData(terms, enumType);
        }

        public void InsertData(List<string> terms, CustomEnumType type) 
        {
            foreach (string term in terms) 
            {
                ThesaurusEntry thesaurus = new ThesaurusEntry()
                {
                    Translations = new List<ThesaurusEntryTranslation>() 
                    {
                        new ThesaurusEntryTranslation()
                        {
                            Language = "en",
                            PreferredTerm = term,
                            Definition = term
                        }
                    }
                };

                thesaurusDAL.InsertOrUpdate(thesaurus);
                customEnumDAL.Insert(new Domain.Sql.Entities.Common.CustomEnum()
                {
                    ThesaurusEntryId = thesaurus.ThesaurusEntryId,
                    OrganizationId = 1,
                    Type = type
                });
            }
        }

        public List<string> ReadAllServiceTypes() 
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            using (var reader = new StreamReader($@"{baseDirectory}\App_Data\serviceTypes.csv"))
            {
                List<string> result = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    result.Add(values[0]);
                }

                return result;
            }
        }
    }
}
