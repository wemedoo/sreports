using AutoMapper;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.FormDistribution.DataIn;
using sReportsV2.DTOs.FormDistribution.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.MapperProfiles
{
    public class FormDistributionProfile : Profile
    {
        public FormDistributionProfile()
        {

            CreateMap<FormDistribution, FormDistributionDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(d => d.Fields, opt => opt.MapFrom(src => src.Fields))
                .ForMember(d => d.VersionId, opt => opt.MapFrom(src => src.VersionId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<FormDistribution, FormDistributionTableDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForAllOtherMembers(opts => opts.Ignore());


            CreateMap<FormFieldValueDistribution, FormFieldValueDistributionDataOut>()
                .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
                .ForMember(d => d.SuccessProbability, opt => opt.MapFrom(src => src.SuccessProbability))
                .ForMember(d => d.Value, opt => opt.MapFrom(src => src.Value))
                .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Form, FormDistribution>()
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForMember(d => d.Fields, opt => opt.MapFrom(src => src.GetAllNonPatientFields()))
                .ForMember(d => d.VersionId, opt => opt.MapFrom(src => src.Version.Id))

                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<FieldSelectable, FormFieldDistribution>()
                .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
                .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Values, opt => opt.MapFrom(src => src.Values))
                .ForAllOtherMembers(opts => opts.Ignore());


            CreateMap<FieldString, FormFieldDistribution>()
                .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
                .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<FormFieldValue, FormFieldValueDistribution>()
                .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
                .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForMember(d => d.Value, opt => opt.MapFrom(src => src.Value))
                .ForAllOtherMembers(opts => opts.Ignore());


            CreateMap<FormFieldDistributionSingleParameterDataIn, FormFieldDistributionSingleParameter>();
            CreateMap<SingleDependOnValueDataIn, SingleDependOnValue>();
            CreateMap<FormFieldNormalDistributionParametersDataIn, FormFieldNormalDistributionParameters>();
            CreateMap<FormFieldValueDistributionDataIn, FormFieldValueDistribution>();
            CreateMap<FormFieldDistributionDataIn, FormFieldDistribution>()
                .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(d => d.ValuesAll, opt => opt.MapFrom(src => src.Values))
                .ForMember(d => d.RelatedVariables, opt => opt.MapFrom(src => src.RelatedVariables))
                .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<FormFieldDistributionSingleParameter, FormFieldDistributionSingleParameterDataOut>();
            CreateMap<SingleDependOnValue, SingleDependOnValueDataOut>();
            CreateMap<FormFieldNormalDistributionParameters, FormFieldNormalDistributionParametersDataOut>();
            CreateMap<sReportsV2.Domain.Entities.Distribution.RelatedVariable, sReportsV2.DTOs.FormDistribution.DataIn.RelatedVariable>().ReverseMap();
            CreateMap<FormFieldValueDistribution, FormFieldValueDistributionDataOut>();
            CreateMap<FormFieldDistribution, FormFieldDistributionDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(d => d.ValuesCombination, opt => opt.MapFrom(src => src.ValuesAll))
                .ForMember(d => d.RelatedVariables, opt => opt.MapFrom(src => src.RelatedVariables))
                .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForAllOtherMembers(opts => opts.Ignore());


        }
    }
}