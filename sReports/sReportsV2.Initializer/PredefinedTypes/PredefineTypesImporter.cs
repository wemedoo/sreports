using sReportsV2.Common.Enums;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.SqlDomain.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Initializer.PredefinedTypes
{
    public class PredefineTypesImporter
    {
        private readonly IThesaurusDAL thesaurusDAL;
        private readonly ICustomEnumDAL customEnumDAL;
        public PredefineTypesImporter() { }

        public PredefineTypesImporter(ICustomEnumDAL customEnumDAL, IThesaurusDAL thesaurusDAL) 
        {
            this.thesaurusDAL = thesaurusDAL;
            this.customEnumDAL = customEnumDAL;
        }

        public void Import() 
        {
            if (customEnumDAL.GetAll().Count() == 0) 
            {
                List<string> serviceTypes = ReadAllServiceTypes();
                InsertData(serviceTypes, CustomEnumType.ServiceType);
                InsertData(PredefinedTypesConstants.OrganizationType, CustomEnumType.OrganizationType);
                InsertData(PredefinedTypesConstants.EpisodeOfCareType, CustomEnumType.EpisodeOfCareType);
                InsertData(PredefinedTypesConstants.EncounterType, CustomEnumType.EncounterType);
                InsertData(PredefinedTypesConstants.EncounterStatus, CustomEnumType.EncounterStatus);
                InsertData(PredefinedTypesConstants.EncounterClassification, CustomEnumType.EncounterClassification);
                InsertData(PredefinedTypesConstants.DiagnosisRole, CustomEnumType.DiagnosisRole);
                InsertData(PredefinedTypesConstants.PatientIdentifierType, CustomEnumType.PatientIdentifierType);
                InsertData(PredefinedTypesConstants.OrganizationIdentifierType, CustomEnumType.OrganizationIdentifierType);
            }
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

                int id = thesaurusDAL.InsertOrUpdate(thesaurus);
                customEnumDAL.Insert(new Domain.Sql.Entities.Common.CustomEnum()
                {
                    ThesaurusEntryId = id,
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
