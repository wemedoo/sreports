using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sReportsV2.Api.DTOs.DfD.DataOut;
using sReportsV2.Domain.Entities.DFD;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;

namespace sReportsV2.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "dfd-v1")]
    public class DfDController : ControllerBase
    {

        /*private IFormInstanceService formInstanceService;
        private IFormService formService;
        public DfDController()
        {
            this.formInstanceService = new FormInstanceService();
            this.formService = new FormService();
        }

        /// <summary>
        /// Get diagnostic report by diagnostic report id
        /// </summary>
        [HttpGet("/test-connection")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult TestConnection()
        {            
            return Ok("Connection works");
        }

        /// <summary>
        /// Get diagnostic report by diagnostic report id
        /// </summary>
        [HttpGet("/test-secure-connection")]
        [Authorize(Policy = "AccessAsDfDApplication")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult TestConnectionOAuth()
        {
            return Ok("Secure connection works");
        }

        [HttpGet("users")]
        [Authorize(Policy = "AccessAsDfDApplication")]
        [ProducesResponseType(typeof(DfDCollectionDataOut<DfDUserDataOut>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDataOut), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetUsers(int limit = 50, int offset = 0)
        {
            string validationResult = ValidateInputs(limit, offset);
            if (!string.IsNullOrWhiteSpace(validationResult))
            {
                return BadRequest(new ErrorDataOut() { Message = validationResult });
            }

            Form form = formService.GetForm(Constants.PatientGeneralInfoForm);
            List<FormInstance> doctors = formInstanceService.GetByDefinitionId(Constants.PatientGeneralInfoForm, limit, offset);           

            DfDCollectionDataOut<DfDUserDataOut> result = new DfDCollectionDataOut<DfDUserDataOut>()
            {
                Data = GetUserDataMapped(doctors, form),
                Offset = offset,
                Limit = limit,
                Total = formInstanceService.CountByDefinition(Constants.PatientGeneralInfoForm)
            };

            return Ok(result);
        }

        [HttpGet("forms")]
        [Authorize(Policy = "AccessAsDfDApplication")]
        [ProducesResponseType(typeof(DfDCollectionDataOut<DfdFormDataOut>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorDataOut), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult GetForms(int limit = 50, int offset = 0)
        {
            string validationResult = ValidateInputs(limit, offset);
            if (!string.IsNullOrWhiteSpace(validationResult))
            {
                return BadRequest(new ErrorDataOut() { Message = validationResult });
            }

            List<Form> forms = formService.GetAllByOrganization(Constants.OrganizationId, limit, offset);

            DfDCollectionDataOut<DfdFormDataOut> result = new DfDCollectionDataOut<DfdFormDataOut>()
            {
                Data = forms.Select(x => new DfdFormDataOut
                {
                    Id = x.Id,
                    Language = x.Language,
                    Title = x.Title
                })
                .ToList(),
                Offset = offset,
                Limit = limit,
                Total = formService.CountByOrganization(Constants.OrganizationId)
            };

            return Ok(result);
        }

        private string ValidateInputs(int limit, int offset)
        {
            string result = string.Empty;
            if(offset < 0)
            {
                result += $"The offset value must be positive number.{System.Environment.NewLine}";
            }

            if(limit <= 0)
            {
                result += $"The limit value must be greater than 0.{System.Environment.NewLine}";
            }
            return  result;
        }

        private List<DfDUserDataOut> GetUserDataMapped(List<FormInstance> doctors, Form form)
        {
            List<DfDUserDataOut> result = new List<DfDUserDataOut>();

            foreach (FormInstance formInstance in doctors)
            {
                DfDUserDataOut user = new DfDUserDataOut()
                {
                    Id = formInstance.Id,
                    PhoneNumber = formInstance.GetFieldValueById(Constants.PatintPhoneNumberFieldId),
                    Gender = GetGender(formInstance.GetFieldValueById(Constants.PatintGenderFieldId))
                };
                result.Add(user);
            }

            return result;
        }

        private Gender GetGender(string value)
        {
            Gender result = Gender.UNKNOWN;
            if(value == Constants.PatientGenderMaleThesaurusId)
            {
                result = Gender.MALE;
            }
            else if(value == Constants.PatientGenderFemaleThesaurusId)
            {
                result = Gender.FEMALE;
            }

            return result;
        }*/
    }
}