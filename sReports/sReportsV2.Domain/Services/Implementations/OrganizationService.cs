namespace sReportsV2.Domain.Services.Implementations
{
    public class OrganizationService //: IOrganizationService
    {
        /*private readonly IMongoCollection<Organization> Collection;
        public OrganizationService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<Organization>("organizationentity");
        }

        public long GetAllEntriesCount(OrganizationFilter filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            return this.GetOrganizationFiltered(filter).Count();
        }

        public long GetAllCount()
        {
            return Collection.AsQueryable().Where(x => !x.IsDeleted).Count();
        }

        public List<Organization> GetAll(OrganizationFilter filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            return this.GetOrganizationFiltered(filter)
                .OrderByDescending(x => x.EntryDatetime)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
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
            return Collection
                .Find(x => x.Id.Equals(id) && !x.IsDeleted)
                .FirstOrDefault();
        }

        public List<Organization> GetOrganizationByOrganizationId(string id)
        {
            return Collection
                .Find(x => x.Id.Equals(id) && !x.IsDeleted)
                .ToList();
        }

        public Organization GetOrganizationByName(string name)
        {
            return Collection
                .Find(x => x.Name.Equals(name) && !x.IsDeleted)
                .FirstOrDefault();
        }

        public List<Organization> GetByIds(List<string> ids)
        {
            return Collection
                .AsQueryable()
                .Where(x => !x.IsDeleted && ids.Contains(x.Id))
                .ToList();
        }

        public List<Organization> GetOrganizationsByIds(List<string> ids)
        {
            return Collection
                .AsQueryable()
                .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
                .ToList();
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
                .Find(x => x.Name.ToUpper(CultureInfo.InvariantCulture).Contains(name.ToUpper(CultureInfo.InvariantCulture)) && !x.IsDeleted)
                .CountDocuments();
        }

        public bool ExistsOrganizationByIdentifier(IdentifierEntity identifier)
        {
            return Collection.Find(x => x.Identifiers
                                   .Any(y => y.System.Equals(identifier.System) && y.Value.Equals(identifier.Value)))
                                   .CountDocuments() > 0;
        }

        public bool ExistsOrganizationById(string id)
        {
            return Collection
                .Find(x => x.Id.Equals(id))
                .CountDocuments() > 0;
        }

        public List<OrganizationUsersCount> GetOrganizationUsersCount(string term, List<string> countries)
        {
            List<OrganizationUsersCount> result = new List<OrganizationUsersCount>();
            result = Collection.AsQueryable().Where(x => !x.IsDeleted)
            .Select(organization => new OrganizationUsersCount()
            {
                OrganizationName = organization.Name,
                UsersCount = organization.NumOfUsers,
                PartOf = organization.PartOf,
                OrganizationId = organization.Id,
                Country = organization.Address != null ? organization.Address.Country : string.Empty
            })
            .ToList();

            var ancestor = result.Where(x => x.PartOf == null);

            SetOrganizationsChildren(result);

            return ancestor
                .Where(x => string.IsNullOrWhiteSpace(term) || x.FoundName(term))
                .Where(x => countries == null || countries.Count == 0 || x.FoundCountry(countries))
                .ToList();
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

        public bool Delete(string id, DateTime lastUpdate)
        {
            Organization forDelete = GetOrganizationById(id);
            Entity.DoConcurrencyCheckForDelete(forDelete);
            forDelete.DoConcurrencyCheck(lastUpdate);

            var filter = Builders<Organization>.Filter.Eq(x => x.Id, id);
            var update = Builders<Organization>
                .Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }


        private IQueryable<Organization> GetOrganizationFiltered(OrganizationFilter filter)
        {
            IQueryable<Organization> organizationQuery = this.Collection.AsQueryable().Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(filter.ClinicalDomain))
            {
                organizationQuery = organizationQuery.Where(x => x.ClinicalDomain.Equals(filter.ClinicalDomain));
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                organizationQuery = organizationQuery.Where(x => x.Name != null && x.Name.ToUpper(CultureInfo.InvariantCulture).Contains(filter.Name.ToUpper(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(filter.City))
            {
                organizationQuery = organizationQuery.Where(x => x.Address != null && x.Address.City != null && x.Address.City.ToUpper(CultureInfo.InvariantCulture).Contains(filter.City.ToUpper(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(filter.Type))
            {
                organizationQuery = organizationQuery.Where(x => x.Type.Equals(filter.Type));
            }

            if (!string.IsNullOrEmpty(filter.Alias))
            {
                organizationQuery = organizationQuery.Where(x => x.Alias != null && x.Alias.ToUpper(CultureInfo.InvariantCulture).Contains(filter.Alias.ToUpper(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(filter.Activity))
            {
                organizationQuery = organizationQuery.Where(x => x.Activity.Equals(filter.Activity));
            }

            if (!string.IsNullOrEmpty(filter.State))
            {
                organizationQuery = organizationQuery
                    .Where(x => x.Address != null && x.Address.State != null && x.Address.State.ToUpper(CultureInfo.InvariantCulture).Contains(filter.State.ToUpper(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(filter.Country))
            {
                organizationQuery = organizationQuery
                    .Where(x => x.Address != null && x.Address.Country != null && x.Address.Country.ToUpper(CultureInfo.InvariantCulture).Contains(filter.Country.ToUpper(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(filter.PostalCode))
            {
                organizationQuery = organizationQuery
                    .Where(x => x.Address != null && x.Address.PostalCode != null && x.Address.PostalCode.ToUpper(CultureInfo.InvariantCulture).Contains(filter.PostalCode.ToUpper(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(filter.Street))
            {
                organizationQuery = organizationQuery
                    .Where(x => x.Address != null && x.Address.Street != null && x.Address.Street.ToUpper(CultureInfo.InvariantCulture).Contains(filter.Street.ToUpper(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(filter.PartOf))
            {
                var partOf = (GetOrganizationByName(filter.PartOf)?.Id) ?? string.Empty;
                organizationQuery = organizationQuery.Where(x => x.PartOf.Equals(partOf));
            }

            organizationQuery = FilterByIdentifier(organizationQuery, filter.IdentifierType, filter.IdentifierValue);
            return organizationQuery;
        }

        private IQueryable<Organization> FilterByIdentifier(IQueryable<Organization> organizationEntities, string system, string value)
        {
            IQueryable<Organization> result = null;
            if (!string.IsNullOrEmpty(system) && !string.IsNullOrEmpty(value))
            {
                if (system.Equals(ResourceTypes.O4PatientId))
                {
                    result = organizationEntities.Where(x => x.Id.Equals(value));
                }
                else
                {
                    result = organizationEntities.Where(x => x.Identifiers.Any(y => !string.IsNullOrEmpty(y.System) && !string.IsNullOrEmpty(y.Value) && y.System.Equals(system) && y.Value.Equals(value)));
                }
            }
            else
            {
                result = organizationEntities;
            }

            return result;
        }

        private void SetOrganizationsChildren(List<OrganizationUsersCount> allOrganization)
        {
            foreach (OrganizationUsersCount organization in allOrganization)
            {
                organization.Children = SetOrganizationChildren(organization.OrganizationId, allOrganization);
            }
        }

        private List<OrganizationUsersCount> SetOrganizationChildren(string organizationId, List<OrganizationUsersCount> allOrganizations)
        {
            List<OrganizationUsersCount> allChildren = new List<OrganizationUsersCount>();
            foreach (OrganizationUsersCount organization in allOrganizations.Where(x => x.PartOf != null && x.PartOf == organizationId))
            {
                if (organizationId == organization.PartOf)
                {
                    allChildren.Add(organization);
                }
            }
            return allChildren;
        }

        public long GetAllEntriesCountByCountry(string country)
        {
            return Collection.AsQueryable().Where(x => x.Address != null && x.Address.Country == country).Count();
        }

        public Organization GetFirsOrDefault()
        {
            return Collection.AsQueryable().FirstOrDefault(x => !x.IsDeleted);
        
        }*/
    }
}
