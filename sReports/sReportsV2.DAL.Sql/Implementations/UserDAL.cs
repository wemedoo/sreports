using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.DAL.Sql.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using sReportsV2.Common.Enums;

namespace sReportsV2.DAL.Sql.Implementations
{
    public class UserDAL : IUserDAL
    {
        private SReportsContext context;
        public UserDAL(SReportsContext context)
        {
            this.context = context;
        }

        public IQueryable<User> GetAllByActiveOrganization(int activeOrganization)
        {
            return context.User.Include(x => x.Organizations).Where(x => !x.IsDeleted && x.Organizations.Any(y => y.OrganizationId == activeOrganization && y.State != Common.Enums.UserState.Archived));
        }

        public User GetById(int id)
        {
            return context.User
                .Include(x => x.Address)
                .Include(x => x.Organizations)
                .Include("Organizations.Organization")
                .Include(x => x.ClinicalTrials)
                .Include(x => x.UserConfig)
                .FirstOrDefault(x => x.Id == id);
        }
        public User GetByEmail(string email)
        {
            return context.User.Include("Organizations")
                .Include("Organizations.Organization")
                .FirstOrDefault(x => x.Email.Equals(email));
        }

        public User GetByUsername(string username)
        {
            return context.User
                .Include("Organizations")
                .Include("Organizations.Organization")
                .Include("ActiveOrganization")
                .FirstOrDefault(x => x.Username.Equals(username));
        }

        public void InsertOrUpdate(User user)
        {
            if (user.Id == 0)
            {
                user.EntryDatetime = DateTime.Now;
                context.User.Add(user);
            }
            context.SaveChanges();
        }

        public bool IsValidUser(string username, string password)
        {
            return context.User.Any(x => x.Username.Equals(username) && x.Password.Equals(password));
        }

        public UserClinicalTrial GetClinicalTrial(int clinicalTrialId)
        {
            return context.UserClinicalTrial.FirstOrDefault(x => x.Id == clinicalTrialId);
        }

        public IQueryable<UserClinicalTrial> GetClinicalTrialsByUser(int userId)
        {
            return context.UserClinicalTrial.Where(x => x.UserId == userId);
        }

        public void SaveClinicalTrial(UserClinicalTrial clinicalTrial) 
        {
            if (clinicalTrial.Id == 0)
            {
                context.UserClinicalTrial.Add(clinicalTrial);
            }
            else 
            {
            
            }

            context.SaveChanges();
        }

        public bool IsUserStillValid(int id)
        {
            User user = this.GetById(id);
            return user.Organizations.FirstOrDefault(x => x.OrganizationId == user.UserConfig.ActiveOrganizationId).State == UserState.Active;
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
            return context.User.Where(x => ids.Contains(x.Id)).ToList();
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
                .Where(x => x.Organizations.Select(o => o.Id).Any(i => organizationIds.Contains(i))).ToList();
        }

        public bool IsUsernameValid(string username)
        {
            return context.User
                .Include(x => x.ClinicalTrials)
                .Include(x => x.Organizations)
                .Include(x => x.UserConfig)
                .FirstOrDefault(x => x.Username == username && !x.IsDeleted) == null;
        }

        public bool IsEmailValid(string email)
        {
            return context.User
                .Include(x => x.ClinicalTrials)
                .Include(x => x.Organizations)
                .Include(x => x.UserConfig)
                .FirstOrDefault(x => x.Email == email && !x.IsDeleted) == null;
        }

        public bool UserExist(int id)
        {
            return context.User.Any(x => x.Id == id && !x.IsDeleted);
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
        public void UpdateOrganizationsUserCounts(User user, User userForUpdate)
        {
            IncrementOrganizationsUserCount(user.GetOrganizationRefs().Where(x => !userForUpdate.GetOrganizationRefs().Contains(x)).ToList(), 1);
            IncrementOrganizationsUserCount(userForUpdate.GetOrganizationRefs().Where(x => !user.GetOrganizationRefs().Contains(x)).ToList(), -1);
        }

        private void IncrementOrganizationsUserCount(List<int> organizationIds, int value)
        {
            foreach (var organization in context.Organization.Where(x => organizationIds.Contains(x.Id))) 
            {
                organization.NumOfUsers += value;
            }

            context.SaveChanges();
        }
    }
}
