using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DTOs.Common;
using sReportsV2.DTOs.Common.DataOut;
using sReportsV2.DTOs.Common.DTO;
using sReportsV2.DTOs.Organization;
using sReportsV2.DTOs.Pagination;
using sReportsV2.DTOs.User.DataIn;
using sReportsV2.DTOs.User.DataOut;
using sReportsV2.DTOs.User.DTO;
using sReportsV2.DAL.Sql.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Security.Claims;
using sReportsV2.Common.Enums;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class UserBLL : IUserBLL
    {
        private readonly IUserDAL userDAL;
        private readonly HttpContextBase context;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IFormDAL formDAL;
        public UserBLL(IUserDAL userDAL, IOrganizationDAL organizationDAL, HttpContextBase context, IFormDAL formDAL)
        {
            this.organizationDAL = organizationDAL;
            this.userDAL = userDAL;
            this.context = context;
            this.formDAL = formDAL;
        }

        public UserDataOut TryLoginUser(UserLoginDataIn userDataIn)
        {
            UserDataOut result = null;
            if (userDAL.IsValidUser(userDataIn.Username, userDataIn.Password))
            {
                User userEntity = this.userDAL.GetByUsername(userDataIn.Username);
                UserCookieData userCookieData = Mapper.Map<UserCookieData>(userEntity);
                userCookieData.Organizations = Mapper.Map<List<OrganizationDataOut>>(userEntity.Organizations.Select(x => x.Organization));
                userCookieData.TimeZoneOffset = userDataIn.TimeZone;
                userCookieData.ActiveLanguage = "en";
                userEntity.UserConfig.TimeZoneOffset = userDataIn.TimeZone;

                var rolesByOrganization = userEntity.GetRolesByActiveOrganization(userCookieData.ActiveOrganization);

                ChangeLanguage(userCookieData);


                result = Mapper.Map<UserDataOut>(userCookieData);
                result.Organizations = Mapper.Map<List<UserOrganizationDataOut>>(userEntity.Organizations);

                userDAL.Save();
                
            }

            return result;
        }

        public void ChangeLanguage(UserCookieData userCookieData)
        {
            userCookieData = Ensure.IsNotNull(userCookieData, nameof(userCookieData));

            if (userCookieData.ActiveLanguage != null)
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(userCookieData.ActiveLanguage);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(userCookieData.ActiveLanguage);
            }

            HttpCookie cookie = new HttpCookie("Language");
            cookie.Value = userCookieData.ActiveLanguage;
            context.Response.Cookies.Add(cookie);
        }

        public void GeneratePassword(string email)
        {
            email = Ensure.IsNotNull(email, nameof(email));
            User user = userDAL.GetByEmail(email);
            user.Password = CreatePassword(8);
            Task.Run(() => EmailSender.SendAsync("sReports password", "Your password is: " + user.Password + ". Please change your password on your first login. ", "", user.Email));
            userDAL.InsertOrUpdate(user);
        }

        public PaginationDataOut<UserDataOut, DataIn> ReloadTable(DataIn dataIn, int activeOrganization)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            IQueryable<User> users = userDAL.GetAllByActiveOrganization(activeOrganization);
            PaginationDataOut<UserDataOut, DataIn> result = new PaginationDataOut<UserDataOut, DataIn>()
            {
                Count = users.Count(),
                Data = Mapper.Map<List<UserDataOut>>(users.OrderBy(x => x.Id).Skip((dataIn.Page - 1) * dataIn.PageSize).Take(dataIn.PageSize).ToList()),
                DataIn = dataIn
            };

            return result;
        }

        public CreateResponseResult Insert(UserDataIn userDataIn, string activeLanguage)
        {
            userDataIn = Ensure.IsNotNull(userDataIn, nameof(userDataIn));
            User dbUser = Mapper.Map<User>(userDataIn);
            dbUser.UserConfig = new UserConfig();
            dbUser.UserConfig.ActiveLanguage = activeLanguage;
            SetSuggestedFormsForNewUser(dbUser, InitializeFormRefs(dbUser));

            if (userDataIn.Id == 0) 
            {
                dbUser.Password = CreatePassword(8);
                Task.Run(() => EmailSender.SendAsync("sReports password", "Your password is: " + dbUser.Password + ". Please change your password on your first login. ", "", dbUser.Email));

            }
            userDAL.InsertOrUpdate(dbUser);

            return new CreateResponseResult()
            {
                Id = dbUser.Id,
                RowVersion = dbUser.RowVersion
            };
        }


       
        private void SetSuggestedFormsForNewUser(User user, List<string> formRefs)
        {
            Random rnd = new Random();

            if (formRefs.Count > 5)
                for (int i = 1; i <= 5; i++)
                {
                    int index = rnd.Next(formRefs.Count);
                    user.AddSuggestedForm(formRefs[index]);
                }
            else
                user.SuggestedForms = formRefs;
        }

        private List<string> InitializeFormRefs(User user)
        {           
            return formDAL.GetByClinicalDomains(organizationDAL.GetClinicalDomainsForIds(user.Organizations.Select(x => x.OrganizationId).ToList()).ToList());
        }

        private string CreatePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        public CreateResponseResult UpdateOrganizations(UserDataIn userDataIn)
        {
            User dbUser = userDAL.GetById(userDataIn.Id);
            userDAL.UpdateOrganizationsUserCounts(Mapper.Map<User>(userDataIn), dbUser);
            if (dbUser == null)
            {
                //TO DO THROW EXCEPTION NOT FOUND
            }
            if (userDataIn.UserOrganizations != null) 
            {
                foreach (UserOrganizationDataIn userOrganizationDataIn in userDataIn.UserOrganizations)
                {
                    UserOrganization userOrganization = dbUser.Organizations.FirstOrDefault(x => x.OrganizationId == userOrganizationDataIn.OrganizationId);
                    if (userOrganization != null)
                    {
                        userOrganization.DepartmentName = userOrganizationDataIn.DepartmentName;
                        userOrganization.IsPracticioner = userOrganizationDataIn.IsPracticioner;
                        userOrganization.LegalForm = userOrganizationDataIn.LegalForm;
                        userOrganization.OrganizationalForm = userOrganizationDataIn.OrganizationalForm;
                        userOrganization.Qualification = userOrganizationDataIn.Qualification;
                        //userOrganization.Roles = userOrganizationDataIn.Roles;
                        userOrganization.SeniorityLevel = userOrganizationDataIn.SeniorityLevel;
                        userOrganization.Speciality = userOrganizationDataIn.Speciality;
                        userOrganization.State = userOrganizationDataIn.State;
                        userOrganization.SubSpeciality = userOrganization.SubSpeciality;
                        userOrganization.Team = userOrganization.Team;
                    }
                    else
                    {
                        dbUser.Organizations.Add(Mapper.Map<UserOrganization>(userOrganizationDataIn));
                        SetUserActiveOrganizationIfNull(dbUser, userOrganizationDataIn.OrganizationId);
                    }
                }
            }
            
            userDAL.InsertOrUpdate(dbUser);
            return new CreateResponseResult()
            {
                Id = dbUser.Id,
                RowVersion = dbUser.RowVersion
            };
        }

        private void SetUserActiveOrganizationIfNull(User user, int activeOrganizationId) 
        {
            if (user.UserConfig.ActiveOrganizationId == null) 
            {
                user.UserConfig.ActiveOrganizationId = activeOrganizationId;
            }
        }
        public CreateResponseResult UpdateClinicalTrials(UserDataIn userDataIn)
        {
            User dbUser = userDAL.GetById(userDataIn.Id);
            if (dbUser == null)
            {
                // TO DO THROW EXCEPTIONS
            }

            if (userDataIn.ClinicalTrials != null) 
            {
                foreach (ClinicalTrialDTO clinicalTrialDTO in userDataIn.ClinicalTrials)
                {
                    var clinicalTrial = dbUser.ClinicalTrials.FirstOrDefault(x => x.Id == clinicalTrialDTO.Id);
                    if (clinicalTrial != null)
                    {
                        SetClinicalTrialData(clinicalTrial, clinicalTrialDTO);
                    }
                    else
                    {
                        dbUser.ClinicalTrials.Add(Mapper.Map<UserClinicalTrial>(clinicalTrialDTO));
                    }
                }
            }

            userDAL.InsertOrUpdate(dbUser);

            return new CreateResponseResult()
            {
                Id = dbUser.Id,
                RowVersion = dbUser.RowVersion
            };
        }

        public void SetClinicalTrialData(UserClinicalTrial dbTrial, ClinicalTrialDTO trial) 
        {
            dbTrial.Name = trial.Name;
            dbTrial.Acronym = trial.Acronym;
            dbTrial.SponosorId = trial.SponosorId;
            dbTrial.WemedooId = trial.WemedooId;
            dbTrial.Status = trial.Status;
            dbTrial.Role = trial.Role;
            dbTrial.IsArchived = trial.IsArchived;
        }

        public UserDataOut GetUserForEdit(int userId)
        {
            User dbUser = userDAL.GetById(userId);

            if(dbUser == null)
            {
                //TO DO THROW EXCEPTION
            }

            UserDataOut userData = Mapper.Map<UserDataOut>(dbUser);
            return userData;
        }

        public UserOrganizationDataOut LinkOrganization(LinkOrganizationDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            OrganizationDataOut organization = Mapper.Map<OrganizationDataOut>(organizationDAL.GetByName(dataIn.OrganizationName));

            return new UserOrganizationDataOut(organization);
        }

        public ClinicalTrialDTO ResetClinicalTrial(int? clinicalTrialId)
        {
            UserClinicalTrial clinicalTrial;
            if (clinicalTrialId == 0)
            {
                clinicalTrial = new UserClinicalTrial();
            }
            else
            {
                clinicalTrial = userDAL.GetClinicalTrial(clinicalTrialId.GetValueOrDefault());
            }

            return Mapper.Map<ClinicalTrialDTO>(clinicalTrial);
        }

        public List<ClinicalTrialDTO> SubmitClinicalTrial(ClinicalTrialWithUserInfoDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            UserClinicalTrial userClinicalTrial = userDAL.GetClinicalTrial(dataIn.ClinicalTrial.Id);
            if (userClinicalTrial == null)
            {
                userClinicalTrial = Mapper.Map<UserClinicalTrial>(dataIn.ClinicalTrial);
            }
            else 
            {
                SetClinicalTrialData(userClinicalTrial, dataIn.ClinicalTrial);
            }
            userDAL.SaveClinicalTrial(userClinicalTrial);
            var trials = userDAL.GetClinicalTrialsByUser(dataIn.UserId.GetValueOrDefault()).ToList();
            return Mapper.Map<List<ClinicalTrialDTO>>(trials);
        }

        public UserDataOut GetById(int userId)
        {
            return Mapper.Map<UserDataOut>(userDAL.GetById(userId));
        }

        public List<UserDataOut> GetByIdsList(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public long GetAllCount()
        {
            return userDAL.GetAllCount();
        }

        public bool IsUsernameValid(string username)
        {
            return userDAL.IsUsernameValid(username);
        }

        public bool IsEmailValid(string email)
        {
            return userDAL.IsEmailValid(email);
        }


        byte[] IUserBLL.ArchiveClinicalTrial(ArchiveTrialDataIn dataIn)
        {
            User user = userDAL.GetById(dataIn.UserId);
            user.ClinicalTrials[user.ClinicalTrials.FindIndex(c => c.Id == dataIn.ClinicalTrialId)].IsArchived = true;
            userDAL.InsertOrUpdate(user);

            return user.RowVersion;
        }
        public bool UserExist(int id)
        {
            return userDAL.UserExist(id);
        }

        public void SetState(int id, UserState state, int organizationId)
        {
            userDAL.SetState(id, state, organizationId);
        }
    }
}
