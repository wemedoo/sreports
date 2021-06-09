using AutoMapper;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.DiagnosticReport;
using sReportsV2.DTOs.EpisodeOfCare;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.FormInstance;
using sReportsV2.DTOs.FormInstance.DataIn;
using sReportsV2.DTOs.FormInstance.DataOut;
using sReportsV2.DTOs.Patient.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class FormInstanceProfile : Profile
    {
        public FormInstanceProfile()
        {
            CreateMap<FormInstancePerDomain, FormInstancePerDomainDataOut>()
           .ForMember(d => d.Count, opt => opt.MapFrom(src => src.Count))
           .ForMember(d => d.Domain, opt => opt.MapFrom(src => src.Domain.ToString()));

            CreateMap<FormInstance, PatientFormInstanceDataOut>()
            .ReverseMap();

            CreateMap<FormInstance, FormInstanceTableDataOut>()
           .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
           .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
           .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
           .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
           .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
           .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
           .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
           //.ForMember(d => d.User, opt => opt.MapFrom(src => src.User))
           //.ForMember(d => d.Patient, opt => opt.MapFrom(src => src.Patient))
           .ReverseMap();

            CreateMap<FormInstance, FormInstanceDataOut>()
            .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(d => d.FormDefinitionId, opt => opt.MapFrom(src => src.FormDefinitionId))
            .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
            .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
            .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
            .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
            //.ForMember(d => d.User, opt => opt.MapFrom(src => src.User))
            //.ForMember(d => d.Patient, opt => opt.MapFrom(src => src.Patient))
            .ForMember(d => d.Referrals, opt => opt.Ignore())
            .ReverseMap();

            CreateMap<FormInstance, FormDataOut>()
            .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
            .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
            .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
            .ForMember(d => d.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(d => d.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(d => d.FormState, opt => opt.MapFrom(src => src.FormState))
            .ReverseMap();

            CreateMap<FormInstance, DiagnosticReportDataOut>()
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
            //.ForMember(d => d.User, opt => opt.MapFrom(src => src.User))
            .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
            .ReverseMap();

            CreateMap<FormInstanceFilterDataIn, FormInstanceFilterData>().ReverseMap();

            CreateMap<FormInstance, FormInstanceReferralDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ReverseMap();

            CreateMap<FormInstanceCovidFilter, FormInstanceCovidFilterDataIn>()
                .ReverseMap();

        }
    }
}