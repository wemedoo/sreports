using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sReportsV2.Api.Common.Extensions;
using sReportsV2.Api.DTOs.FormInstance.DataIn;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Constants;

namespace sReportsV2.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class ObservationController //: CommonController
    {
       /* public ObservationController(IMapper mapper,
            IFormInstanceService formInstanceService,
            IEpisodeOfCareService episodeOfCareService,
            IEncounterService encounterService,
            IFormService formService)
        : base(mapper, formInstanceService, episodeOfCareService, encounterService, formService)
        { }

        //------------READ------------

        /// <summary>
        /// Get observation by observation id
        /// </summary>
        [HttpGet("{observationId}")]
        [ProducesResponseType(typeof(Observation), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetObservation(string observationId)
        {
            string[] splitted = observationId.Split('.');
            if (string.IsNullOrEmpty(observationId) || splitted.Count() != 2 || !splitted[0].IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));

            if (ObservationNotExist(splitted[0] , splitted[1]))
                return NotFound(GetError(IssueType.NotFound));

            string formInstanceId = splitted[0];
            string formFieldId = splitted[1].Replace('-', '_');

            FormInstance formInstance = formInstanceService.GetById(formInstanceId);
            Form form = new Form(formInstance, formService.GetForm(formInstance.FormDefinitionId));
            Field field = form.GetField(formFieldId);

            Observation observation = mapper.Map<Observation>(field);
            observation.Value = new FhirString(field.Value?[0]);
            observation.Performer.Add(new ResourceReference(formInstance.UserId.ToString()));
            observation.Subject = new ResourceReference(episodeOfCareService.GetByEncounter(formInstance.EncounterRef)?.PatientId);
            observation.Id = $"{"Observation"}/{formInstance.Id.ToString()}.{field.Id.Replace('_', '-')}";

            return Ok(observation);
        }


        //------------SEARCH------------

        /// <summary>
        /// Get observation(s) by search parameters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public ActionResult<object> GetAllObservationsByParameters([FromQuery] FormInstanceFilterDataIn formInstanceFhirDataIn)
        {
            FormInstanceFhirFilter filterFormInstance = mapper.Map<FormInstanceFhirFilter>(formInstanceFhirDataIn);
            Bundle bundle = new Bundle();
            List<FormInstance> listFormInstances = formInstanceService.GetByParameters(filterFormInstance);

            foreach (FormInstance formInstance in listFormInstances)
            {
                string basePath = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}";
                string patientId = episodeOfCareService.GetByEncounter(formInstance.EncounterRef)?.PatientId;
                bundle.AddObservationsIntoBundle(formInstance, patientId, basePath);
            }

            return Ok(bundle);
        }

        /// <summary>
        /// Post observation(s) by search parameters
        /// </summary>
        [HttpPost("_search")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public ActionResult<object> PostAllObservationsByParameters([FromBody] FormInstanceFilterDataIn formInstanceFhirDataIn)
        {
            string basePath = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}";
            FormInstanceFhirFilter filterFormInstance = mapper.Map<FormInstanceFhirFilter>(formInstanceFhirDataIn);
            Bundle bundle = new Bundle();
            List<FormInstance> listFormInstances = formInstanceService.GetByParameters(filterFormInstance);

            foreach (FormInstance formInstance in listFormInstances)
            {
                string patientId = episodeOfCareService.GetByEncounter(formInstance.EncounterRef)?.PatientId;
                bundle.AddObservationsIntoBundle(formInstance, patientId, basePath);
            }

            return Ok(bundle);
        }

        private bool ObservationNotExist(string formInstanceId, string formFieldId) 
        {
            FormInstance formInstance = this.formInstanceService.GetById(formInstanceId);
            Form form = this.formService.GetForm(formInstance?.FormDefinitionId);
            form.SetFields(formInstance.Fields);
            return !formInstanceService.ExistsFormInstance(formInstanceId) || form.GetField(formFieldId.Replace('-', '_')) == null || form.GetField(formFieldId.Replace('-', '_')).FhirType != ResourceTypes.Observation;
        }*/
    }
}