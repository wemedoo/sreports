using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sReportsV2.Api.Common.Extensions;
using sReportsV2.Api.DTOs.FormInstance.DataIn;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
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
    public class ProcedureController //: CommonController
    {
        /*public ProcedureController(IMapper mapper,
            IFormInstanceService formInstanceService,
            IEpisodeOfCareService episodeOfCareService,
            IEncounterService encounterService,
            IFormService formService)
        : base(mapper, formInstanceService, episodeOfCareService, encounterService,formService) { }

        //------------READ------------

        /// <summary>
        /// Get procedure by procedure id
        /// </summary>
        [HttpGet("{procedureId}")]
        [ProducesResponseType(typeof(Procedure), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(OperationOutcome), StatusCodes.Status404NotFound)]
        public IActionResult GetProcedure(string procedureId)
        {
            string[] splitted = procedureId.Split('.');
            if (string.IsNullOrEmpty(procedureId) || splitted.Count() != 2 || !splitted[0].IsObjectId())
                return BadRequest(GetError(IssueType.Invalid));
            
            if (ProcedureNotExist(splitted[0], splitted[1]))
                return NotFound(GetError(IssueType.NotFound));

            string formInstanceId = splitted[0];
            string formFieldId = splitted[1];

            FormInstance formInstance = formInstanceService.GetById(formInstanceId);
            Form form = new Form(formInstance, formService.GetForm(formInstance.FormDefinitionId));
            FieldSelectable field = form.GetField(formFieldId.Replace('-', '_')) as FieldSelectable;

            Procedure procedure = mapper.Map<Procedure>(field);
            procedure.Performer.Add(new Procedure.PerformerComponent() { Actor = new ResourceReference(formInstance.UserId.ToString()) });
            procedure.Subject = new ResourceReference(episodeOfCareService.GetByEncounter(formInstance.EncounterRef)?.PatientId);
            procedure.Id = $"{"Procedure"}/{formInstance.Id.ToString()}.{field.Id.Replace('_', '-')}";

            foreach (FormFieldValue reasonCode in field.Values)
            {
                foreach (string value in field.Value[0].Split(',').Where(x=> x.Equals(reasonCode.Label)))
                {
                    procedure.ReasonCode.Add(new CodeableConcept(ResourceTypes.O40MtId, reasonCode.ThesaurusId, reasonCode.Value));
                }
            }

            return Ok(procedure);
        }

        //------------SEARCH------------

        /// <summary>
        /// Get procedure(s) by search parameters
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult GetAllProceduresByParameters([FromQuery] FormInstanceFilterDataIn formInstanceFhirDataIn)
        {
            string basePath = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}";
            FormInstanceFhirFilter filterFormInstance = mapper.Map<FormInstanceFhirFilter>(formInstanceFhirDataIn);
            Bundle bundle = new Bundle();
            List<FormInstance> listFormInstances = formInstanceService.GetByParameters(filterFormInstance);

            foreach (FormInstance formInstance in listFormInstances)
            {
                string patientId = episodeOfCareService.GetByEncounter(formInstance.EncounterRef)?.PatientId;               
                bundle.AddProceduresIntoBundle(formInstance, patientId, basePath);
            }

            return Ok(bundle);
        }

        /// <summary>
        /// Post procedure(s) by search parameters
        /// </summary>
        [HttpPost("_search")]
        [ProducesResponseType(typeof(Bundle), StatusCodes.Status200OK)]
        public IActionResult PostAllProceduresByParameters([FromBody] FormInstanceFilterDataIn formInstanceFhirDataIn)
        {
            string basePath = $"{this.Request.Scheme}://{this.Request.Host.Value.ToString()}";
            FormInstanceFhirFilter filterFormInstance = mapper.Map<FormInstanceFhirFilter>(formInstanceFhirDataIn);
            Bundle bundle = new Bundle();
            List<FormInstance> listFormInstances = formInstanceService.GetByParameters(filterFormInstance);

            foreach (FormInstance formInstance in listFormInstances)
            {
                string patientId = episodeOfCareService.GetByEncounter(formInstance.EncounterRef)?.PatientId;
                bundle.AddProceduresIntoBundle(formInstance, patientId, basePath);
            }

            return Ok(bundle);
        }

        private bool ProcedureNotExist(string formInstanceId, string formFieldId) 
        {
            FormInstance formInstance = this.formInstanceService.GetById(formInstanceId);
            Form form = this.formService.GetForm(formInstance?.FormDefinitionId);
            form.SetFields(formInstance.Fields);
            return !formInstanceService.ExistsFormInstance(formInstanceId) || form.GetField(formFieldId.Replace('-', '_')) == null || form.GetField(formFieldId.Replace('-', '_')).FhirType != ResourceTypes.Procedure;
        }*/
    }
}