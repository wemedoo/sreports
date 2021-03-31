using AutoMapper;
using Hl7.Fhir.Model;
using sReportsV2.Api.DTOs.EpisodeOfCare.DataIn;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.MapperProfiles
{
    public class EpisodeOfCareProfile : Profile
    {
        public EpisodeOfCareProfile()
        {
            CreateMap<EpisodeOfCareFilterDataIn, EpisodeOfCareFhirFilter>();

            CreateMap<EpisodeOfCareEntity, EpisodeOfCare>()
                .ForMember(org => org.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(eoc => eoc.StatusHistory, opt => opt.MapFrom(src => src.ListHistoryStatus))
                .ForMember(eoc => eoc.Patient, opt => opt.MapFrom(src => new ResourceReference() { Reference = src.PatientId }))
                .ForMember(eoc => eoc.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(eoc => eoc.Type, opt => opt.MapFrom(src => new List<CodeableConcept>() { new CodeableConcept() { Text = src.Type } }))
                .ForMember(eoc => eoc.Diagnosis, opt => opt.MapFrom(src => new List<EpisodeOfCare.DiagnosisComponent>()
                {
                    new EpisodeOfCare.DiagnosisComponent()
                    {
                        Rank = int.Parse(src.DiagnosisRank),
                        Condition = new ResourceReference() { Reference = src.DiagnosisCondition },
                        Role = new CodeableConcept() { Text = src.DiagnosisRole.ToString() }
                    }
                }))
                .ForMember(eoc => eoc.Period, opt => opt.MapFrom(src => new Period()
                {
                    StartElement = new FhirDateTime() { Value = src.Period.Start.ToString() },
                    EndElement = new FhirDateTime() { Value = src.Period.End != null ? src.Period.End.ToString() : null }
                }))
                .ForMember(eoc => eoc.ManagingOrganization, opt => opt.MapFrom(src => new ResourceReference() { Reference = src.OrganizationRef }))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<EpisodeOfCareStatus, EpisodeOfCare.StatusHistoryComponent>()
                .ForMember(eoc => eoc.Status, opt => opt.MapFrom(src => src.StatusValue))
                .ForMember(eoc => eoc.Period, opt => opt.MapFrom(src => new Period()
                {
                    StartElement = new FhirDateTime() { Value = src.StartTime.ToString() },
                    EndElement = new FhirDateTime() { Value = src.EndTime.ToString() }
                }))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<string, CodeableConcept>()
                .ForMember(org => org.Coding, opt => opt.MapFrom(src => new List<Coding>() { new Coding() { Display = src } }))
                .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}
