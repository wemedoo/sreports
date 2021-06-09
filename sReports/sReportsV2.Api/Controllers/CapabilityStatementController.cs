using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using sReportsV2.Domain.Services.Interfaces;

namespace sReportsV2.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class CapabilityStatementController //: CommonController
    {
        /*public CapabilityStatementController(IMapper mapper,
         IFormInstanceService formInstanceService,
         IEpisodeOfCareService episodeOfCareService,
         IEncounterService encounterService,
         IFormService formService)
     : base(mapper, formInstanceService, episodeOfCareService, encounterService, formService) { }


        [HttpGet]
        public IActionResult Get()
        {
            CapabilityStatement capabilityStatement = new CapabilityStatement();
            SetCapabilityStatementAdministrativeInfo(capabilityStatement);

            capabilityStatement.Rest = new List<CapabilityStatement.RestComponent>();
            CapabilityStatement.RestComponent rest = new CapabilityStatement.RestComponent();
            rest.Mode = CapabilityStatement.RestfulCapabilityMode.Server;
            rest.Security = new CapabilityStatement.SecurityComponent() { Service = new List<CodeableConcept>() { new CodeableConcept() { Text = "OAuth" } } };
            SetResources(rest);
            capabilityStatement.Rest.Add(rest);

            JsonSerializerSettings JsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };

            return Ok(JsonConvert.SerializeObject(capabilityStatement, JsonSettings));
        }

        private void SetResources(CapabilityStatement.RestComponent rest) 
        {
            rest.Resource = new List<CapabilityStatement.ResourceComponent>();

            rest.Resource.Add(CreateResourceComponent(ResourceType.Patient, new List<CapabilityStatement.TypeRestfulInteraction>()
            {
                CapabilityStatement.TypeRestfulInteraction.Read,
                CapabilityStatement.TypeRestfulInteraction.Create

            },
            new List<string>() { "patientId", "Family", "Given", "City", "Country", "PostalCode", "State", "Telecom", "Identifier" }
            ));
            rest.Resource.Add(CreateResourceComponent(ResourceType.DiagnosticReport, new List<CapabilityStatement.TypeRestfulInteraction>()
            {
                CapabilityStatement.TypeRestfulInteraction.Read,
                CapabilityStatement.TypeRestfulInteraction.Create

            },
            new List<string>() { "diagnosticReportId", "Encounter", "Performer" }
            ));

            rest.Resource.Add(CreateResourceComponent(ResourceType.Encounter, new List<CapabilityStatement.TypeRestfulInteraction>()
            {
                CapabilityStatement.TypeRestfulInteraction.Read,
                CapabilityStatement.TypeRestfulInteraction.Create

            },
            new List<string>() { "id", "Class", "Type", "Status" }
            ));

            rest.Resource.Add(CreateResourceComponent(ResourceType.EpisodeOfCare, new List<CapabilityStatement.TypeRestfulInteraction>()
            {
                CapabilityStatement.TypeRestfulInteraction.Read,
                CapabilityStatement.TypeRestfulInteraction.Create

            },
            new List<string>() { "episodeOfCareId", "Condition", "Type", "Status" }
            ));

            rest.Resource.Add(CreateResourceComponent(ResourceType.Observation, new List<CapabilityStatement.TypeRestfulInteraction>()
            {
                CapabilityStatement.TypeRestfulInteraction.Read,
                CapabilityStatement.TypeRestfulInteraction.Create

            },
           new List<string>() { "observationId", "Encounter", "Performer" }
           ));


            rest.Resource.Add(CreateResourceComponent(ResourceType.Organization, new List<CapabilityStatement.TypeRestfulInteraction>()
            {
                CapabilityStatement.TypeRestfulInteraction.Read,
                CapabilityStatement.TypeRestfulInteraction.Create

            },
           new List<string>() { "organizationId", "Type", "Name", "PartOf", "Identifier" }
           ));

            rest.Resource.Add(CreateResourceComponent(ResourceType.Procedure, new List<CapabilityStatement.TypeRestfulInteraction>()
            {
                CapabilityStatement.TypeRestfulInteraction.Read,
                CapabilityStatement.TypeRestfulInteraction.Create

            },
           new List<string>() { "procedureId", "Encounter", "Performer" }
           ));
        }

        private CapabilityStatement.ResourceComponent CreateResourceComponent(ResourceType type, List<CapabilityStatement.TypeRestfulInteraction> interactions, List<string> searchParams) 
        {
            CapabilityStatement.ResourceComponent resource = new CapabilityStatement.ResourceComponent();
            resource.Type = type;
            resource.Interaction = new List<CapabilityStatement.ResourceInteractionComponent>();
            foreach(var interaction in interactions) 
            {
                resource.Interaction.Add(new CapabilityStatement.ResourceInteractionComponent() { Code = interaction });
            }


            resource.SearchParam = new List<CapabilityStatement.SearchParamComponent>();
            foreach (string searchParam in searchParams) 
            {
                resource.SearchParam.Add(new CapabilityStatement.SearchParamComponent() { Name = searchParam, Type = SearchParamType.String });
            }


            return resource;
        }

        private void SetCapabilityStatementAdministrativeInfo(CapabilityStatement capabilityStatement)
        {
            capabilityStatement.Name = "sReports";
            capabilityStatement.Status = PublicationStatus.Draft;
            capabilityStatement.Date = Date.Today().ToString();
            capabilityStatement.Publisher = "weMedoo";
            capabilityStatement.Contact = new List<ContactDetail>() { new ContactDetail() { Telecom = new List<ContactPoint>() { new ContactPoint() { System = ContactPoint.ContactPointSystem.Url, Value = "https://nianalytics.com/" } } } };
            capabilityStatement.Kind = CapabilityStatement.CapabilityStatementKind.Capability;
            capabilityStatement.Software = new CapabilityStatement.SoftwareComponent() { Name = "sReports" };
            capabilityStatement.FhirVersion = "4.0.1";
            capabilityStatement.Format = new List<string>() { "json", "xml" };
        }*/
    }
}