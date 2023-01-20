using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.DTOs.Form.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;
using System.Web;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IPdfBLL
    {
        Byte[] Generate(Form form, UserCookieData userCookieData, Dictionary<string, string> translatedFields);
        Byte[] GenerateSynoptic (FormDataOut data, UserCookieData userCookieData, Dictionary<string, string> translatedFields, string signingUserCompleteName);    
        void Upload(HttpPostedFileBase file, UserCookieData userCookieData);
    }
}
