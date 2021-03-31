using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using SendGrid;
using SendGrid.Helpers.Mail;
using Serilog;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.UserEntities;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IMongoCollection<User> Collection;
        public UserService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<User>("users") as IMongoCollection<User>;
        }

        public UserData GetById(string id)
        {
            return Collection.AsQueryable()
            .Where(x => x.Id.Equals(id))
            .Select(x => new UserData()
            {
                Id = id,
                ActiveOrganization = x.ActiveOrganization,
                FirstName = x.FirstName,
                LastName = x.LastName,
                OrganizationRefs = x.OrganizationRefs,
                Username = x.Username
            })
            .FirstOrDefault();
        }

        public User GetByUsername(string username)
        {
            return Collection.Find(x => x.Username.Equals(username)).FirstOrDefault();
        }

        public bool IsValidUser(string username, string password)
        {
            return Collection.Find(x => x.Username.Equals(username) && x.Password.Equals(password)).CountDocuments() > 0;
        }

        public void UpdateLanguage(string username, string language)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Username, username);
            var updateDef = Builders<User>.Update.Set(s => s.ActiveLanguage, language);
            UpdateUser(filter, updateDef);
        }

        public void UpdatePageSize(string username, int pageSize)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Username, username);
            var updateDef = Builders<User>.Update.Set(s => s.PageSize, pageSize);
            UpdateUser(filter, updateDef);
        }
        public void UpdateOrganization(string username, string organization)
        {
            var filter = Builders<User>.Filter.Eq(x => x.Username, username);
            var updateDef = Builders<User>.Update.Set(s => s.ActiveOrganization, organization);
            UpdateUser(filter, updateDef);
        }

        private void UpdateUser(FilterDefinition<User> filter, UpdateDefinition<User> updateDefinition)
        {
            try
            {
                Collection.UpdateOne(filter, updateDefinition);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw new Exception(ex.Message, ex.InnerException);
            }
        }

        public long GetAllEntriesCount()
        {
            return Collection.Find(x => !x.IsDeleted).CountDocuments();
        }

        public List<User> GetAll(int pageSize, int page)
        {
            return Collection
                .Find(x => !x.IsDeleted)
                .SortByDescending(x => x.EntryDatetime)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
        }

        public void Insert(User user, string activeOrganization, string activeLanguage)
        {
            user = Ensure.IsNotNull(user, nameof(user));

            if (user.Id == null)
            {
                user.EntryDatetime = DateTime.Now;
                user.LastUpdate = user.EntryDatetime;
                user.Password = CreatePassword(8);
                user.ActiveOrganization = activeOrganization;
                user.ActiveLanguage = activeLanguage;
                Collection.InsertOne(user);
            }
            else
            {
                User userForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(user.Id));
                user.EntryDatetime = userForUpdate.EntryDatetime;
                userForUpdate.DoConcurrencyCheck(user.LastUpdate.Value);
                user.Password = userForUpdate.Password;
                user.LastUpdate = DateTime.Now;
                user.ActiveOrganization = activeOrganization;
                user.ActiveLanguage = activeLanguage;
                FilterDefinition<User> filter = Builders<User>.Filter.Eq(s => s.Id, user.Id);
                var result = Collection.ReplaceOne(filter, user).ModifiedCount;
            }
        }

        public User GetUserById(string id)
        {
            return Collection.Find(x => x.Id.Equals(id) && !x.IsDeleted).FirstOrDefault();
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            User forDelete = GetUserById(id);
            DoConcurrencyCheckForDelete(forDelete);
            forDelete.DoConcurrencyCheck(lastUpdate);

            var filter = Builders<User>.Filter.Eq(x => x.Id, id);
            var update = Builders<User>.Update.Set(x => x.IsDeleted, true);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public void UpdatePassword(User user, string password)
        {
            user = Ensure.IsNotNull(user, nameof(user));
            password = Ensure.IsNotNull(password, nameof(password));

            User userForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(user.Id));
            user.EntryDatetime = userForUpdate.EntryDatetime;
            userForUpdate.DoConcurrencyCheck(user.LastUpdate.Value);
            user.Password = password;
            user.LastUpdate = DateTime.Now;
            FilterDefinition<User> filter = Builders<User>.Filter.Eq(s => s.Id, user.Id);
            var result = Collection.ReplaceOne(filter, user).ModifiedCount;

        }

        private void DoConcurrencyCheckForDelete(User forDelete)
        {
            if (forDelete == null)
            {
                throw new MongoDbConcurrencyDeleteException();
            }
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
       
    }
}
