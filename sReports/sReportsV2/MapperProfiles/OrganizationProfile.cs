using AutoMapper;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.OrganizationEntities;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Organization.DataIn;
using sReportsV2.DTOs.Organization.DataOut;
using sReportsV2.SqlDomain.Filter;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<OrganizationDataOut, Organization>()
                .ForMember(d => d.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(d => d.Alias, opt => opt.MapFrom(src => src.Alias))
                //.ForMember(d => d.ClinicalDomains, opt => opt.MapFrom(src => src.ClinicalDomain))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Identifiers, opt => opt.MapFrom(src => src.Identifiers))
                .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(d => d.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(d => d.PrimaryColor, opt => opt.MapFrom(src => src.PrimaryColor))
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
                .ForMember(d => d.SecondaryColor, opt => opt.MapFrom(src => src.SecondaryColor))
                .ForMember(d => d.Telecoms, opt => opt.MapFrom(src => src.Telecoms))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Type))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<OrganizationDataIn, Organization>()
            .ForMember(d => d.AddressId, opt => opt.MapFrom(src => src.AddressId))
            .ForMember(d => d.Address, opt => opt.MapFrom(src => src.Address))
            .ForMember(d => d.Alias, opt => opt.MapFrom(src => src.Alias))
            //.ForMember(d => d.ClinicalDomains, opt => opt.MapFrom(src => src.ClinicalDomain))
            .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.Identifiers, opt => opt.MapFrom(src => src.Identifiers))
            .ForMember(d => d.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
            .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(d => d.PrimaryColor, opt => opt.MapFrom(src => src.PrimaryColor))
            .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.FromBase64String(src.RowVersion)))
            .ForMember(d => d.SecondaryColor, opt => opt.MapFrom(src => src.SecondaryColor))
            .ForMember(d => d.Telecoms, opt => opt.MapFrom(src => src.Telecom))
            .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Type))
            .ForAllOtherMembers(x => x.Ignore());


            CreateMap<Organization, OrganizationDataOut>()
                .ForMember(d => d.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(d => d.Alias, opt => opt.MapFrom(src => src.Alias))
                .ForMember(d => d.ClinicalDomain, opt => opt.MapFrom(src => src.ClinicalDomains
                .Where(x => !x.IsDeleted)
                .Select(x => (DocumentClinicalDomain)x.ClinicalDomainId).ToList()))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(d => d.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Identifiers, opt => opt.MapFrom(src => src.Identifiers))
                .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
                .ForMember(d => d.LogoUrl, opt => opt.MapFrom(src => src.LogoUrl))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(d => d.PrimaryColor, opt => opt.MapFrom(src => src.PrimaryColor))
                .ForMember(d => d.RowVersion, opt => opt.MapFrom(src => Convert.ToBase64String(src.RowVersion)))
                .ForMember(d => d.SecondaryColor, opt => opt.MapFrom(src => src.SecondaryColor))
                .ForMember(d => d.Telecoms, opt => opt.MapFrom(src => src.Telecoms))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(d => d.Parent, opt => opt.MapFrom(src => src.OrganizationRelation.Parent))
                .ForAllOtherMembers(x => x.Ignore());

            CreateMap<OrganizationFilterDataIn, OrganizationFilter>();

            CreateMap<TelecomDTO, Domain.Sql.Entities.Common.Telecom>()
                .ReverseMap();

            CreateMap<IdentifierDataIn, Identifier>()
                .ReverseMap();

            CreateMap<OrganizationUsersCountDataOut, OrganizationUsersCount>()
                .ReverseMap();
        }
    }
}