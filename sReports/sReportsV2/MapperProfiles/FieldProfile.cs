using AutoMapper;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.FieldEntity.Custom;
using sReportsV2.Domain.Entities.FieldEntity.Custom.Action;
using sReportsV2.DTOs.Field.DataIn;
using sReportsV2.DTOs.Field.DataIn.Custom;
using sReportsV2.DTOs.Field.DataIn.Custom.Action;
using sReportsV2.DTOs.Field.DataOut;
using sReportsV2.DTOs.Field.DataOut.Custom;
using sReportsV2.DTOs.Field.DataOut.Custom.Action;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.MapperProfiles
{
    public class FieldProfile : Profile
    {
        public FieldProfile()
        {
            CreateMap<Field, FieldDataOut>();

            CreateMap<FieldCalculative, FieldCalculativeDataOut>()
                .IncludeBase<Field, FieldDataOut>();

            CreateMap<FieldCheckbox, FieldCheckboxDataOut>()
                 .IncludeBase<FieldSelectable, FieldSelectableDataOut>();

            CreateMap<FieldDate, FieldDateDataOut>()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldDatetime, FieldDatetimeDataOut>()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldEmail, FieldEmailDataOut>()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldFile, FieldFileDataOut>()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldNumeric, FieldNumericDataOut>()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldRadio, FieldRadioDataOut>()
                 .IncludeBase<FieldSelectable, FieldSelectableDataOut>();

            CreateMap<FieldRegex, FieldRegexDataOut>()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldSelect, FieldSelectDataOut>()
                 .IncludeBase<FieldSelectable, FieldSelectableDataOut>();

            CreateMap<FieldTextArea, FieldTextAreaDataOut>()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldText, FieldTextDataOut>()
                 .IncludeBase<FieldString, FieldStringDataOut>();

            CreateMap<FieldString, FieldStringDataOut>()
                 .IncludeBase<Field, FieldDataOut>();

            CreateMap<FieldSelectable, FieldSelectableDataOut>()
                 .IncludeBase<Field, FieldDataOut>();

            CreateMap<FieldDataIn, Field>();

            CreateMap<FieldCalculativeDataIn, FieldCalculative>()
                .IncludeBase<FieldDataIn, Field>();

            CreateMap<FieldCheckboxDataIn, FieldCheckbox>()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectable>();

            CreateMap<FieldDateDataIn, FieldDate>()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldDatetimeDataIn, FieldDatetime>()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldEmailDataIn, FieldEmail>()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldFileDataIn, FieldFile>()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldNumericDataIn, FieldNumeric>()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldRadioDataIn, FieldRadio>()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectable>();

            CreateMap<FieldRegexDataIn, FieldRegex>()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldSelectDataIn, FieldSelect>()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectable>();

            CreateMap<FieldTextAreaDataIn, FieldTextArea>()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldTextDataIn, FieldText>()
                 .IncludeBase<FieldStringDataIn, FieldString>();

            CreateMap<FieldStringDataIn, FieldString>()
                 .IncludeBase<FieldDataIn, Field>();

            CreateMap<FieldSelectableDataIn, FieldSelectable>()
                 .IncludeBase<FieldDataIn, Field>();


            CreateMap<FieldDataIn, FieldDataOut>();

            CreateMap<FieldCalculativeDataIn, FieldCalculativeDataOut>()
                .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<FieldCheckboxDataIn, FieldCheckboxDataOut>()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectableDataOut>();

            CreateMap<FieldDateDataIn, FieldDateDataOut>()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldDatetimeDataIn, FieldDatetimeDataOut>()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldEmailDataIn, FieldEmailDataOut>()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldFileDataIn, FieldFileDataOut>()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldNumericDataIn, FieldNumericDataOut>()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldRadioDataIn, FieldRadioDataOut>()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectableDataOut>();

            CreateMap<FieldRegexDataIn, FieldRegexDataOut>()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldSelectDataIn, FieldSelectDataOut>()
                 .IncludeBase<FieldSelectableDataIn, FieldSelectableDataOut>();

            CreateMap<FieldTextAreaDataIn, FieldTextAreaDataOut>()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldTextDataIn, FieldTextDataOut>()
                 .IncludeBase<FieldStringDataIn, FieldStringDataOut>();

            CreateMap<FieldStringDataIn, FieldStringDataOut>()
                 .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<FieldSelectableDataIn, FieldSelectableDataOut>()
                 .IncludeBase<FieldDataIn, FieldDataOut>();


            CreateMap<CustomFieldButtonDataIn, CustomFieldButton>()
                    .IncludeBase<FieldDataIn, Field>();

            CreateMap<CustomFieldButtonDataIn, CustomFieldButtonDataOut>()
                    .IncludeBase<FieldDataIn, FieldDataOut>();

            CreateMap<CustomFieldButton, CustomFieldButtonDataOut>()
                    .IncludeBase<Field, FieldDataOut>();

            CreateMap<CustomAction, CustomActionDataOut>();
            CreateMap<CustomActionDataIn, CustomActionDataOut>();
            CreateMap<CustomActionDataIn, CustomAction>();


            /*Javascript Action mapping*/
            CreateMap<JavascriptActionDataIn, JavascriptAction>()
                .IncludeBase<CustomActionDataIn, CustomAction>();

            CreateMap<JavascriptAction, JavascriptActionDataOut>()
                .IncludeBase<CustomAction, CustomActionDataOut>();

            CreateMap<JavascriptActionDataIn, JavascriptActionDataOut>()
                .IncludeBase<CustomActionDataIn, CustomActionDataOut>();

            /*Custom Action mapping*/
            CreateMap<ControllerActionDataIn, ControllerAction>()
                .IncludeBase<CustomActionDataIn, CustomAction>();

            CreateMap<ControllerAction, ControllerActionDataOut>()
                .IncludeBase<CustomAction, CustomActionDataOut>();

            CreateMap<ControllerActionDataIn, ControllerActionDataOut>()
                .IncludeBase<CustomActionDataIn, CustomActionDataOut>();
        }
    }
}