using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using sReportsV2.Api.DTOs.Patient.DataIn;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using sReportsV2.Api.Common.Extensions;
using sReportsV2.Common.Constants;

namespace sReportsV2.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class PatientController //: CommonController
    {
       /* private readonly IPatientService patientService;

        public PatientController(IMapper mapper,
            IFormInstanceService formInstanceService,
            IEpisodeOfCareService episodeOfCareService,
            IEncounterService encounterService, 
            IPatientService patientService,
            IFormService formService) : base(mapper, formInstanceService, episodeOfCareService, encounterService, formService)
        {
            this.patientService = patientService;
        }

        /// <summary>
        /// Get all diagnostic reports, observations and procedures by patient id
        /// </summary>
        [HttpGet("{patientId}/$everything")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetAllByPatientID(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId) || !patientId.IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));

            if (!patientService.ExistsPatientByObjectId(patientId))
                return NotFound(GetError(IssueType.NotFound));

            Bundle bundle = MakeBundleForPatient(patientId, GetFormInstances(patientId));

            return Ok(bundle);
        }

        // GET: FhirDiagnosticReport

        /// <summary>
        /// Get all diagnostic reports for patient
        /// </summary>
        [HttpGet("{patientId}/DiagnosticReport")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetAllDiagnosticReportsForPatient(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId) || !patientId.IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));

            if (!patientService.ExistsPatientByObjectId(patientId))
                return NotFound(GetError(IssueType.NotFound));

            List<EpisodeOfCareEntity> episodes = episodeOfCareService.GetByPatientId(patientId).ToList();
            List<FormInstance> instances = new List<FormInstance>();

            foreach (EpisodeOfCareEntity episode in episodes)
            {
                instances.AddRange(formInstanceService.GetByEpisodeOfCareId(episode.Id));
            }

            Bundle bundle = new Bundle();

            foreach (FormInstance formInstance in instances)
            {
                DiagnosticReport report = GetDiagnosticReport(formInstance, patientId);
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{"/api/DiagnosticReport"}/{formInstance.Id}";
                bundle.AddResourceEntry(report, path);
            }

            return Ok(bundle);
        }

        // GET: FhirObservation

        /// <summary>
        /// Get all observations for patient
        /// </summary>
        [HttpGet("{patientId}/Observation")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetAllObservations(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId) || !patientId.IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));

            if (!patientService.ExistsPatientByObjectId(patientId))
                return NotFound(GetError(IssueType.NotFound));

            Bundle observations = GetFhirElements(patientId, ResourceTypes.Observation, GetFormInstances(patientId));

            return Ok(observations);
        }

        // GET: FhirProcedure

        /// <summary>
        /// Get all procedures for patient
        /// </summary>
        [HttpGet("{patientId}/Procedure")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetAllProcedures(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId) || !patientId.IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));

            if (!patientService.ExistsPatientByObjectId(patientId))
                return NotFound(GetError(IssueType.NotFound));

            Bundle procedures = GetFhirElements(patientId, ResourceTypes.Procedure, GetFormInstances(patientId));

            return Ok(procedures);
        }

        //------------READ------------

        /// <summary>
        /// Get patient by patient id
        /// </summary>
        [Authorize(Policy = "AccessAsApplication")]
        [HttpGet("{patientId}")]
        [ProducesResponseType(typeof(Patient), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetPatient(string patientId)
        {
            if (string.IsNullOrWhiteSpace(patientId) || !patientId.IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));

            if (!patientService.ExistsPatientByObjectId(patientId))
                return NotFound(GetError(IssueType.NotFound));

            Patient patient = mapper.Map<Patient>(patientService.GetById(patientId));

            return Ok(patient);
        }

        //------------SEARCH------------

        /// <summary>
        /// Get patient(s) by search parameters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult GetPatientByParameters([FromQuery] PatientFilterDataIn patientFhirDataIn)
        {
            PatientFhirFilter filterPatient = mapper.Map<PatientFhirFilter>(patientFhirDataIn);
            List<Patient> listPatients = mapper.Map<List<Patient>>(patientService.GetByParameters(filterPatient));

            Bundle patients = new Bundle();
            foreach (Patient item in listPatients)
            {
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{this.Request.Path.Value.ToString()}/{item.Id}";
                patients.AddResourceEntry(item, path);
            }

            return Ok(patients);
        }

        /// <summary>
        /// Post patient(s) by search parameters
        /// </summary>
        [HttpPost("_search")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult PostPatientByParameters([FromBody] PatientFilterDataIn patientFhirDataIn)
        {
            PatientFhirFilter filterPatient = mapper.Map<PatientFhirFilter>(patientFhirDataIn);
            List<Patient> listPatients = mapper.Map<List<Patient>>(patientService.GetByParameters(filterPatient));

            Bundle patients = new Bundle();
            foreach (Patient item in listPatients)
            {
                string path = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}{"/api/Patient"}/{item.Id}";
                patients.AddResourceEntry(item, path);
            }

            return Ok(patients);
        }*/

    }
}
