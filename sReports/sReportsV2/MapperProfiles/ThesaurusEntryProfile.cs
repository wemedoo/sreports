using AutoMapper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Administration;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using sReportsV2.DTOs.ThesaurusEntry.DTO;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class ThesaurusEntryProfile : Profile
    {
        public ThesaurusEntryProfile()
        {
            CreateMap<Version, VersionDataOut>()
                .ReverseMap();

            CreateMap<SimilarTerm, SimilarTermDTO>()
                .ReverseMap();

            CreateMap<CodeSystem, CodeSystemDataOut>()
                .ReverseMap();

            CreateMap<ThesaurusEntry, ThesaurusEntryDataOut>()
                .ReverseMap();

            CreateMap<ThesaurusEntryDataOut, ThesaurusEntry>()
                .ReverseMap();

            CreateMap<ThesaurusEntryTranslation, ThesaurusEntryTranslationDTO>()
                .ReverseMap();

            CreateMap<ThesaurusEntryTranslationDTO, ThesaurusEntryTranslation>()
                .ReverseMap();

            CreateMap<ThesaurusEntryTranslationDTO, sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntryTranslation>()
                .ReverseMap();

            CreateMap<ThesaurusEntryDataIn, ThesaurusEntry>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.Translations, opt => opt.MapFrom(src => src.Translations))
                .ForMember(d => d.State, opt => opt.MapFrom(src => src.State))
                .ReverseMap();


            CreateMap<ThesaurusEntryTranslationDataIn, ThesaurusEntryTranslation>()
                .ForMember(d => d.Abbreviations, opt => opt.MapFrom(src => src.Abbreviations))
                .ForMember(d => d.Definition, opt => opt.MapFrom(src => src.Definition))
                .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(d => d.PreferredTerm, opt => opt.MapFrom(src => src.PreferredTerm))
                .ForMember(d => d.Synonyms, opt => opt.MapFrom(src => src.Synonyms))
                .ReverseMap();

            CreateMap<ThesaurusEntryTree, ThesaurusEntryTreeDataOut>()
                .ReverseMap();

            CreateMap<ThesaurusEntryCodingSystem, ThesaurusEntryCodingSystemDTO>()
                .ReverseMap();

            CreateMap<ThesaurusEntryCodingSystemDataIn, ThesaurusEntryCodingSystem>();

            CreateMap<AdministrativeData, AdministrativeDataDataOut>()
                .ReverseMap();

            CreateMap<ThesaurusEntryFilterDataIn, ThesaurusEntryFilterData>()
                .ReverseMap();

            CreateMap<ThesaurusReviewFilterDataIn, ThesaurusReviewFilterData>()
                .ReverseMap();

            CreateMap<AdministrationFilterDataIn, ThesaurusEntryFilterData>()
                .ForMember(d => d.PreferredTerm, opt => opt.MapFrom(src => src.PreferredTerm))
                .ForMember(d => d.Page, opt => opt.MapFrom(src => src.Page))
                .ForMember(d => d.PageSize, opt => opt.MapFrom(src => src.PageSize))
                .ReverseMap();

            CreateMap<O4CodeableConcept, O4CodeableConceptDataIn>().ReverseMap();
            CreateMap<O4CodeableConcept, O4CodeableConceptDataOut>()
                .ForMember(x => x.System, opt => opt.Ignore())
                .ReverseMap();

        }
    }
}