using AutoMapper;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DTOs.Administration;
using sReportsV2.DTOs.CodeSystem;
using sReportsV2.DTOs.DTOs.CodeSystem;
using sReportsV2.DTOs.DTOs.GlobalThesaurus.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataIn;
using sReportsV2.DTOs.O4CodeableConcept.DataOut;
using sReportsV2.DTOs.ThesaurusEntry;
using sReportsV2.DTOs.ThesaurusEntry.DataIn;
using sReportsV2.DTOs.ThesaurusEntry.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.MapperProfiles
{
    public class ThesaurusEntrySqlProfile : Profile
    {
        public ThesaurusEntrySqlProfile() 
        {
            CreateMap<ThesaurusEntryTranslationDataOut, ThesaurusEntryTranslation>()
               .ReverseMap();

            CreateMap<ThesaurusFilterDataIn, ThesaurusFilter>()
                   .ReverseMap();

            CreateMap<O4CodeableConcept, O4CodeableConceptDataOut>()
               .ReverseMap();
            CreateMap<O4CodeableConceptDataIn, O4CodeableConcept>()
               .ReverseMap();

            CreateMap<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry, ThesaurusEntryDataIn>()
                 .ForMember(x => x.UmlsCode, opt => opt.Ignore())
                 .ForMember(x => x.UmlsDefinitions, opt => opt.Ignore())
                 .ForMember(x => x.UmlsName, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<AdministrativeDataDataOut, AdministrativeData>()
                 
                .ReverseMap();

            CreateMap<Domain.Sql.Entities.ThesaurusEntry.Version, VersionDataOut>()
                .ReverseMap();

            CreateMap<SimilarTermDataOut, SimilarTerm>()
               .ReverseMap();

            CreateMap<ThesaurusEntryTranslation, ThesaurusEntryTranslationDataIn>().ReverseMap();
            CreateMap<ThesaurusFilterDataIn, ThesaurusFilter>()
                   .ReverseMap();

            CreateMap<GlobalThesaurusFilter, GlobalThesaurusFilterDataIn>()
               .ReverseMap();

            CreateMap<ThesaurusEntryFilterDataIn, ThesaurusEntryFilterData>()
                .ReverseMap();

            CreateMap<ThesaurusReviewFilterDataIn, ThesaurusReviewFilterData>()
                .ReverseMap();

            CreateMap<CodeSystem, CodeSystemDataOut>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
               .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
               .ForMember(d => d.Value, opt => opt.MapFrom(src => src.Value));


            CreateMap<AdministrationFilterDataIn, ThesaurusEntryFilterData>()
               .ForMember(d => d.PreferredTerm, opt => opt.MapFrom(src => src.PreferredTerm))
               .ForMember(d => d.Page, opt => opt.MapFrom(src => src.Page))
               .ForMember(d => d.PageSize, opt => opt.MapFrom(src => src.PageSize))
               .ReverseMap();

            CreateMap<CodeSystem, CodeSystemDataIn>()
               .ReverseMap();
        }
        
    }
}