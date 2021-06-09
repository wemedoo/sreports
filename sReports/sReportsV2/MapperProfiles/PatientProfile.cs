using AutoMapper;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.CTCAE.DataIn;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.DTOs.Patient;
using sReportsV2.DTOs.Patient.DataIn;
using sReportsV2.DTOs.Patient.DataOut;
using System.Collections.Generic;

namespace sReportsV2.MapperProfiles
{
    public class PatientProfile : Profile
    {
        public PatientProfile()
        {
            CreateMap<Patient, PatientTableDataOut>()
             .ForMember(o => o.FirstName, opt => opt.MapFrom(src => src.Name.Given))
             .ForMember(o => o.LastName, opt => opt.MapFrom(src => src.Name.Family))
             .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
             .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Address, AddressDTO>().ReverseMap();
            CreateMap<Domain.Sql.Entities.Common.Telecom, TelecomDTO>().ReverseMap();
            CreateMap<Domain.Sql.Entities.Common.Name, NameDTO>().ReverseMap();
            CreateMap<Contact, ContactDTO>().ReverseMap();

            CreateMap<PatientDataIn, Patient>()
                  .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id))
                  .ForMember(o => o.Active, opt => opt.MapFrom(src => src.Activity))
                  .ForMember(o => o.Name, opt => opt.MapFrom(src => new Domain.Sql.Entities.Common.Name()
                  {
                      Family = src.FamilyName,
                      Given = src.Name
                  }))
                  .ForMember(o => o.ContactPerson, opt => opt.MapFrom(src => src.ContactPerson))
                  .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
                  .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                  .ForMember(o => o.MultipleB, opt => opt.MapFrom(src => new MultipleBirth()
                  {
                      isMultipleBorn = src.MultipleBirth,
                      Number = src.MultipleBirthNumber
                  }))
                  .ForMember(o => o.Gender, opt => opt.MapFrom(src => src.Gender))
                  .ForMember(o => o.Telecoms, opt => opt.MapFrom(src => src.Telecoms))
                  .ForMember(o => o.Identifiers, opt => opt.MapFrom(src => src.Identifiers))
                  .ForMember(o => o.Communications, opt => opt.MapFrom(src => src.Communications))
                  .ForMember(o => o.Addresss, opt => opt.MapFrom(src => src.Address))
                  .ForAllOtherMembers(opts => opts.Ignore());
          
            CreateMap<Patient, Contact>()
                .ForMember(o => o.Relationship, opt => opt.MapFrom(src => src.ContactPerson.Relationship))
                .ForMember(o => o.Name, opt => opt.MapFrom(src => src.ContactPerson.Name))
                .ForMember(o => o.Gender, opt => opt.MapFrom(src => src.ContactPerson.Gender))
                .ForMember(o => o.Address, opt => opt.MapFrom(src => src.ContactPerson.Address))
                .ForMember(o => o.Telecoms, opt => opt.MapFrom(src => src.ContactPerson.Telecoms))
                .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Contact, ContactDTO>().ReverseMap();

            CreateMap<PatientDataOut, Contact>()
               .ForMember(o => o.Relationship, opt => opt.MapFrom(src => src.Relationship))
               .ForMember(o => o.Name, opt => opt.MapFrom(src => new Name(src.ContactName, src.ContactFamily)))
               .ForMember(o => o.Gender, opt => opt.MapFrom(src => src.ContactGender))
               .ForMember(o => o.Address, opt => opt.MapFrom(src => src.ContactAddress))
               .ForMember(o => o.Telecoms, opt => opt.MapFrom(src => src.ContactTelecoms));

            CreateMap<Patient, PatientDataIn>()
             .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id.ToString()))
             .ForMember(o => o.Activity, opt => opt.MapFrom(src => src.Active))
             .ForMember(o => o.Name, opt => opt.MapFrom(src => src.Name.Given))
             .ForMember(o => o.FamilyName, opt => opt.MapFrom(src => src.Name.Family))
             .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
             .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
             .ForMember(o => o.MultipleBirth, opt => opt.MapFrom(src => src.MultipleB.isMultipleBorn))
             .ForMember(o => o.MultipleBirthNumber, opt => opt.MapFrom(src => src.MultipleB.Number))
             .ForMember(o => o.ContactPerson, opt => opt.MapFrom(src => src.ContactPerson))
             .ForMember(o => o.Gender, opt => opt.MapFrom(src => src.Gender))
             .ForMember(o => o.Telecoms, opt => opt.MapFrom(src => src.Telecoms))
             .ForMember(o => o.Address, opt => opt.MapFrom(src => src.Addresss))
             .ForMember(o => o.Communications, opt => opt.MapFrom(src => src.Communications))
            .ForMember(o => o.Identifiers, opt => opt.MapFrom(src => src.Identifiers))
            .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<Patient, CTCAEPatient>()
            .ForMember(o => o.PatientId, opt => opt.MapFrom(src => src.Id))
            .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<CTCAEPatient, Patient>()
           .ForMember(o => o.Id, opt => opt.MapFrom(src => src.PatientId))
           .ForAllOtherMembers(opts => opts.Ignore());

            CreateMap<PatientDataOut, Patient>()
              .ForMember(o => o.Id, opt => opt.MapFrom(src =>src.Id))
              .ForMember(o => o.Active, opt => opt.MapFrom(src => src.Activity))
              .ForMember(o => o.Name, opt => opt.MapFrom(src => new Name()
              {
                  Family = src.FamilyName,
                  Given = src.Name
              }))
              .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
              .ForMember(o => o.MultipleB, opt => opt.MapFrom(src => new MultipleBirth()
              {
                  isMultipleBorn = src.MultipleBirth,
                  Number = src.MultipleBirthNumber
              }))
              .ForMember(o => o.Addresss, opt => opt.MapFrom(src => src.Address))
              .ForMember(o => o.Gender, opt => opt.MapFrom(src => src.Gender))
              .ForMember(o => o.ContactPerson, opt => opt.MapFrom(src => src))
              .ForMember(o => o.Telecoms, opt => opt.MapFrom(src => src.Telecoms))
              .ForMember(o => o.Communications, opt => opt.MapFrom(src => src.Communications))
              .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
              .ForMember(o => o.Identifiers, opt => opt.MapFrom(src => src.Identifiers))
              .ReverseMap();    

            CreateMap<Patient, PatientDataOut>()
             .ForMember(o => o.Id, opt => opt.MapFrom(src => src.Id.ToString()))
             .ForMember(o => o.Activity, opt => opt.MapFrom(src => src.Active))
             .ForMember(o => o.Name, opt => opt.MapFrom(src => src.Name.Given))
             .ForMember(o => o.FamilyName, opt => opt.MapFrom(src => src.Name.Family))
             .ForMember(o => o.BirthDate, opt => opt.MapFrom(src => src.BirthDate))
             .ForMember(o => o.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
             .ForMember(o => o.MultipleBirth, opt => opt.MapFrom(src => src.MultipleB.isMultipleBorn))
             .ForMember(o => o.MultipleBirthNumber, opt => opt.MapFrom(src => src.MultipleB.Number))
             .ForMember(o => o.ContactGender, opt => opt.MapFrom(src => src.ContactPerson.Gender))
             .ForMember(o => o.ContactName, opt => opt.MapFrom(src => src.ContactPerson.Name.Given))
             .ForMember(o => o.ContactFamily, opt => opt.MapFrom(src => src.ContactPerson.Name.Family))
             .ForMember(o => o.Relationship, opt => opt.MapFrom(src => src.ContactPerson.Relationship))
             .ForMember(o => o.Gender, opt => opt.MapFrom(src => src.Gender))
             .ForMember(o => o.Address, opt => opt.MapFrom(src =>src.Addresss ))
             .ForMember(o => o.ContactTelecoms, opt => opt.MapFrom(src => src.ContactPerson.Telecoms))
             .ForMember(o => o.ContactAddress, opt => opt.MapFrom(src => src.ContactPerson.Address))
             .ForMember(o => o.Communications, opt => opt.MapFrom(src => src.Communications))
             .ForMember(o => o.Identifiers, opt => opt.MapFrom(src => src.Identifiers))
             .ReverseMap();

            CreateMap<PatientFilterDataIn, PatientFilter>();

            CreateMap<Communication, CommunicationDTO>().ReverseMap();

        }
    }
}