using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Common.Helpers;

namespace sReportsV2.DAL.Sql.Implementations
{
    public class UserDAL : IUserDAL
    {
        private SReportsContext context;
        public UserDAL(SReportsContext context)
        {
            this.context = context;
        }

        public IQueryable<UserView> GetAllByActiveOrganization(int activeOrganization)
        {
            IQueryable<UserView> queryable = context.UserViews;
            var z = queryable.ToList();

            if (activeOrganization > 0)
            {
                return queryable.Where(x => !x.IsDeleted && x.OrganizationId == activeOrganization && x.State != UserState.Archived);
            }
            else
            {
                return queryable.Where(x => !x.IsDeleted).GroupBy(x => x.UserId).Where(x => x.All(n => n.State == UserState.Archived || n.State == null)).Select(x => x.FirstOrDefault());
            }
        }

        public User GetById(int id)
        {
            var z = context.User.ToList();
            return context.User
                .Include(x => x.Address)
                .Include(x => x.Organizations)
                .Include("Organizations.Organization")
                .Include(x => x.ClinicalTrials)
                .Include(x => x.UserConfig)
                .Include(x => x.AcademicPositions)
                .Include(x => x.Roles)
                .Include("Roles.Role")
                .FirstOrDefault(x => x.UserId == id);
        }
        public User GetByEmail(string email)
        {
            return context.User.
                Include("Organizations")
                .Include("Organizations.Organization")
                .Include("Roles")
                .Include("Roles.Role")
                .Include("Roles.Role.Permissions")
                .Include("Roles.Role.Permissions.Module")
                .Include("Roles.Role.Permissions.Permission")
                .FirstOrDefault(x => x.Email.Equals(email) && !x.IsDeleted);
        }

        public User GetByUsername(string username)
        {
            return context.User
                .Include("Organizations")
                .Include("Organizations.Organization")
                .Include("Roles")
                .Include("Roles.Role")
                .FirstOrDefault(x => x.Username.Equals(username) && !x.IsDeleted);
        }

        public void InsertOrUpdate(User user)
        {
            if (user.UserId == 0)
            {
                context.User.Add(user);
            }
            context.SaveChanges();
        }

        public bool IsValidUser(string username, string password)
        {
            return context.User.Any(x => x.Username.Equals(username) && x.Password.Equals(password) && !x.IsDeleted);
        }

        public UserClinicalTrial GetClinicalTrial(int clinicalTrialId)
        {
            return context.UserClinicalTrial.FirstOrDefault(x => x.UserClinicalTrialId == clinicalTrialId);
        }

        public IQueryable<UserClinicalTrial> GetClinicalTrialsByUser(int userId)
        {
            return context.UserClinicalTrial.Where(x => x.UserId == userId);
        }

        public void SaveClinicalTrial(UserClinicalTrial clinicalTrial)
        {
            if (clinicalTrial.UserClinicalTrialId == 0)
            {
                context.UserClinicalTrial.Add(clinicalTrial);
            }

            context.SaveChanges();
        }

        public bool IsUserStillValid(int id)
        {
            User user = this.GetById(id);
            return user != null && user.Organizations.FirstOrDefault(x => x.OrganizationId == user.UserConfig.ActiveOrganizationId)?.State == UserState.Active;
        }

        public int CountAll()
        {
            return this.context.User.Count();
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public List<User> GetAllByIds(List<int> ids)
        {
            return context.User.Where(x => ids.Contains(x.UserId)).ToList();
        }

        public long GetAllCount()
        {
            return context.User.Where(x => !x.IsDeleted).Count();
        }

        public List<User> GetAllByOrganizationIds(List<int> organizationIds)
        {
            return context.User
                .Include(x => x.ClinicalTrials)
                .Include(x => x.Organizations)
                .Include(x => x.UserConfig)
                .Where(x => x.Organizations.Select(o => o.UserOrganizationId).Any(i => organizationIds.Contains(i))).ToList();
        }

        public bool IsUsernameValid(string username)
        {
            return context.User
                .FirstOrDefault(x => x.Username == username && !x.IsDeleted) == null;
        }

        public bool IsEmailValid(string email)
        {
            return context.User
                .FirstOrDefault(x => x.Email == email && !x.IsDeleted) == null;
        }

        public bool UserExist(int id)
        {
            return context.User.Any(x => x.UserId == id && !x.IsDeleted);
        }

        public void Delete(int id)
        {
            var fromDb = GetById(id);
            if (fromDb != null)
            {
                fromDb.IsDeleted = true;
                fromDb.SetLastUpdate();
                context.SaveChanges();
            }
        }

        public void SetState(int id, UserState state, int organizationId)
        {
            if (state == UserState.Archived)
            {
                IncrementOrganizationsUserCount(new List<int>() { organizationId }, -1);
            }
            User user = this.GetById(id);
            user.Organizations.FirstOrDefault(x => x.OrganizationId == organizationId).State = state;
            this.InsertOrUpdate(user);
        }
        public void UpdateOrganizationsUserCounts(User user, User dbUser)
        {
            IncrementOrganizationsUserCount(user.GetOrganizationRefs().Where(x => !dbUser.GetOrganizationRefs().Contains(x)).ToList(), 1);
        }

        private void IncrementOrganizationsUserCount(List<int> organizationIds, int value)
        {
            foreach (var organization in context.Organization.Where(x => organizationIds.Contains(x.OrganizationId)))
            {
                organization.NumOfUsers += value;
            }

            context.SaveChanges();
        }

        public void UpdateUsersCountForAllOrganization()
        {
            var organizationIds = GetOrganizationIds();
            Dictionary<int, int> numOfUsersPerOrganization = MapNumOfUsersPerOrganization(organizationIds);

            foreach (var organization in context.Organization.Where(x => !x.IsDeleted))
            {
                organization.NumOfUsers = numOfUsersPerOrganization[organization.OrganizationId];
            }

            context.SaveChanges();
        }

        private List<int> GetOrganizationIds()
        {
            return context.Organization.Where(x => !x.IsDeleted).Select(x => x.OrganizationId).ToList();
        }

        private Dictionary<int, int> MapNumOfUsersPerOrganization(List<int> organizationIds)
        {
            Dictionary<int, int> numOfUsersPerOrganization = new Dictionary<int, int>();
            foreach (var organizationId in organizationIds)
            {
                numOfUsersPerOrganization[organizationId] = GetAllByActiveOrganization(organizationId).Count();
            }
            return numOfUsersPerOrganization;
        }

        public void UpdatePassword(User user, string newPassword)
        {
            User userDb = GetById(user.UserId);
            userDb.Password = newPassword;
            userDb.SetLastUpdate();
            context.SaveChanges();
        }

        public List<User> GetAllBySearchWord(string searchWord)
        {
            return context.User
                .Include("Roles")
                .Include("Roles.Role")
                .Where(u => u.FirstName.Contains(searchWord))
                .ToList();
        }

        public List<UserClinicalTrial> GetlClinicalTrialsByName(string name)
        {
            return context.UserClinicalTrial.Where(clinicalTrial => !string.IsNullOrEmpty(clinicalTrial.Name) && clinicalTrial.Name.Contains(name) && clinicalTrial.IsArchived.HasValue && !clinicalTrial.IsArchived.Value).ToList();
        }
        public List<UserClinicalTrial> GetlClinicalTrialByIds(List<int> ids)
        {
            return context.UserClinicalTrial.Where(clinicalTrial => ids.Contains(clinicalTrial.UserClinicalTrialId)).ToList();
        }

        public List<AutoCompleteUserData> GetUsersFilteredByName(string searchValue)
        {
            return context.User
                .Where(x => x.FirstName.ToLower().Contains(searchValue.ToLower())
                        || x.LastName.ToLower().Contains(searchValue.ToLower()))
                .Select(x => new AutoCompleteUserData { UserId = x.UserId, FirstName = x.FirstName, LastName = x.LastName, UserName = x.Username })
                .ToList();
        }

        public List<UserView> GetAll(UserFilter userFilter, int activeOrganization)
        {
            IQueryable<UserView> result = GetAllByActiveOrganization(activeOrganization);

            if (userFilter.ColumnName != null)
            {
                result = SortTableHelper.OrderByField(result, userFilter.ColumnName, userFilter.IsAscending)
                     .Skip((userFilter.Page - 1) * userFilter.PageSize)
                     .Take(userFilter.PageSize);
            }
            else
            {
                result = result.OrderBy(x => x.UserId)
                     .Skip((userFilter.Page - 1) * userFilter.PageSize)
                     .Take(userFilter.PageSize);
            }

            return result.ToList();
        }

        public long GetAllFilteredCount(int activeOrganization)
        {
            return GetAllByActiveOrganization(activeOrganization).Count();
        }
    }
}
