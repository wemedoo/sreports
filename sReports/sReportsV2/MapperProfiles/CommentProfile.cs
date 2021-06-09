using AutoMapper;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.DTOs.Form.DataIn;
using sReportsV2.DTOs.Form.DataOut;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.MapperProfiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
           CreateMap<Comment, FormCommentDataOut>()
          .ReverseMap();

           CreateMap<FormCommentDataIn, Comment>()
          .ReverseMap();

            CreateMap<sReportsV2.Domain.Sql.Entities.FormComment.Comment, FormCommentDataOut>()
          .ReverseMap();

            CreateMap<FormCommentDataIn, sReportsV2.Domain.Sql.Entities.FormComment.Comment>()
           .ReverseMap();
        }
    }
}