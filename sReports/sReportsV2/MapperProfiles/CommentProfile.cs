using AutoMapper;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;

namespace sReportsV2.MapperProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<Comment, FormCommentDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<FormCommentDataIn, Comment>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();

            CreateMap<Domain.Sql.Entities.FormComment.Comment, FormCommentDataOut>()
                .ForMember(d => d.Id, opt => opt.MapFrom(src => src.CommentId))
                .ReverseMap();

            CreateMap<FormCommentDataIn, Domain.Sql.Entities.FormComment.Comment>()
                .ForMember(d => d.CommentId, opt => opt.MapFrom(src => src.Id))
                .ReverseMap();
        }
    }
}