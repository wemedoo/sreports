using sReportsV2.Common.Enums;
using sReportsV2.Domain.Sql.Entities.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.DAL.Sql.Interfaces
{
    public interface IUserDAL
    {
        int CountAll();
        bool IsValidUser(string username, string password);
        User GetById(int id);
        User GetByUsername(string username);
        User GetByEmail(string email);
        void InsertOrUpdate(User user);
        IQueryable<User> GetAllByActiveOrganization(int activeOrganization);
        UserClinicalTrial GetClinicalTrial(int clinicalTrialId);
        void SaveClinicalTrial(UserClinicalTrial clinicalTrial);
        IQueryable<UserClinicalTrial> GetClinicalTrialsByUser(int userId);
        bool IsUserStillValid(int id);
        void Save();
        List<User> GetAllByIds(List<int> ids);
        long GetAllCount();
        List<User> GetAllByOrganizationIds(List<int> organizationIds);
        bool IsEmailValid(string email);
        bool IsUsernameValid(string username);
        bool UserExist(int id);
        void SetState(int id, UserState state, int organizationId);
        void UpdateOrganizationsUserCounts(User user, User userForUpdate);
    }
}
