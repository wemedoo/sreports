using sReportsV2.Common.Enums;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
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
        UserDataOut TryLoginUser(UserLoginDataIn user);
        UserDataOut GetUserForEdit(int userId);
        void GeneratePassword(string email);
        PaginationDataOut<UserDataOut, DataIn> ReloadTable(DataIn dataIn, int activeOrganization);
        CreateResponseResult Insert(UserDataIn userDataIn, string activeLanguage);
        CreateResponseResult UpdateOrganizations(UserDataIn userDataIn);
        CreateResponseResult UpdateClinicalTrials(UserDataIn userDataIn);
        UserOrganizationDataOut LinkOrganization(LinkOrganizationDataIn dataIn);
        ClinicalTrialDTO ResetClinicalTrial(int? clinicalTrialId);
        List<ClinicalTrialDTO> SubmitClinicalTrial(ClinicalTrialWithUserInfoDataIn dataIn);
        UserDataOut GetById(int userId);
        List<UserDataOut> GetByIdsList(List<int> ids);
        long GetAllCount();
        bool IsUsernameValid(string username);
        bool IsEmailValid(string email);
        byte[] ArchiveClinicalTrial(ArchiveTrialDataIn dataIn);
        bool UserExist(int id);
        void SetState(int id, UserState state, int organizationId);



    }
}
