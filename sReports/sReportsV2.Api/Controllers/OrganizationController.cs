using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sReportsV2.Api.Common.Extensions;
using sReportsV2.Api.DTOs.Organization.DataIn;
using sReportsV2.Domain.Services.Interfaces;

namespace sReportsV2.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class OrganizationController //: CommonController
    {
       /* private readonly IOrganizationService organizationService;
        
        public OrganizationController(IMapper mapper,
            IFormInstanceService formInstanceService,
            IEpisodeOfCareService episodeOfCareService,
            IEncounterService encounterService,
            IOrganizationService organizationService,
            IFormService formService) : base(mapper, formInstanceService, episodeOfCareService, encounterService, formService)
        {
            this.organizationService = organizationService;
        }

        //------------READ------------

        /// <summary>
        /// Get organization by organization id
        /// </summary>
        [HttpGet("{organizationId}")]
        [ProducesResponseType(typeof(Organization), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetOrganization(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId) || !organizationId.IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));
            
            if (!organizationService.ExistsOrganizationById(organizationId))
                return NotFound(GetError(IssueType.NotFound));

            Organization organization = mapper.Map<Organization>(organizationService.GetOrganizationById(organizationId));

            return Ok(organization);
        }

        //------------SEARCH------------

        /// <summary>
        /// Get organization(s) by search parameters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult GetOrganizationByParameters([FromQuery] OrganizationFilterDataIn organizationFhirDataIn)
        {
            OrganizationFilter filterOrganization = mapper.Map<OrganizationFilter>(organizationFhirDataIn);
            List<Organization> listOrganizations = mapper.Map<List<Organization>>(organizationService.GetByParameters(filterOrganization));

            Bundle organizations = new Bundle();

            foreach (Organization item in listOrganizations)
            {
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.Path.Value.ToString()}/{item.Id}";
                organizations.AddResourceEntry(item, path);
            }

            return Ok(organizations);
        }

        /// <summary>
        /// Post organization(s) by search parameters
        /// </summary>
        [HttpPost("_search")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult PostOrganizationByParameters([FromBody] OrganizationFilterDataIn organizationFhirDataIn)
        {
            OrganizationFilter filterOrganization = mapper.Map<OrganizationFilter>(organizationFhirDataIn);
            List<Organization> listOrganizations = mapper.Map<List<Organization>>(organizationService.GetByParameters(filterOrganization));

            Bundle organizations = new Bundle();

            foreach (Organization item in listOrganizations)
            {
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{"/api/Organization"}/{item.Id}";
                organizations.AddResourceEntry(item, path);
            }

            return Ok(organizations);
        }*/

    }
}