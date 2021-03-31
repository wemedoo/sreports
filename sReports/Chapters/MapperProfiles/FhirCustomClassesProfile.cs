using Hl7.Fhir.Model;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.FieldEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;


namespace Chapters.MapperProfiles
{
    public class FhirCustomClassesProfile : Profile
    {
        public FhirCustomClassesProfile()
        {
            CreateMap<O4CodeableConcept, CodeableConcept>()
                    .ForMember(p => p.Coding, opt => opt.MapFrom(src => new List<O4CodeableConcept>() { new O4CodeableConcept(src.System, src.Version, src.Code, src.Value, src.VersionPublishDate) }))
                    .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<O4CodeableConcept, Coding>()
                    .ForMember(p => p.Code, opt => opt.MapFrom(src => src.Code))
                    .ForMember(p => p.System, opt => opt.MapFrom(src => src.System))
                    .ForMember(p => p.Display, opt => opt.MapFrom(src => src.Value))
                    .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<O4Identifier, Identifier>()
                    .ForMember(p => p.Value, opt => opt.MapFrom(src => src.Value))
                    .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<O4ResourceReference, ResourceReference>()
                    .ForMember(p => p.Identifier, opt => opt.MapFrom(src => src.Identifier))
                    .ForMember(p => p.Reference, opt => opt.MapFrom(src => src.Reference))
                     .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Field, Procedure>()
                    .ForMember(p => p.Subject, opt => opt.MapFrom(src => src.Subject))
                    .ForMember(p => p.Status, opt => opt.MapFrom(src => EventStatus.Completed))
                    .ForMember(p => p.Code, opt => opt.MapFrom(src => src.Code))
                    .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Field, Observation>()
                    .ForMember(p => p.Subject, opt => opt.MapFrom(src => src.Subject))
                    .ForMember(o => o.Status, opt => opt.MapFrom(src => ObservationStatus.Final))
                    .ForMember(o => o.Performer, opt => opt.MapFrom(src => new List<O4ResourceReference>() { new O4ResourceReference() { Reference = src.Performer} }))
                    .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Field, DiagnosticReport>()
                    .ForMember(dr => dr.Status, opt => opt.MapFrom(src => DiagnosticReport.DiagnosticReportStatus.Final))
                    .ForMember(p => p.Code, opt => opt.MapFrom(src => src.Code))
                    .ForMember(p => p.Result, opt => opt.MapFrom(src => src.Result))
                    .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}
