using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.FormDistribution.DataIn;
using sReportsV2.DTOs.FormDistribution.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IFormDistributionBLL
    {
        FormDistributionParameterizationDataOut GetFormDistributionForParameterization(int thesaurusId, string versionId);
        FormFieldDistributionDataOut GetFormFieldDistribution(string formDistributionId, string fieldId);
        void SetParameters(FormDistributionDataIn dataIn);
        RelationFieldAutocompleteResultDataOut GetRelationFieldAutocomplete(AutocompleteDataIn dataIn, string formDistributionId);
        FormFieldDistributionDataOut ResetAllRelationsForField(string formDistributionId, string formFieldDistributionId, UserCookieData userCookieData);
        Field GetFormField(int thesaurusId, string versionId, UserCookieData userCookieData, string fieldId);

    }
}
