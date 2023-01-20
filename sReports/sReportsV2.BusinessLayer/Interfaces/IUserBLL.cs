using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.DTOs.User.DataIn;
using sReportsV2.DTOs.DTOs.User.DataOut;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using System;
using System.Collections.Generic;

namespace sReportsV2.BusinessLayer.Interfaces
{
    public interface IUserBLL
    {
        Tuple<UserDataOut, int?> TryLoginUser(UserLoginDataIn user);
        User IsValidUser(UserLoginDataIn user);
        UserDataOut GetUserForEdit(int userId);
        void GeneratePassword(string email);
        PaginationDataOut<UserViewDataOut, DataIn> ReloadTable(UserFilterDataIn dataIn, int activeOrganization);
        CreateResponseResult Insert(UserDataIn userDataIn, string activeLanguage);
        CreateResponseResult UpdateOrganizations(UserDataIn userDataIn);
        CreateResponseResult UpdateClinicalTrials(UserDataIn userDataIn);
        void UpdateLanguage(EnumDTO data, UserCookieData userCookieData);
        UserOrganizationDataOut LinkOrganization(LinkOrganizationDataIn dataIn);
        ClinicalTrialDTO ResetClinicalTrial(int? clinicalTrialId);
        List<ClinicalTrialDTO> SubmitClinicalTrial(ClinicalTrialWithUserInfoDataIn dataIn);
        List<ClinicalTrialDTO> GetlClinicalTrialsByName(string name);
        UserDataOut GetById(int userId);
        List<UserDataOut> GetByIdsList(List<int> ids);
        bool IsUsernameValid(string username);
        bool IsEmailValid(string email);
        byte[] ArchiveClinicalTrial(ArchiveTrialDataIn dataIn);
        bool UserExist(int id);
        void SetState(int id, UserState? state, int organizationId);
        void SetActiveOrganization(UserCookieData userCookieData, int organizationId);
        void UpdatePageSize(int pageSize, UserCookieData userCookieData);
        void ChangePassword(string oldPassword, string newPassword, string confirmPassword, string userId);
        List<UserData> ProposeUserBySearchWord(string searchWord);
        void AddSuggestedForm(string username, string formId);
        void RemoveSuggestedForm(string username, string formId);
        List<UserDataOut> GetUsersByName(string searchValue);

    }
}
