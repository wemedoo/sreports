using AutoMapper;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.ThesaurusEntry.DTO;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.DTOs.CustomEnum.DataOut;
using sReportsV2.DAL.Sql.Interfaces;
using System.Web.Mvc;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.DTOs.DTOs.SmartOncology.Enum.DataOut;
using ExcelImporter.Importers;
using sReportsV2.Common.Localization;
using System;

namespace sReportsV2.Common.Singleton
{
    sealed class SingletonDataContainer
    {
        private static SingletonDataContainer instance;
        private readonly List<EnumDTO> languages = new List<EnumDTO>();
        private List<CustomEnumDataOut> enums = new List<CustomEnumDataOut>();
        private List<SmartOncologyEnumDataOut> smartOncologyEnums = new List<SmartOncologyEnumDataOut>();
        private List<CodeSystemDataOut> codeSystems;
        private readonly List<EnumDTO> patientLanguages = new List<EnumDTO>();
        private SingletonDataContainer()
        {
            this.PopulateLanguages();
            this.PopulateCodeSystems();
            this.PopulateEnums();
            this.PopulateSmartOncologyEnums();
            this.PopulatePatientLanguages();
        }

        public static SingletonDataContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SingletonDataContainer();
                }
                return instance;
            }
        }

        public void RefreshSingleton()
        {
            instance = new SingletonDataContainer();
        }

        public void RefreshSingleton(string updatedThesaurusIdStr)
        {
            if(int.TryParse(updatedThesaurusIdStr, out int updatedThesaurusId) && GetEnums().Any(e => e.Thesaurus.Id == updatedThesaurusId))
            {
                RefreshSingleton();
            }
        }

        public List<EnumDTO> GetLanguages()
        {
            return this.languages;
        }

        public List<EnumDTO> GetPatientLanguages()
        {
            return this.patientLanguages;
        }

        public List<CodeSystemDataOut> GetCodeSystems()
        {
            return this.codeSystems;
        }

        #region Common CustomEnum methods

        public List<CustomEnumDataOut> GetEnums()
        {
            return this.enums;
        }

        public CustomEnumDataOut GetCustomEnum(int customEnumId)
        {
            return GetEnums().FirstOrDefault(x => x.Id == customEnumId);
        }

        public string GetCustomEnumPreferredTerm(int customEnumId)
        {
            string preferredTerm = string.Empty;
            CustomEnumDataOut customEnumData = GetCustomEnum(customEnumId);
            if (customEnumData != null)
            {
                preferredTerm = customEnumData.Thesaurus.GetPreferredTermByTranslationOrDefault("en");
            }
            return preferredTerm;
        }


        public List<CustomEnumDataOut> GetEnumsByType(CustomEnumType type)
        {
            return GetEnums().Where(x => x.Type.Equals(type)).ToList();
        }

        public int GEtCustomEnumId(string term, CustomEnumType type)
        {
            return GetEnumsByType(type).Where(e => e.Thesaurus.GetPreferredTermByTranslationOrDefault("en") == term).Select(en => en.Id).FirstOrDefault();
        }

        #endregion /Common CustomEnum methods

        #region Digital Guideline methods

        public List<CustomEnumDataOut> GetNCCNCategoriesOfEvidenceAndConsensus()
        {
            return new List<CustomEnumDataOut>()
            {
                MockEnumDataOut("Category 1", "Category 1", 50000),
                MockEnumDataOut("Category 2A", "Category 2A", 50001),
                MockEnumDataOut("Category 2B", "Category 2B", 50002),
                MockEnumDataOut("Category 3", "Category 3", 50003)
            };
        }

        public List<CustomEnumDataOut> GetNCCNCategoriesOfPreference()
        {
            return new List<CustomEnumDataOut>()
            {
                MockEnumDataOut("Preferred Intervention", "Preferred Intervention", 50004),
                MockEnumDataOut("Other recommend intervention", "Other recommend intervention", 50005),
                MockEnumDataOut("Useful in certain circumstances", "Useful in certain circumstances", 50006)
            };
        }

        public List<CustomEnumDataOut> GetOxfordLevelOfEvidenceSystem()
        {
            return new List<CustomEnumDataOut>()
            {
                MockEnumDataOut("1a", "Preferred Intervention", 50007),
                MockEnumDataOut("1b", "Other recommend intervention", 50008),
                MockEnumDataOut("1c", "All or none randomized controlled trials", 50009),
                MockEnumDataOut("2a", "All or none randomized controlled trials", 500010),
                MockEnumDataOut("2b", "Individual cohort study or low quality randomized controlled trials (e.g. < 80% follow up)", 500011),
                MockEnumDataOut("2c", "Outcomes Research: ecological studies", 500012),
                MockEnumDataOut("3a", "Systematic review (with homogenity) of case-control studies", 500013),
                MockEnumDataOut("4", "Case series(and poor quality cohort and case-control studies)", 500014),
                MockEnumDataOut("5", "Expert opinion without explicit critical appraisal, or based on physiology, bench reasearch or first principles", 500015),
                MockEnumDataOut("3b", "Individual case controll study", 500016)
            };
        }

        public List<CustomEnumDataOut> GetStrengthOfRecommendation()
        {
            return new List<CustomEnumDataOut>()
            {
                MockEnumDataOut("HIGH","There is a lot of confidence that the true effect lies close to that of the estimated effect",50016),
                MockEnumDataOut("MODERATE","There is moderate confidence in the estimated effect: The true effect is likely to be close to the estimated effect" +
                ", but there is apossiblity that it is substantially different",50017),
                MockEnumDataOut("LOW","There is limited effect in the estimated effect: The true effect might be substantially different from the estimated effect",50018),
                MockEnumDataOut("VERY LOW","There is very little confidence in the estimated effect: The true effect islikely to be substantially different from the estimated effect",50018),
            };
        }

        #endregion /Digital Guideline methods

        public List<SmartOncologyEnumDataOut> GetSmartOncologyEnums(string type)
        {
            return this.smartOncologyEnums.Where(e => e.Type == type).ToList();
        }

        private void PopulateLanguages()
        {
            languages.Add(new EnumDTO() { Label = "Serbian", Value = "sr" });
            languages.Add(new EnumDTO() { Label = "Serbian Cyrilic", Value = "sr-Cyrl-RS" });
            languages.Add(new EnumDTO() { Label = "English", Value = "en" });
            languages.Add(new EnumDTO() { Label = "French", Value = "fr" });
            languages.Add(new EnumDTO() { Label = "German", Value = "de" });
            languages.Add(new EnumDTO() { Label = "Spanish", Value = "es" });
            languages.Add(new EnumDTO() { Label = "Italian", Value = "it" });
            languages.Add(new EnumDTO() { Label = "Russian", Value = "ru" });
            languages.Add(new EnumDTO() { Label = "Portuguese", Value = "pt" });
        }

        private void PopulatePatientLanguages() 
        {
            foreach (SpokenLang lang in Enum.GetValues(typeof(SpokenLang)))
            {
                patientLanguages.Add(new EnumDTO() { Label = lang.ToString(), Value = lang.GetLangAttribute().Iso6391 });
            }
        }

        private void PopulateCodeSystems()
        {
            ICodeSystemDAL codeSystemDAL = DependencyResolver.Current.GetService<ICodeSystemDAL>();
            this.codeSystems = Mapper.Map<List<CodeSystemDataOut>>(codeSystemDAL.GetAll().OrderBy(x => x.Label));
        }

        private void PopulateEnums()
        {
            var customEnumDal = DependencyResolver.Current.GetService<ICustomEnumDAL>();
            List<CustomEnum> enums = customEnumDal.GetAll().ToList();

            this.enums = Mapper.Map<List<CustomEnumDataOut>>(enums);
        }

        private void PopulateSmartOncologyEnums()
        {
            var presentationStage = GetEnumsFromExcel(SmartOncologyEnumNames.sReportsVocabularyFileName, SmartOncologyEnumNames.DiagnosesSheet, SmartOncologyEnumNames.PresentationStage);
            var anatomy = GetEnumsFromExcel(SmartOncologyEnumNames.sReportsVocabularyFileName, SmartOncologyEnumNames.DiagnosesSheet, SmartOncologyEnumNames.Anatomy);
            var morphology = GetEnumsFromExcel(SmartOncologyEnumNames.sReportsVocabularyFileName, SmartOncologyEnumNames.DiagnosesSheet, SmartOncologyEnumNames.Morphology);
            var therapeuticContext = GetEnumsFromExcel(SmartOncologyEnumNames.sReportsVocabularyFileName, SmartOncologyEnumNames.TherapyCategorizationSheet, SmartOncologyEnumNames.TherapeuticContext);
            var chemotherapyType = GetEnumsFromExcel(SmartOncologyEnumNames.ChemotherapyCompendiumFileName, SmartOncologyEnumNames.ChemotherapySchemasSheet, SmartOncologyEnumNames.ChemotherapyType);

            List<SmartOncologyEnumDataOut> newEnums = new List<SmartOncologyEnumDataOut>();
            newEnums.AddRange(presentationStage);
            newEnums.AddRange(anatomy);
            newEnums.AddRange(morphology);
            newEnums.AddRange(therapeuticContext);
            newEnums.AddRange(chemotherapyType);
           
            this.smartOncologyEnums = newEnums;

        }

        private List<SmartOncologyEnumDataOut> GetEnumsFromExcel(string fileName, string sheetName, string columnName)
        {
            List<SmartOncologyEnumDataOut> enums = new SchemaColumnImporter(fileName, sheetName, columnName).GetEnumsFromExcel();

            return enums;
        }

        private CustomEnumDataOut MockEnumDataOut(string label, string definition, int id)
        {
            return new CustomEnumDataOut()
            {
                Id = 10,
                Thesaurus = new ThesaurusEntryDataOut()
                {
                    Id = id,
                    Translations = new List<ThesaurusEntryTranslationDTO>()
                    {
                       new ThesaurusEntryTranslationDTO()
                       {
                           Language = "en",
                           PreferredTerm = label,
                           Definition = definition
                       }
                    }
                }
            };
        }
    }
}