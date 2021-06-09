using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sReportsV2.Api.Common.Extensions;
using sReportsV2.Api.DTOs.FormInstance.DataIn;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Services.Interfaces;

namespace sReportsV2.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class DiagnosticReportController //: CommonController
    {
        /*public DiagnosticReportController(IMapper mapper,
             IFormInstanceService formInstanceService,
             IEpisodeOfCareService episodeOfCareService,
             IEncounterService encounterService,
             IFormService formService)
         : base(mapper, formInstanceService, episodeOfCareService, encounterService, formService) { }

        //------------READ------------

        /// <summary>
        /// Get diagnostic report by diagnostic report id
        /// </summary>
        [HttpGet("{diagnosticReportId}")]
        [ProducesResponseType(typeof(DiagnosticReport), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetDiagnosticReport(string diagnosticReportId)
        {
            if (string.IsNullOrWhiteSpace(diagnosticReportId) || !diagnosticReportId.IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));

            if (!formInstanceService.ExistsFormInstance(diagnosticReportId))
                return NotFound(GetError(IssueType.NotFound));
           
            FormInstance formInstance = formInstanceService.GetById(diagnosticReportId);
            DiagnosticReport diagnosticReport = GetDiagnosticReport(formInstance, formInstance.Id);

            return Ok(diagnosticReport);
        }

        //------------SEARCH------------

        /// <summary>
        /// Get diagnostic report(s) by search parameters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult GetDiagnosticReportByParameters([FromQuery] FormInstanceFilterDataIn formInstanceFhirDataIn)
        {
            FormInstanceFhirFilter filterFormInstance = mapper.Map<FormInstanceFhirFilter>(formInstanceFhirDataIn);
            List<FormInstance> listFormInstances = formInstanceService.GetByParameters(filterFormInstance);

            Bundle formInstance = new Bundle();

            foreach (FormInstance item in listFormInstances)
            {
                DiagnosticReport report = GetDiagnosticReport(item, item.Id);
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.Path.Value.ToString()}/{item.Id}";
                formInstance.AddResourceEntry(report, path);
            }

            return Ok(formInstance);   
        }

        /// <summary>
        /// Post diagnostic report(s) by search parameters
        /// </summary>
        [HttpPost("_search")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult PostDiagnosticReportByParameters([FromBody] FormInstanceFilterDataIn formInstanceFhirDataIn)
        {
            FormInstanceFhirFilter filterFormInstance = mapper.Map<FormInstanceFhirFilter>(formInstanceFhirDataIn);
            List<FormInstance> listFormInstances = formInstanceService.GetByParameters(filterFormInstance);

            Bundle formInstance = new Bundle();

            foreach (FormInstance item in listFormInstances)
            {
                DiagnosticReport report = GetDiagnosticReport(item, item.Id);
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{"/api/DiagnosticReport"}/{item.Id}";
                formInstance.AddResourceEntry(report, path);
            }

            return Ok(formInstance);
        }*/

    }
}