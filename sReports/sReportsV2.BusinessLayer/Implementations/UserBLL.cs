using AutoMapper;
using sReportsV2.BusinessLayer.Interfaces;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Helpers;
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
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using sReportsV2.Common.Enums;
using sReportsV2.DTOs;
using System.Net;
using sReportsV2.Common.Entities.User;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Helpers.EmailSender.Interface;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Exceptions;
using sReportsV2.DTOs.DTOs.User.DataIn;
using sReportsV2.DTOs.DTOs.User.DataOut;

namespace sReportsV2.BusinessLayer.Implementations
{
    public class UserBLL : IUserBLL
    {
        private readonly IUserDAL userDAL;
        private readonly HttpContextBase context;
        private readonly IOrganizationDAL organizationDAL;
        private readonly IFormDAL formDAL;
        private readonly IEmailSender emailSender;

        public UserBLL(IUserDAL userDAL, IOrganizationDAL organizationDAL, HttpContextBase context, IFormDAL formDAL, IEmailSender emailSender)
        {
            this.organizationDAL = organizationDAL;
            this.userDAL = userDAL;
            this.context = context;
            this.formDAL = formDAL;
            this.emailSender = emailSender;
        }

        public Tuple<UserDataOut, int?> TryLoginUser(UserLoginDataIn userDataIn)
        {
            UserDataOut result = null;
            UserCookieData userCookieData = null;
            User userEntity = IsValidUser(userDataIn);
            if (userEntity != null)
            {
                userCookieData = Mapper.Map<UserCookieData>(userEntity);
                userCookieData.TimeZoneOffset = userDataIn.TimeZone;
                userCookieData.ActiveLanguage = "en";
                userEntity.UserConfig.TimeZoneOffset = userDataIn.TimeZone;

                UpdateUserCookie(userCookieData);
                result = Mapper.Map<UserDataOut>(userCookieData);
                result.Organizations = Mapper.Map<List<UserOrganizationDataOut>>(userEntity.GetNonArchivedOrganizations());

                userDAL.Save();

            }

            return new Tuple<UserDataOut, int?>(result, userCookieData?.ActiveOrganization);
        }

        public User IsValidUser(UserLoginDataIn userDataIn)
        {
            User userEntity = this.userDAL.GetByUsername(userDataIn.Username);
            if (userEntity != null && userDAL.IsValidUser(userDataIn.Username, PasswordHelper.Hash(userDataIn.Password, userEntity.Salt)))
            {
                return userEntity;
            }

            return null;
        }

        public UserDataOut GetById(int userId)
        {
            return Mapper.Map<UserDataOut>(userDAL.GetById(userId));
        }

        public List<UserDataOut> GetByIdsList(List<int> ids)
        {
            throw new NotImplementedException();
        }

        public bool IsUsernameValid(string username)
        {
            return userDAL.IsUsernameValid(username);
        }

        public bool IsEmailValid(string email)
        {
            return userDAL.IsEmailValid(email);
        }

        public bool UserExist(int id)
        {
            return userDAL.UserExist(id);
        }

        public PaginationDataOut<UserViewDataOut, DataIn> ReloadTable(UserFilterDataIn dataIn, int activeOrganization)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));

            UserFilter filterData = Mapper.Map<UserFilter>(dataIn);
            PaginationDataOut<UserViewDataOut, DataIn> result = new PaginationDataOut<UserViewDataOut, DataIn>()
            {
                Count = (int)userDAL.GetAllFilteredCount(dataIn.ShowUnassignedUsers ? -1 : activeOrganization),
                Data = Mapper.Map<List<UserViewDataOut>>(userDAL.GetAll(filterData, dataIn.ShowUnassignedUsers ? -1 : activeOrganization)),
                DataIn = dataIn
            };

            return result;
        }

        public CreateResponseResult Insert(UserDataIn userDataIn, string activeLanguage)
        {
            userDataIn = Ensure.IsNotNull(userDataIn, nameof(userDataIn));
            User user = Mapper.Map<User>(userDataIn);
            User userDb = userDAL.GetById(userDataIn.Id) ?? new User();
            if (userDataIn.Id == 0)
            {
                userDb = user;
                userDb.Salt = PasswordHelper.CreateSalt(10);
                var tuplePass = PasswordHelper.CreateHashedPassword(8, userDb.Salt);
                userDb.Password = tuplePass.Item2;
                string mailContent = $"<div>" +
                    $"Dear {userDb.FirstName} {userDb.LastName},<br><br>you are granted access to the {EmailSenderNames.SoftwareName} clinical information system.<br>" +
                    $"Use the following link to access the system: <a href='{HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)}/User/Login?ReturnUrl=%2f'>{EmailSenderNames.SoftwareName} Login</a><br>" +
                    $"Your username is: <b>{userDb.Username}</b><br>" +
                    $"Your password is: <b>{tuplePass.Item1}</b><br><br>" +
                    $"After the first login, it's recommended to change the password.<br><br>" +
                    $"If you need any help or you have any additional questions please write on the <a href='mailto:smartoncology@wemedoo.com'>smartoncology@wemedoo.com</a>.<br><br>" +
                    $"------------------------------------------------------------------------------" +
                    $"</div>";
                Task.Run(() => emailSender.SendAsync($"{EmailSenderNames.SoftwareName} Registration", string.Empty, mailContent, userDb.Email));
            }
            else
            {
                userDb.Copy(user);
            }
            userDb.UpdateRoles(userDataIn.Roles);
            userDb.SetUserConfig(activeLanguage);
            userDb.ConfigureAcademicPositions(userDataIn.AcademicPositions);
            userDAL.InsertOrUpdate(userDb);

            return new CreateResponseResult()
            {
                Id = userDb.UserId,
                RowVersion = userDb.RowVersion
            };
        }

        public CreateResponseResult UpdateOrganizations(UserDataIn userDataIn)
        {
            User dbUser = userDAL.GetById(userDataIn.Id);
            if (dbUser == null)
            {
                //TO DO THROW EXCEPTION NOT FOUND
            }
            userDAL.UpdateOrganizationsUserCounts(Mapper.Map<User>(userDataIn), dbUser);
            if (userDataIn.UserOrganizations != null) 
            {
                foreach (UserOrganizationDataIn userOrganizationDataIn in userDataIn.UserOrganizations)
                {
                    UserOrganization userOrganizationDb = dbUser.Organizations.FirstOrDefault(x => x.OrganizationId == userOrganizationDataIn.OrganizationId);
                    UserOrganization userOrganization = Mapper.Map<UserOrganization>(userOrganizationDataIn);
                    if (userOrganizationDb != null)
                    {
                        userOrganizationDb.Copy(userOrganization);
                    }
                    else
                    {
                        dbUser.Organizations.Add(userOrganization);
                        SetUserActiveOrganizationIfNull(dbUser, userOrganizationDataIn.OrganizationId);
                    }
                }
                SetPredefinedFormsForNewUser(dbUser);
            }

            userDAL.InsertOrUpdate(dbUser);
            return new CreateResponseResult()
            {
                Id = dbUser.UserId,
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
                throw new NullReferenceException();
            }

            UserDataOut userData = Mapper.Map<UserDataOut>(dbUser);
            return userData;
        }

        public UserOrganizationDataOut LinkOrganization(LinkOrganizationDataIn dataIn)
        {
            dataIn = Ensure.IsNotNull(dataIn, nameof(dataIn));
            OrganizationDataOut organization = Mapper.Map<OrganizationDataOut>(organizationDAL.GetById(dataIn.OrganizationId));

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
                    var clinicalTrial = dbUser.ClinicalTrials.FirstOrDefault(x => x.UserClinicalTrialId == clinicalTrialDTO.Id);
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
                Id = dbUser.UserId,
                RowVersion = dbUser.RowVersion
            };
        }

        byte[] IUserBLL.ArchiveClinicalTrial(ArchiveTrialDataIn dataIn)
        {
            User user = userDAL.GetById(dataIn.UserId);
            user.ClinicalTrials[user.ClinicalTrials.FindIndex(c => c.UserClinicalTrialId == dataIn.ClinicalTrialId)].IsArchived = true;
            userDAL.InsertOrUpdate(user);

            return user.RowVersion;
        }

        public void SetState(int id, UserState? state, int organizationId)
        {
            if (state.HasValue)
            {
                userDAL.SetState(id, state.Value, organizationId);
            }
            else
            {
                userDAL.Delete(id);
            }
        }

        public void SetActiveOrganization(UserCookieData userCookieData, int organizationId)
        {
            User user = this.userDAL.GetById(userCookieData.Id);
            user.UserConfig.ActiveOrganizationId = organizationId;
            this.userDAL.InsertOrUpdate(user);
            userCookieData.ActiveOrganization = organizationId;
            HttpContext.Current.Session["userData"] = userCookieData;
        }

        public void UpdatePageSize(int pageSize, UserCookieData userCookieData)
        {
            User user = userDAL.GetByUsername(userCookieData.Username);
            if (user != null)
            {
                user.UserConfig.PageSize = pageSize;
            }
            userCookieData.PageSize = pageSize;
            userDAL.InsertOrUpdate(user);
            UpdateUserCookie(userCookieData);
        }

        public void UpdateLanguage(EnumDTO data, UserCookieData userCookieData)
        {
            User user = userDAL.GetByUsername(userCookieData.Username);
            if(user != null)
            {
                user.UserConfig.ActiveLanguage = data.Value;
            }
            userDAL.InsertOrUpdate(user);
            userCookieData.ActiveLanguage = data.Value;
            HttpContext.Current.Session["userData"] = userCookieData;
            UpdateUserCookie(userCookieData);
        }

        public void GeneratePassword(string email)
        {
            email = Ensure.IsNotNull(email, nameof(email));
            User user = userDAL.GetByEmail(email);
            var tuplePass = PasswordHelper.CreateHashedPassword(8, user.Salt);
            user.Password = tuplePass.Item2;
            string mailContent = $"Dear {user.FirstName} {user.LastName},<br><br>your password has been changed.<br>" +
                $"Your new password is: <b>{tuplePass.Item1}</b><br><br><br>" +
                $"If you need help or you have any additional questions please write on the <a href='mailto:smartoncology@wemedoo.com'>smartoncology@wemedoo.com</a>.<br><br><br>" +
                $"------------------------------------------------------------------------------";
            Task.Run(() => emailSender.SendAsync($"{EmailSenderNames.SoftwareName} password", string.Empty, mailContent, user.Email));
            userDAL.InsertOrUpdate(user);
        }

        public void ChangePassword(string oldPassword, string newPassword, string confirmPassword, string userId)
        {
            User user = ValidateChangePasswordInput(oldPassword, newPassword, confirmPassword, userId);
            try
            {
                userDAL.UpdatePassword(user, PasswordHelper.Hash(newPassword, user.Salt));
            }
            catch (Exception)
            {
                string message = "Error while updating user password";
                throw new UserAdministrationException(HttpStatusCode.Conflict, message);
            }
        }

        public List<UserData> ProposeUserBySearchWord(string searchWord)
        {
            List<User> proposedUsers = userDAL.GetAllBySearchWord(searchWord);
            List<User> proposedUsersFiltered = new List<User>();
            
            foreach (User user in proposedUsers)
            {
                if (user.UserHasPermission(PermissionNames.ViewComments, ModuleNames.Designer) && user.UserHasPermission(PermissionNames.View, ModuleNames.Designer))
                {
                    proposedUsersFiltered.Add(user);
                }
            };
            return Mapper.Map<List<UserData>>(proposedUsersFiltered);
        }

        public void AddSuggestedForm(string username, string formId)
        {
            User user = userDAL.GetByUsername(username);
            user.AddSuggestedForm(formId);
            userDAL.InsertOrUpdate(user);
        }

        public void RemoveSuggestedForm(string username, string formId)
        {
            User user = userDAL.GetByUsername(username);
            user.RemoveSuggestedForm(formId);
            userDAL.InsertOrUpdate(user);
        }

        public List<ClinicalTrialDTO> GetlClinicalTrialsByName(string name)
        {
            return Mapper.Map<List<ClinicalTrialDTO>>(userDAL.GetlClinicalTrialsByName(name));
        }

        private User ValidateChangePasswordInput(string oldPassword, string newPassword, string confirmPassword, string userId)
        {
            if (!Int32.TryParse(userId, out int userIdentifier))
            {
                throw new UserAdministrationException(HttpStatusCode.BadRequest, "User id is in invalid format.");
            }

            User user = userDAL.GetById(userIdentifier);
            if (user == null)
            {
                throw new UserAdministrationException(HttpStatusCode.NotFound, string.Format("User with given id ({0}) does not exist.", userId));
            }

            if (!PasswordHelper.Hash(oldPassword, user.Salt).Equals(user.Password))
            {
                throw new UserAdministrationException(HttpStatusCode.BadRequest, "Current password is not correct.");
            }

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                throw new UserAdministrationException(HttpStatusCode.Conflict, "Invalid input: Password should not be empty.");
            }

            if (!newPassword.Equals(confirmPassword))
            {
                throw new UserAdministrationException(HttpStatusCode.Conflict, "New and confirm passwords do not match.");
            }

            if (newPassword.Equals(oldPassword))
            {
                throw new UserAdministrationException(HttpStatusCode.Conflict, "Old and new password match. Please provide new value.");
            }

            string errorMessage = PasswordHelper.AdditionalPasswordChecking(newPassword);
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new UserAdministrationException(HttpStatusCode.Conflict, errorMessage);
            }

            return user;
        }

        private void SetUserActiveOrganizationIfNull(User user, int activeOrganizationId)
        {
            if (user.UserConfig.ActiveOrganizationId == null)
            {
                user.UserConfig.ActiveOrganizationId = activeOrganizationId;
            }
        }

        private void SetPredefinedFormsForNewUser(User user)
        {
            List<string> formRefs = InitializeFormRefs(user);
            if (formRefs.Count > 5)
            {
                Random rnd = new Random();
                for (int i = 1; i <= 5; i++)
                {
                    int index = rnd.Next(formRefs.Count);
                    user.AddSuggestedForm(formRefs[index]);
                }
            }
            else
            {
                user.UserConfig.PredefinedForms = formRefs;
            }
        }

        private List<string> InitializeFormRefs(User user)
        {
            return formDAL.GetByClinicalDomains(organizationDAL.GetClinicalDomainsForIds(user.GetOrganizationRefs()));
        }

        private void UpdateUserCookie(UserCookieData userCookieData)
        {
            userCookieData = Ensure.IsNotNull(userCookieData, nameof(userCookieData));

            if (userCookieData.ActiveLanguage != null)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(userCookieData.ActiveLanguage);
            }

            HttpCookie cookie = new HttpCookie("Language")
            {
                Value = userCookieData.ActiveLanguage
            };
            context.Response.Cookies.Add(cookie);
        }

        public List<UserDataOut> GetUsersByName(string searchValue)
        {
            List<AutoCompleteUserData> users = userDAL.GetUsersFilteredByName(searchValue);
            List<UserDataOut> result = new List<UserDataOut>();

            foreach (AutoCompleteUserData user in users)
            {
                result.Add(new UserDataOut { Id = user.UserId, FirstName = user.FirstName, LastName = user.LastName, Username = user.UserName});
            }

            return result;
        }

    }
}
