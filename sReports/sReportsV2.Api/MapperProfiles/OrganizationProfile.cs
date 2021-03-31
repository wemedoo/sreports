using AutoMapper;
using Hl7.Fhir.Model;
using sReportsV2.Api.DTOs.Organization.DataIn;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Entities.PatientEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.MapperProfiles
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<OrganizationFilterDataIn, OrganizationFilter>();

            CreateMap<Domain.Entities.OrganizationEntities.Organization, Hl7.Fhir.Model.Organization>()
                .ForMember(org => org.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(org => org.Active, opt => opt.MapFrom(src => src.Activity))
                .ForMember(org => org.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(org => org.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(org => org.Alias, opt => opt.MapFrom(src => new List<FhirString> { new FhirString { Value = src.Alias } }))
                .ForMember(org => org.Telecom, opt => opt.MapFrom(src => src.Telecom))
                .ForMember(pat => pat.Address, opt => opt.MapFrom(src => new List<Domain.Entities.PatientEntities.Address>() { src.Address }))
                .ForMember(org => org.PartOf, opt => opt.MapFrom(src => src.PartOf))
                .ForMember(org => org.Identifier, opt => opt.MapFrom(src => src.Identifiers))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<string, CodeableConcept>()
                .ForMember(org => org.Coding, opt => opt.MapFrom(src => new List<Coding>() { new Coding() { Display = src } }))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Telecom, ContactPoint>()
               .ForMember(p => p.Value, opt => opt.MapFrom(src => src.Value))
               .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<string, ResourceReference>()
               .ForMember(enc => enc.Display, opt => opt.MapFrom(src => src))
               .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<IdentifierEntity, Identifier>()
               .ForMember(p => p.System, opt => opt.MapFrom(src => src.System))
               .ForMember(p => p.Value, opt => opt.MapFrom(src => src.Value))
               .ForMember(p => p.Use, opt => opt.MapFrom(src => src.Use))
               .ForMember(p => p.Type, opt => opt.MapFrom(src => src.Type))
               .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}
