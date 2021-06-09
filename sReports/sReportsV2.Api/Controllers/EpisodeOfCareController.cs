using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sReportsV2.Api.Common.Extensions;
using sReportsV2.Api.DTOs.EpisodeOfCare.DataIn;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Services.Interfaces;

namespace sReportsV2.Api.Controllers
{
   [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class EpisodeOfCareController //: CommonController
    {
        /*public EpisodeOfCareController(IMapper mapper,
             IFormInstanceService formInstanceService,
             IEpisodeOfCareService episodeOfCareService,
             IEncounterService encounterService,
             IFormService formService)
         : base(mapper, formInstanceService, episodeOfCareService, encounterService,formService) { }

        //------------READ------------

        /// <summary>
        /// Get episode of care by episode of care id
        /// </summary>
        [HttpGet("{episodeOfCareId}")]
        [ProducesResponseType(typeof(EpisodeOfCare), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetEpisodeOfCare(string episodeOfCareId)
        {
            if (string.IsNullOrWhiteSpace(episodeOfCareId) || !episodeOfCareId.IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));

            if (!episodeOfCareService.ExistById(episodeOfCareId))
                return NotFound(GetError(IssueType.NotFound));

            EpisodeOfCare episodeOfCare = mapper.Map<EpisodeOfCare>(episodeOfCareService.GetEOCById(episodeOfCareId));

            return Ok(episodeOfCare);
        }

        //------------SEARCH------------

        /// <summary>
        /// Get episode of care(s) by search parameters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult GetEOCByParameters([FromQuery] EpisodeOfCareFilterDataIn episodeOfCareFhirDataIn)
        {
            EpisodeOfCareFhirFilter filterEpisodeOfCare = mapper.Map<EpisodeOfCareFhirFilter>(episodeOfCareFhirDataIn);
            List<EpisodeOfCare> listEpisodeOfCares = mapper.Map<List<EpisodeOfCare>>(episodeOfCareService.GetByParameters(filterEpisodeOfCare));

            Bundle episodeOfCares = new Bundle();

            foreach (EpisodeOfCare item in listEpisodeOfCares)
            {
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.Path.Value.ToString()}/{item.Id}";
                episodeOfCares.AddResourceEntry(item, path);
            }

            return Ok(episodeOfCares);
        }

        /// <summary>
        /// Post episode of care(s) by search parameters
        /// </summary>
        [HttpPost("_search")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult PostEOCByParameters([FromBody] EpisodeOfCareFilterDataIn episodeOfCareFhirDataIn)
        {
            EpisodeOfCareFhirFilter filterEpisodeOfCare = mapper.Map<EpisodeOfCareFhirFilter>(episodeOfCareFhirDataIn);
            List<EpisodeOfCare> listEpisodeOfCares = mapper.Map<List<EpisodeOfCare>>(episodeOfCareService.GetByParameters(filterEpisodeOfCare));

            Bundle episodeOfCares = new Bundle();

            foreach (EpisodeOfCare item in listEpisodeOfCares)
            {
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{"/api/EpisodeOfCare"}/{item.Id}";
                episodeOfCares.AddResourceEntry(item, path);
            }

            return Ok(episodeOfCares);
        }*/

    }
}