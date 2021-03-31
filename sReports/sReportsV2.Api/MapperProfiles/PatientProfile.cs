using AutoMapper;
using Hl7.Fhir.Model;
using sReportsV2.Api.DTOs.Patient.DataIn;
using sReportsV2.Domain.Entities.PatientEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sReportsV2.Api.MapperProfiles
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<PatientFilterDataIn, PatientFhirFilter>();

            CreateMap<PatientEntity, Patient>()
              .ForMember(org => org.Id, opt => opt.MapFrom(src => src.Id))
              .ForMember(pat => pat.Active, opt => opt.MapFrom(src => src.Active))
              .ForMember(pat => pat.Name, opt => opt.MapFrom(src => new List<Name>() { src.Name }))
              .ForMember(pat => pat.Gender, opt => opt.MapFrom(src => src.Gender))
              .ForMember(pat => pat.BirthDate, opt => opt.MapFrom(src => src.BirthDate.ToString()))
              .ForMember(pat => pat.Address, opt => opt.MapFrom(src => new List<Domain.Entities.PatientEntities.Address>() { src.Addresss }))
              .ForMember(pat => pat.MultipleBirth, opt => opt.MapFrom(src => new FhirBoolean() { Value = src.MultipleB.isMultipleBorn }))
              //.ForMember(pat => pat.MultipleBirth, opt => opt.MapFrom(src => new Integer() { Value = src.MultipleB.Number }))
              .ForMember(pat => pat.Contact, opt => opt.MapFrom(src => new List<Contact>() { src.ContactPerson }))
              .ForMember(pat => pat.Communication, opt => opt.MapFrom(src => src.Communications))
              .ForMember(org => org.Telecom, opt => opt.MapFrom(src => src.Telecoms))
              .ForMember(org => org.Identifier, opt => opt.MapFrom(src => src.Identifiers))
              .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Domain.Entities.PatientEntities.Address, Hl7.Fhir.Model.Address>()
               .ForMember(addr => addr.City, opt => opt.MapFrom(src => src.City))
               .ForMember(addr => addr.State, opt => opt.MapFrom(src => src.State))
               .ForMember(addr => addr.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
               .ForMember(addr => addr.Country, opt => opt.MapFrom(src => src.Country))
               .ForMember(addr => addr.District, opt => opt.MapFrom(src => src.Street))
               .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Domain.Entities.PatientEntities.Communication, Patient.CommunicationComponent>()
             .ForMember(addr => addr.Preferred, opt => opt.MapFrom(src => src.Preferred))
             .ForMember(addr => addr.Language, opt => opt.MapFrom(src => new CodeableConcept() { Text = src.Language }))
             .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Name, HumanName>()
                .ForMember(n => n.Family, opt => opt.MapFrom(src => src.Family))
                .ForMember(n => n.Given, opt => opt.MapFrom(src => new List<string>() { src.Given }))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Contact, Patient.ContactComponent>()
                .ForMember(pat => pat.Relationship, opt => opt.MapFrom(src => new List<CodeableConcept>() { new CodeableConcept() { Text = src.Relationship } }))
                .ForMember(pat => pat.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(org => org.Telecom, opt => opt.MapFrom(src => src.Telecoms))
                .ForMember(pat => pat.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(pat => pat.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Telecom, ContactPoint>()
               .ForMember(p => p.Value, opt => opt.MapFrom(src => src.Value))
               .ForAllOtherMembers(opts => opts.Ignore());
        }
    }
}
