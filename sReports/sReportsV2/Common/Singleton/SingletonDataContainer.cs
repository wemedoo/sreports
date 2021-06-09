using AutoMapper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Entities.Role;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.ThesaurusEntry.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sReportsV2.DTOs.DTOs.User.DTO;
using sReportsV2.DTOs.CustomEnum.DataOut;
using sReportsV2.DAL.Sql.Interfaces;
using System.Web.Mvc;
using sReportsV2.Domain.Sql.Entities.Common;

namespace sReportsV2.Common.Singleton
{
    sealed class SingletonDataContainer
    {
        private static SingletonDataContainer instance;
        private readonly List<EnumDTO> languages = new List<EnumDTO>();
        private List<CustomEnumDataOut> enums = new List<CustomEnumDataOut>();
        private List<CodeSystemDataOut> codeSystems;
        private List<string> roles = new List<string>();

        private SingletonDataContainer()
        {
            this.PopulateLanguages();
            this.PopulateCodeSystems();
            this.PopulateRoles();
            this.PopulateEnums();
        }

        public static SingletonDataContainer Instance
        {
            get
            {
                if(instance == null)
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

        public List<EnumDTO> GetLanguages()
        {
            return this.languages;
        }

        public List<CodeSystemDataOut> GetCodeSystems()
        {
            return this.codeSystems;
        }

        public List<RoleDTO> GetRoles()
        {
            return new List<RoleDTO>()
            {
                new RoleDTO()
                {
                    Id = 1,
                    Name = "Administrator"
                },
                new RoleDTO()
                {
                    Id = 2,
                    Name = "Doctor"
                }
            };
        }

        public List<CustomEnumDataOut> GetEnums()
        {
            return this.enums;
        }

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

        private void PopulateCodeSystems()
        {
            ICodeSystemDAL codeSystemDAL = DependencyResolver.Current.GetService<ICodeSystemDAL>();
            this.codeSystems = Mapper.Map<List<CodeSystemDataOut>>(codeSystemDAL.GetAll());
        }

        private void PopulateRoles()
        {
            this.roles = this.PopulateRole(this.roles, new RoleService());
        }

        private void PopulateEnums()
        {
            this.enums = this.PopulateEnum(this.enums);
        }

        private List<CustomEnumDataOut> PopulateEnum(List<CustomEnumDataOut> listToPopulate)
        {
            var customEnumDal = DependencyResolver.Current.GetService<ICustomEnumDAL>();
            List<CustomEnum> enums = customEnumDal.GetAll().ToList();


            return Mapper.Map<List<CustomEnumDataOut>>(enums);
        }

        private List<string> PopulateRole(List<string> listToPopulate, IRoleService service)
        {
            List<Roles> allRoles = service.GetAll();
            List<string> roles = new List<string>();
            roles = allRoles.Select(x => x.Role).ToList();

            foreach (string role in roles)
            {
                listToPopulate.Add(role);
            }

            return listToPopulate;
        }

        public CustomEnumDataOut MockEnumDataOut(string label, string definition, int id)
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