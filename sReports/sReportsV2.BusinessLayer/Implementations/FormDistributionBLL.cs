using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.DTOs.Autocomplete;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.Autocomplete.DataOut;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormDistribution.DataIn;
using sReportsV2.DTOs.FormDistribution.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class FormDistributionBLL : IFormDistributionBLL
    {
        private readonly IFormDistributionDAL formDistributionDAL;
        private readonly IFormDAL formDAL;

        public FormDistributionBLL(IFormDistributionDAL formDistributionDAL, IFormDAL formDAL)
        {
            this.formDistributionDAL = formDistributionDAL;
            this.formDAL = formDAL;
        }

        public FormDistributionParameterizationDataOut GetFormDistributionForParameterization(int thesaurusId, string versionId)
        {
            FormDistribution formDistribution = formDistributionDAL.GetByThesaurusIdAndVersion(thesaurusId, versionId);
            Form form = formDAL.GetFormByThesaurusAndVersion(thesaurusId, versionId);

            if (formDistribution == null)
            {
                formDistribution = Create(form);
            }

            FormDistributionDataOut dataOut = Mapper.Map<FormDistributionDataOut>(formDistribution);
            SetRelatedFieldsLabels(dataOut, form);
            
            return new FormDistributionParameterizationDataOut() 
            {
                Form = Mapper.Map<FormDataOut>(form),
                FormDistribution = dataOut
            };
        }

        public void SetParameters(FormDistributionDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            FormDistribution formDistribution;
            if (!string.IsNullOrEmpty(dataIn.FormDistributionId))
            {
                formDistribution = formDistributionDAL.GetById(dataIn.FormDistributionId);
            }
            else
            {
                Form form = formDAL.GetFormByThesaurusAndVersion(dataIn.ThesaurusId, dataIn.VersionId);
                formDistribution = GetFromForm(form);
            }
            UpdateField(dataIn, formDistribution);
            formDistributionDAL.InsertOrUpdate(formDistribution);            
        }

        public FormFieldDistributionDataOut GetFormFieldDistribution(string formDistributionId, string fieldId)
        {
            return Mapper.Map<FormFieldDistributionDataOut>(formDistributionDAL.GetFormFieldDistribution(formDistributionId, fieldId));
        }

        public RelationFieldAutocompleteResultDataOut GetRelationFieldAutocomplete(AutocompleteDataIn dataIn, string formDistributionId)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            FormDistributionDataOut formDistribution = Mapper.Map<FormDistributionDataOut>(formDistributionDAL.GetById(formDistributionId));
            FormFieldDistributionDataOut targetField = formDistribution.GetFieldById(dataIn.ExcludeId);
            IEnumerable<FormFieldDistributionDataOut> filteredFields = FilterFieldsByLabel(formDistribution, dataIn.Term, CreateFieldExclusionList(targetField));
            List<RelationFieldAutocompleteDataOut>  formFieldDistributionDataOuts = FormatFieldsForDisplay(filteredFields, dataIn);

            RelationFieldAutocompleteResultDataOut result = new RelationFieldAutocompleteResultDataOut()
            {
                pagination = new AutocompletePaginatioDataOut()
                {
                    more = Math.Ceiling(filteredFields.Count() / 15.00) > dataIn.Page,
                },
                results = formFieldDistributionDataOuts
            };

            return result;
        }

        public FormFieldDistributionDataOut ResetAllRelationsForField(string formDistributionId, string formFieldDistributionId, UserCookieData userCookieData)
        {
            FormDistribution formDistribution = formDistributionDAL.GetById(formDistributionId);
            Form form = formDAL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(formDistribution.ThesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, formDistribution.VersionId);
            Field fieldDefinition = form.GetFieldById(formFieldDistributionId);
            FormFieldDistribution blankFieldDistribution = GetDistributionField(fieldDefinition);

            FormFieldDistribution fieldDb = formDistribution.Fields.FirstOrDefault(x => x.Id.Equals(formFieldDistributionId));
            if (fieldDb != null)
            {
                formDistribution.Fields[formDistribution.Fields.IndexOf(fieldDb)] = blankFieldDistribution;
            }
            formDistributionDAL.InsertOrUpdate(formDistribution);

            return Mapper.Map<FormFieldDistributionDataOut>(blankFieldDistribution);
        }

        private IEnumerable<FormFieldDistributionDataOut> FilterFieldsByLabel(FormDistributionDataOut formdistribution, string labelName, List<string> exclusionList)
        {
            return formdistribution.Fields.Where(x => x.CanBeAddedToRelation() && !exclusionList.Contains(x.Id) && x.Label.ToLower().Contains(labelName.ToLower()));
        }

        private List<string> CreateFieldExclusionList(FormFieldDistributionDataOut targetField)
        {
            List<string> exclusionList = targetField.GetRelatedFieldsIds();
            exclusionList.Add(targetField.Id);
            return exclusionList;
        }

        private List<RelationFieldAutocompleteDataOut> FormatFieldsForDisplay(IEnumerable<FormFieldDistributionDataOut> filteredFields, AutocompleteDataIn dataIn)
        {
            return filteredFields
                .OrderBy(x => x.Label)
                .Skip(dataIn.Page * 15)
                .Take(15)
                .Select(x => new RelationFieldAutocompleteDataOut()
                {
                    id = x.Id.ToString(),
                    text = x.Label,
                    type = x.Type
                })
                .ToList();
        }


        private void UpdateField(FormDistributionDataIn dataIn, FormDistribution formDistribution)
        {
            FormFieldDistributionDataIn fieldDataIn = dataIn.Fields.FirstOrDefault();
            if (fieldDataIn != null)
            {
                FormFieldDistribution fieldDb = formDistribution.Fields.FirstOrDefault(x => x.Id.Equals(fieldDataIn.Id));
                if (fieldDb != null)
                {
                    formDistribution.Fields[formDistribution.Fields.IndexOf(fieldDb)] = Mapper.Map<FormFieldDistribution>(fieldDataIn);
                }
            }
        }
        private FormDistribution Create(Form form)
        {
            FormDistribution formDistribution = GetFromForm(form);
            formDistributionDAL.InsertOrUpdate(formDistribution);

            return formDistribution;
        }

        private void SetRelatedFieldsLabels(FormDistributionDataOut formDistributionDataOut, Form form)
        {
            List<Field> fields = form.GetAllFields();
            foreach (var field in formDistributionDataOut.Fields)
            {
                foreach (var rel in field.RelatedVariables)
                {
                    rel.Label = fields.FirstOrDefault(x => x.Id == rel.Id).Label;
                }
            }
        }

        private FormDistribution GetFromForm(Form form)
        {
            return new FormDistribution()
            {
                EntryDatetime = form.EntryDatetime,
                ThesaurusId = form.ThesaurusId,
                Title = form.Title,
                Fields = GetDistributionFields(form.GetAllFields().Where(x => x is FieldCheckbox || x is FieldRadio || x is FieldSelect || x is FieldNumeric).ToList()),
                VersionId = form.Version.Id
            };
        }

        private List<FormFieldDistribution> GetDistributionFields(List<Field> fields)
        {
            List<FormFieldDistribution> result = new List<FormFieldDistribution>();
            foreach (Field field in fields)
            {
                FormFieldDistribution fieldDistribution = GetDistributionField(field);
                result.Add(fieldDistribution);
            }

            return result;
        }

        private FormFieldDistribution GetDistributionField(Field field)
        {
            FormFieldDistribution fieldDistribution = new FormFieldDistribution()
            {
                Id = field.Id,
                Label = field.Label,
                RelatedVariables = new List<Domain.Entities.Distribution.RelatedVariable>(),
                ThesaurusId = field.ThesaurusId,
                Type = field.Type,
                ValuesAll = new List<FormFieldDistributionSingleParameter>()
                 {
                     new FormFieldDistributionSingleParameter()
                     {
                         NormalDistributionParameters = new FormFieldNormalDistributionParameters(),
                         Values = field is FieldSelectable ? (field as FieldSelectable).Values.Select(x => new FormFieldValueDistribution()
                         {
                             Label = x.Label,
                             ThesaurusId = x.ThesaurusId,
                             Value = x.Value
                         }).ToList() : null
                     }
                 }
            };

            return fieldDistribution;
        }

        public Field GetFormField(int thesaurusId, string versionId, UserCookieData userCookieData, string fieldId)
        {
            Form form = formDAL.GetFormByThesaurusAndLanguageAndVersionAndOrganization(thesaurusId, userCookieData.ActiveOrganization, userCookieData.ActiveLanguage, versionId);
            return form.GetFieldById(fieldId);
        }
    }
}
