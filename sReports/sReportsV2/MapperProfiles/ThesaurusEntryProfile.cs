using AutoMapper;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.CustomFHIRClasses;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.Models.ThesaurusEntry;

namespace sReportsV2.MapperProfiles
{
    public class ThesaurusEntryProfile : Profile
    {
        public ThesaurusEntryProfile()
        {
            CreateMap<CodeSystem, CodeSystemDataOut>()
                .ReverseMap();

            CreateMap<ThesaurusEntry, ThesaurusEntryViewModel>()
                .ReverseMap();

            CreateMap<ThesaurusEntryViewModel, ThesaurusEntry>()
                .ReverseMap();

            CreateMap<ThesaurusEntryTranslation, ThesaurusEntryTranslationViewModel>()
                .ReverseMap();

            CreateMap<ThesaurusEntryTranslationViewModel, ThesaurusEntryTranslation>()
                .ReverseMap();

            CreateMap<ThesaurusEntryDataIn, ThesaurusEntry>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(d => d.O40MTId, opt => opt.MapFrom(src => src.O40MTId))
                .ForMember(d => d.Translations, opt => opt.MapFrom(src => src.Translations))
                .ReverseMap();

            CreateMap<ThesaurusEntryTranslationDataIn, ThesaurusEntryTranslation>()
                .ForMember(d => d.Abbreviations, opt => opt.MapFrom(src => src.Abbreviations))
                .ForMember(d => d.Definition, opt => opt.MapFrom(src => src.Definition))
                .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(d => d.PreferredTerm, opt => opt.MapFrom(src => src.PreferredTerm))
                .ForMember(d => d.SimilarTerms, opt => opt.MapFrom(src => src.SimilarTerms))
                .ForMember(d => d.Synonyms, opt => opt.MapFrom(src => src.Synonyms))
                .ReverseMap();

            CreateMap<ThesaurusEntryTree, ThesaurusEntryTreeDataOut>()
                .ReverseMap();

            CreateMap<ThesaurusEntryCodingSystem, ThesaurusEntryCodingSystemViewModel>()
                .ReverseMap();

            CreateMap<ThesaurusEntryCodingSystemDataIn, ThesaurusEntryCodingSystem>();

            CreateMap<AdministrativeData, AdministrativeDataViewModel>()
                .ReverseMap();

            CreateMap<ThesaurusEntryFilterDataIn, ThesaurusEntryFilterData>()
                .ReverseMap();

            CreateMap<O4CodeableConcept, O4CodeableConceptDataIn>().ReverseMap();
            CreateMap<O4CodeableConcept, O4CodeableConceptDataOut>()
                .ForMember(x => x.System, opt => opt.Ignore())
                .ReverseMap();

        }
    }
}