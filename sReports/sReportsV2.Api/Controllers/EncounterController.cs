using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sReportsV2.Api.Common.Extensions;
using sReportsV2.Api.DTOs;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Services.Interfaces;

namespace sReportsV2.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class EncounterController //: CommonController
    {
       /* public EncounterController(IMapper mapper,
             IFormInstanceService formInstanceService,
             IEpisodeOfCareService episodeOfCareService,
             IEncounterService encounterService,
             IFormService formService)
         : base(mapper, formInstanceService, episodeOfCareService, encounterService, formService) { }

        //------------READ------------

        /// <summary>
        /// Get encounter by encounter id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Encounter), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetEncounter(string id)
        {
            if (string.IsNullOrWhiteSpace(id) || !id.IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));

            if (!encounterService.ExistEncounter(id))
                return NotFound(GetError(IssueType.NotFound));

            Encounter encounter = mapper.Map<Encounter>(encounterService.GetById(id));

            return Ok(encounter);
        }

        //------------SEARCH------------

        /// <summary>
        /// Get encounter(s) by search parameters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult GetEncounterByParameters([FromQuery] EncounterFilterDataIn encounterFilterDataIn)
        {
            EncounterFhirFilter filterEncounter = mapper.Map<EncounterFhirFilter>(encounterFilterDataIn);
            List<Encounter> listEncounters = mapper.Map<List<Encounter>>(encounterService.GetByParameters(filterEncounter));

            Bundle encounters = new Bundle();
            foreach (Encounter item in listEncounters)
            {
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.Path.Value.ToString()}/{item.Id}";
                encounters.AddResourceEntry(item, path);
            }

            return Ok(encounters);
        }

        /// <summary>
        /// Post encounter(s) by search parameters
        /// </summary>
        [HttpPost("_search")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult PostEncounterByParameters([FromBody] EncounterFilterDataIn encounterFilterDataIn)
        {
            EncounterFhirFilter filterEncounter = mapper.Map<EncounterFhirFilter>(encounterFilterDataIn);
            List<Encounter> listEncounters = mapper.Map<List<Encounter>>(encounterService.GetByParameters(filterEncounter));

            Bundle encounters = new Bundle();

            foreach (Encounter item in listEncounters)
            {
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{"/api/Encounter"}/{item.Id}";
                encounters.AddResourceEntry(item, path);
            }

            return Ok(encounters);
        }
       */
    }
}