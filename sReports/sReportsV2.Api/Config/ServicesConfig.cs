using Microsoft.Extensions.DependencyInjection;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.Config
{
    public class ServicesConfig
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            /*services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IEpisodeOfCareService, EpisodeOfCareService>();
            services.AddScoped<IEncounterService, EncounterService>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IFormService, FormService>();
            services.AddScoped<IFormInstanceService, FormInstanceService>();*/
        }
    }
}
