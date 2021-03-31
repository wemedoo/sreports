using AutoMapper;
using sReportsV2.MapperProfiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.Configs
{
    public static class AutoMapperWebConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile<FormProfile>();
                cfg.AddProfile<FormInstanceProfile>();
                cfg.AddProfile<UserProfile>();
                cfg.AddProfile<ThesaurusEntryProfile>();
                cfg.AddProfile<DocumentPropertiesProfile>();
                cfg.AddProfile<PatientProfile>();
                cfg.AddProfile<UmlsProfile>();
                cfg.AddProfile<EpisodeOfCareProfile>();
                cfg.AddProfile<EncounterProfile>();
                cfg.AddProfile<OrganizationProfile>();
                cfg.AddProfile<FormDistributionProfile>();
                cfg.AddProfile<FieldProfile>();
            });
        }
    }
}