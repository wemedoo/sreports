using MongoDB.Bson;
using MongoDB.Driver;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Entities.UserEntities;
using sReportsV2.Domain.Exceptions;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace sReportsV2.Domain.Services.Implementations
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IMongoCollection<Organization> Collection;
        private readonly IMongoCollection<User> CollectionUser;
        public OrganizationService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<Organization>("organizationentity") as IMongoCollection<Organization>;
            CollectionUser = MongoDatabase.GetCollection<User>("users") as IMongoCollection<User>;
        }

        public void Insert(Organization organization)
        {
            organization = Ensure.IsNotNull(organization, nameof(organization));

            organization.SetAncestors(GetOrganizationById(organization.PartOf));

            if (organization.Id == null)
            {
                organization.EntryDatetime = DateTime.Now;
                organization.LastUpdate = organization.EntryDatetime;
                Collection.InsertOne(organization);
            }
            else
            {
                Organization organizationForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(organization.Id));
                organization.EntryDatetime = organizationForUpdate.EntryDatetime;
                organizationForUpdate.DoConcurrencyCheck(organization.LastUpdate.Value);

                organization.LastUpdate = DateTime.Now;
                FilterDefinition<Organization> filter = Builders<Organization>.Filter.Eq(s => s.Id, organization.Id);
                var result = Collection.ReplaceOne(filter, organization).ModifiedCount;
            }
        }

        public long GetAllEntriesCount()
        {
            return Collection.Find(x => !x.IsDeleted).CountDocuments();
        }

        public List<Organization> GetAll(int pageSize, int page)
        {
            return Collection
                .Find(x => !x.IsDeleted)
                .SortByDescending(x => x.EntryDatetime)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToList();
        }
        public List<Organization> GetOrganizations()
        {
            return Collection
                .Find(x => !x.IsDeleted)
                .SortByDescending(x => x.EntryDatetime)
                .ToList();
        }

        public Organization GetOrganizationById(string id)
        {
            return Collection.Find(x => x.Id.Equals(id) && !x.IsDeleted).FirstOrDefault();
        }
        public List<Organization> GetOrganizationByOrganizationId(string id)
        {
            return Collection.Find(x => x.Id.Equals(id) && !x.IsDeleted).ToList();
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            Organization forDelete = GetOrganizationById(id);
            DoConcurrencyCheckForDelete(forDelete);
            forDelete.DoConcurrencyCheck(lastUpdate);

            var filter = Builders<Organization>.Filter.Eq(x => x.Id, id);
            var update = Builders<Organization>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public List<Organization> GetOrganizationsByIds(List<string> ids)
        {
            return Collection.AsQueryable().Where(x => ids.Contains(x.Id) && !x.IsDeleted).ToList();
        }

        public List<Organization> GetByParameters(OrganizationFilter organizationFilter)
        {
            organizationFilter = Ensure.IsNotNull(organizationFilter, nameof(organizationFilter));

            IdentifierEntity entity = new IdentifierEntity();
            if (!string.IsNullOrEmpty(organizationFilter.Identifier))
            {
                entity.System = organizationFilter.Identifier.Split('|')[0];
                entity.Value = organizationFilter.Identifier.Split('|')[1];
            }

            return Collection.Find(x => (organizationFilter.Type == null || x.Type.Equals(organizationFilter.Type))
                                                    && (organizationFilter.Name == null || x.Name.Equals(organizationFilter.Name))
                                                    && (organizationFilter.PartOf == null || x.PartOf.Equals(organizationFilter.PartOf))
                                                    && (organizationFilter.Identifier == null || (x.Identifiers.Any(y => y.System.Equals(entity.System))) && x.Identifiers.Any(y => y.Value.Equals(entity.Value)))
                                                    && !x.IsDeleted).ToList();
        }

        public List<Organization> SearchByName(string name, int page, int pageSize)
        {
            return Collection
                .Find(x => x.Name.ToUpper(CultureInfo.InvariantCulture).Contains(name.ToUpper(CultureInfo.InvariantCulture)))
                .Skip(page * pageSize)
                .Limit(pageSize)
                .ToList();
        }

        public long GetSearchByNameCount(string name)
        {
            return Collection
                .Find(x => x.Name.ToUpper(CultureInfo.InvariantCulture).Contains(name.ToUpper(CultureInfo.InvariantCulture)) && !x.IsDeleted).CountDocuments();
        }

        public bool ExistsOrganizationByIdentifier(IdentifierEntity identifier)
        {
            return Collection.Find(x => x.Identifiers
                                   .Any(y => y.System.Equals(identifier.System) && y.Value.Equals(identifier.Value)))
                                   .CountDocuments() > 0;
        }

        public bool ExistsOrganizationById(string id)
        {
            return Collection.Find(x => x.Id.Equals(id))
                                   .CountDocuments() > 0;
        }

        public List<OrganizationUsersCount> GetOrganizationUsersCount()
        {
            List<OrganizationUsersCount> result = new List<OrganizationUsersCount>();
            foreach(Organization organization in Collection.AsQueryable().Where(x => !x.IsDeleted).ToList())
            {
                result.Add(new OrganizationUsersCount()
                {
                    OrganizationName = organization.Name,
                    UsersCount = CollectionUser.AsQueryable().Where(x => x.OrganizationRefs.Contains(organization.Id)).Count(),
                    PartOf = organization.PartOf,
                    OrganizationId = organization.Id
                });
            }
            var ancestor = result.Where(x => x.PartOf == null);
            var descendants = result.Where(x => x.PartOf != null);
            SetOrganizationChildren(descendants, ancestor);
            return ancestor.ToList();
        }

        private void SetOrganizationChildren(IEnumerable<OrganizationUsersCount> allData, IEnumerable<OrganizationUsersCount> data)
        {
            foreach (OrganizationUsersCount organization in data)
            {
                var children = allData.Where(x => x.PartOf.Equals(organization.OrganizationId));
                if (children.Any())
                {
                    SetOrganizationChildren(allData.Where(x => !children.Select(y => y.OrganizationId).Contains(x.OrganizationId)), children);
                }
                organization.Children = children.OrderByDescending(x => x.UsersCount).ToList();
            }
        }
        private void DoConcurrencyCheckForDelete(Organization forDelete)
        {
            if (forDelete == null)
            {
                throw new MongoDbConcurrencyDeleteException();
            }
        }
    }
}
