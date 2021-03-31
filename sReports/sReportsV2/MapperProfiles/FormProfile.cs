using AutoMapper;
using MongoDB.Bson;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Distribution;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Form;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.Models.Common;
using sReportsV2.Models.Form;
using sReportsV2.Models.Form.Tree;
using System.Linq;

namespace sReportsV2.MapperProfiles
{
    public class FormProfile : Profile
    {
        public FormProfile()
        {
            CreateMap<EnumData, EnumViewModel>().ReverseMap();

            CreateMap<FormFilterDataIn, FormFilterData>().ReverseMap();

            CreateMap<Form, FormTreeViewModel>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.Chapters, opt => opt.MapFrom(src => src.Chapters))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
               .ReverseMap();

            CreateMap<FormChapter, FormTreeChapterViewModel>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.Pages, opt => opt.MapFrom(src => src.Pages))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
               .ReverseMap();

            CreateMap<FormPage, FormTreePageViewModel>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.FieldSets, opt => opt.MapFrom(src => src.ListOfFieldSets.SelectMany(x => x)))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
               .ReverseMap();

            CreateMap<FieldSet, FormTreeFieldSetViewModel>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.Fields, opt => opt.MapFrom(src => src.Fields))
               .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ReverseMap();

            CreateMap<Field, FormTreeFieldViewModel>()
           .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
           .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
           .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
           .ReverseMap();

            CreateMap<FieldSelectable, FormTreeFieldViewModel>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.Values, opt => opt.MapFrom(src => src.Values))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
               .ReverseMap();

            CreateMap<FieldString, FormTreeFieldViewModel>()
               .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
               .ReverseMap();

            CreateMap<FormFieldValue, FormTreeFieldValueViewModel>()
               .ForMember(d => d.Value, opt => opt.MapFrom(src => src.Value))
               .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
               .ForMember(d => d.Label, opt => opt.MapFrom(src => src.Label))
               .ReverseMap();

            CreateMap<Form, FormEpisodeOfCareDataOut>()
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
                .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id));

            /*DATA OUT*/
            CreateMap<EnumData, EnumDataOut>();

            CreateMap<Form, FormDataOut>()
            .ForMember(d => d.About, opt => opt.MapFrom(src => src.About))
            .ForMember(d => d.Chapters, opt => opt.MapFrom(src => src.Chapters))
            .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(d => d.State, opt => opt.MapFrom(src => src.State))
            .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
            .ForMember(d => d.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(d => d.FormState, opt => opt.MapFrom(src => src.FormState))
            .ForMember(d => d.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
            .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
            .ForMember(d => d.DocumentProperties, opt => opt.MapFrom(src => src.DocumentProperties))
            .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
            .ForMember(d => d.WorkflowHistory, opt => opt.MapFrom(src => src.WorkflowHistory))
            .ReverseMap();

            CreateMap<FormAbout, FormAboutDataOut>();
            CreateMap<FormChapter, FormChapterDataOut>();
            CreateMap<FormPage, FormPageDataOut>();
            CreateMap<FieldSet, FormFieldSetDataOut>();


            CreateMap<FormFieldDependable, FormFieldDependableDataOut>().ReverseMap();
            CreateMap<FormFieldValue, FormFieldValueDataOut>().ReverseMap();
            CreateMap<LayoutStyle, FormLayoutStyleDataOut>().ReverseMap();
            CreateMap<Help, FormHelpDataOut>();
            CreateMap<FormEpisodeOfCare, FormEpisodeOfCareDataDataOut>();

            /*DATA IN*/
            CreateMap<FormDataIn, Form>()
            .ForMember(d => d.About, opt => opt.MapFrom(src => src.About))
            .ForMember(d => d.Chapters, opt => opt.MapFrom(src => src.Chapters))
            .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.State, opt => opt.MapFrom(src => src.State))
            .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(d => d.Language, opt => opt.MapFrom(src => src.Language))
            .ForMember(d => d.Version, opt => opt.MapFrom(src => src.Version))
            .ForMember(d => d.ThesaurusId, opt => opt.MapFrom(src => src.ThesaurusId))
            .ForMember(d => d.EntryDatetime, opt => opt.MapFrom(src => src.EntryDatetime))
            .ForMember(d => d.LastUpdate, opt => opt.MapFrom(src => src.LastUpdate))
            .ForMember(d => d.DocumentProperties, opt => opt.MapFrom(src => src.DocumentProperties))
            .ReverseMap();

            CreateMap<DTOs.Form.DataIn.FormEpisodeOfCareDataDataIn, FormEpisodeOfCare>();

            CreateMap<FormAboutDataIn, FormAbout>();

            CreateMap<FormPageImageMap, FormPageImageMapDataOut>();

            CreateMap<FormPageImageMapDataIn, FormPageImageMap>();

            CreateMap<FormChapterDataIn, FormChapter>();

            CreateMap<FormPageDataIn, FormPage>();

            CreateMap<FormFieldSetDataIn, FieldSet>();


            //CreateMap<FormFieldDataIn, FormField>();

            CreateMap<FormFieldDependableDataIn, FormFieldDependable>();

            CreateMap<FormFieldValueDataIn, FormFieldValue>();

            CreateMap<FormLayoutStyleDataIn, LayoutStyle>();

            CreateMap<FormHelpDataIn, Help>();

            CreateMap<KeyValue, KeyValueDTO>().ReverseMap();
            CreateMap<ReferalInfo, ReferralInfoDTO>().ReverseMap();

            CreateMap<FormStatus, FormStatusDataOut>()
                .ForMember(d => d.Created, opt => opt.MapFrom(src => src.Created))
                .ForMember(d => d.Status, opt => opt.MapFrom(src => src.Status))
                .ForAllOtherMembers(d => d.Ignore());
        }
    }
}