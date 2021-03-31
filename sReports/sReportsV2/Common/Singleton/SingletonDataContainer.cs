using AutoMapper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.Role;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.Domain.Enums;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Encounter.DataOut;
using sReportsV2.DTOs.Patient;
using sReportsV2.Models.Common;
using sReportsV2.Models.ThesaurusEntry;
using sReportsV2.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Common.Singleton
{
    sealed class SingletonDataContainer
    {
        private static SingletonDataContainer instance;
        private readonly List<EnumViewModel> languages = new List<EnumViewModel>();
        private List<IdentifierTypeDataOut> patientIdentifierTypes;
        private List<IdentifierTypeDataOut> organizationIdentifierTypes;
        private List<ServiceTypeDTO> serviceTypes;
        private List<CodeSystemDataOut> codeSystems;
        private List<EnumDataOut> encounterTypes = new List<EnumDataOut>();
        private List<EnumDataOut> encounterStatuses = new List<EnumDataOut>();
        private List<EnumDataOut> encounterClassification = new List<EnumDataOut>();
        private List<EnumDataOut> diagnosisRoles = new List<EnumDataOut>();
        private List<EnumDataOut> organizationTypes = new List<EnumDataOut>();
        private List<EnumDataOut> episodeOfCareTypes = new List<EnumDataOut>();

        private List<string> roles = new List<string>();
        private IThesaurusEntryService thesaurusService = new ThesaurusEntryService();

        private SingletonDataContainer()
        {
            this.PopulateLanguages();
            this.PopulatePatientIdentifierTypes();
            this.PopulateOrganizationIdentifierTypes();
            this.PopulateEncounterServiceTypes();
            this.PopulateCodeSystems();
            this.PopulateEncounterTypes();
            this.PopulateEncounterStatuses();
            this.PopulateEncounterClassifications();
            this.PopulateDiagnosisRole();
            this.PopulateOrganizationTypes();
            this.PopulateRoles();
            this.PopulateEpisodeOfCareTypes();


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

        public string GetServiceTypeByThesaurus(string thesaurusId) 
        {
            return serviceTypes.FirstOrDefault(x => x.ThesaurusId.Equals(thesaurusId)).Display;
        }

        public List<EnumViewModel> GetLanguages()
        {
            return this.languages;
        }

        public List<IdentifierTypeDataOut> GetIdentifierTypes()
        {          
            return this.patientIdentifierTypes;
        }
        public List<IdentifierTypeDataOut> GetOrganizationIdentifierTypes()
        {

            return this.organizationIdentifierTypes;
        }
        public List<ServiceTypeDTO> GetServiceTypes()
        {
            return this.serviceTypes;
        }

        public List<CodeSystemDataOut> GetCodeSystems()
        {
            return this.codeSystems;
        }
        public List<EnumDataOut> GetEncounterStatus()
        {
            return this.encounterStatuses;
        }
        public List<EnumDataOut> GetEncounterClassification()
        {
            return this.encounterClassification;
        }
        public List<EnumDataOut> GetEncounterTypes()
        {
            return this.encounterTypes;
        }
        public List<EnumDataOut> GetDiagnosisRoles()
        {
            return this.diagnosisRoles;
        }
        public List<string> GetRoles()
        {
            return this.roles;
        }
        public List<EnumDataOut> GetOrganizationTypes()
        {
            return this.organizationTypes;
        }
        public List<EnumDataOut> GetEpisodeOfCareTypes()
        {
            return this.episodeOfCareTypes;
        }
        private void PopulateLanguages()
        {
            languages.Add(new EnumViewModel() { Label = "Serbian", Value = "sr" });
            languages.Add(new EnumViewModel() { Label = "Serbian Cyrilic", Value = "sr-Cyrl-RS" });
            languages.Add(new EnumViewModel() { Label = "English", Value = "en" });
            languages.Add(new EnumViewModel() { Label = "French", Value = "fr" });
            languages.Add(new EnumViewModel() { Label = "German", Value = "de" });
            languages.Add(new EnumViewModel() { Label = "Spanish", Value = "es" });
            languages.Add(new EnumViewModel() { Label = "Italian", Value = "it" });
            languages.Add(new EnumViewModel() { Label = "Russian", Value = "ru" });
            languages.Add(new EnumViewModel() { Label = "Portuguese", Value = "pt" });
        }

        private void PopulatePatientIdentifierTypes()
        {
            PatientService service = new PatientService();
            this.patientIdentifierTypes = Mapper.Map<List<IdentifierTypeDataOut>>(service.GetIdentifierTypes(IdentifierKind.Patient));
        }

        private void PopulateCodeSystems()
        {
            CodeSystemService service = new CodeSystemService();
            this.codeSystems = Mapper.Map<List<CodeSystemDataOut>>(service.GetAll());
        }
       
        private void PopulateOrganizationIdentifierTypes()
        {
            PatientService service = new PatientService();
            this.organizationIdentifierTypes = Mapper.Map<List<IdentifierTypeDataOut>>(service.GetIdentifierTypes(IdentifierKind.Organization));
        }

        private void PopulateEncounterServiceTypes()
        {
            EncounterService service = new EncounterService();
            this.serviceTypes = Mapper.Map<List<ServiceTypeDTO>>(service.GetAllServiceTypes());
        }

        private void PopulateEncounterStatuses()
        {
            this.encounterStatuses = this.PopulateEnum(this.encounterStatuses, new EncounterStatusService());
        }

        private void PopulateEncounterClassifications()
        {
            this.encounterClassification = this.PopulateEnum(this.encounterClassification, new EncounterClassificationService());
        }

        private void PopulateEncounterTypes()
        {
            this.encounterTypes = this.PopulateEnum(this.encounterTypes, new EncounterTypeSevice());
        }

        private void PopulateEpisodeOfCareTypes()
        {
            this.episodeOfCareTypes = this.PopulateEnum(this.episodeOfCareTypes, new EpisodeOfCareTypeService());
        }

        private void PopulateDiagnosisRole()
        {
            this.diagnosisRoles = this.PopulateEnum(this.diagnosisRoles, new DiagnosisRoleService());
        }

        private void PopulateRoles()
        {
            this.roles = this.PopulateRole(this.roles, new RoleService());
        }

        private void PopulateOrganizationTypes()
        {
            this.organizationTypes =  this.PopulateEnum(this.organizationTypes, new OrganizationTypeService());
        }

        private List<EnumDataOut> PopulateEnum(List<EnumDataOut> listToPopulate, IEnumCommonService service) 
        {
            List<EnumEntry> enums = service.GetAll();
            List<string> thesaurusIds = new List<string>();
            thesaurusIds = enums.Select(x => x.ThesaurusId).ToList();

            foreach (ThesaurusEntry thesaurus in this.thesaurusService.GetByIdsList(thesaurusIds))
            {
                listToPopulate.Add(new EnumDataOut() { Thesaurus = Mapper.Map<ThesaurusEntryViewModel>(thesaurus), Label = thesaurus.GetPreferredTermByTranslationOrDefault("en") });
            }

            return listToPopulate.OrderBy(x => x.Label).ToList();
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

    }
}