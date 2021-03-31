using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.UserEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Interfaces
{
    public interface IUserService
    {
        UserData GetById(string id);
        User GetByUsername(string username);
        bool IsValidUser(string username, string password);
        void UpdateLanguage(string username, string language);
        void UpdatePageSize(string username, int pageSize);
        void UpdateOrganization(string username, string organization);
        long GetAllEntriesCount();
        List<User> GetAll(int pageSize, int page);
        void Insert(User user, string activeOrganization, string activeLanguage);
        bool Delete(string userId, DateTime lastUpdate);
        User GetUserById(string id);
        void UpdatePassword(User user, string password);
    }
}
