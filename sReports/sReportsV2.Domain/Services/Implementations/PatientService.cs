using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.FormInstance;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Enums;
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
    public class PatientService : IPatientService
    {
        private readonly IMongoCollection<PatientEntity> Collection;
        private readonly IMongoCollection<EpisodeOfCareEntity> eocCollection;
        private readonly IMongoCollection<EncounterEntity> encounterCollection;
        private readonly IMongoCollection<FormInstance> formInstanceCollection;



        private readonly IMongoCollection<IdentifierType> IdentifierTypeCollection;
        public PatientService()
        {
            IMongoDatabase MongoDatabase = MongoDBInstance.Instance.GetDatabase();
            Collection = MongoDatabase.GetCollection<PatientEntity>("patiententity") as IMongoCollection<PatientEntity>;
            eocCollection = MongoDatabase.GetCollection<EpisodeOfCareEntity>("episodeofcareentity") as IMongoCollection<EpisodeOfCareEntity>;
            encounterCollection = MongoDatabase.GetCollection<EncounterEntity>("encounterentity") as IMongoCollection<EncounterEntity>;
            formInstanceCollection = MongoDatabase.GetCollection<FormInstance>("forminstance") as IMongoCollection<FormInstance>;

            IdentifierTypeCollection = MongoDatabase.GetCollection<IdentifierType>("identifiertype") as IMongoCollection<IdentifierType>;
        }

        public List<PatientEntity> GetAll(PatientFilter filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            return this.GetPatientFiltered(filter)
                .OrderByDescending(x => x.EntryDatetime)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();
        }

        public PatientEntity GetById(string id)
        {
            return Collection.Find(x => !x.IsDeleted && x.Id.Equals(id)).FirstOrDefault();
        }

        public PatientEntity GetByIdentifier(IdentifierEntity identifier)
        {
            identifier = Ensure.IsNotNull(identifier, nameof(identifier));

            PatientEntity result = null;
            if (identifier.System.Equals(ResourceTypes.O4PatientId))
            {
                result = GetById(identifier.Value);
            }
            else
            {
                result = Collection
                    .Find(x => !x.IsDeleted && x.Identifiers
                        .Any(y => y.Value.Equals(identifier.Value) && y.System.Equals(identifier.System)))
                    .FirstOrDefault();
            }
            return result;
        }

        public int GetAllEntriesCount(PatientFilter filter)
        {
            filter = Ensure.IsNotNull(filter, nameof(filter));

            return this.GetPatientFiltered(filter).Count();
        }

        public string Insert(PatientEntity patient)
        {
            patient = Ensure.IsNotNull(patient, nameof(patient));

            if (patient.Id == null)
            {
                patient.EntryDatetime = DateTime.Now;
                patient.IsDeleted = false;
                patient.LastUpdate = patient.EntryDatetime;
                Collection.InsertOne(patient);
            }
            else
            {
                PatientEntity patientForUpdate = Collection.AsQueryable().FirstOrDefault(x => x.Id.Equals(patient.Id));
                patient.EntryDatetime = patientForUpdate.EntryDatetime;
                patientForUpdate.DoConcurrencyCheck(patient.LastUpdate.Value);

                patient.LastUpdate = DateTime.Now;
                var filter = Builders<PatientEntity>.Filter.Eq(s => s.Id, patient.Id);
                var result = Collection.ReplaceOne(filter, patient).ModifiedCount;
            }

            return patient.Id.ToString();
        }

        public bool ExistsPatietById(string umcn)
        {
            return this.Collection.AsQueryable().Any(x => !x.IsDeleted && x.Identifiers.Any(y => y.Value.Equals(umcn)));
        }

        public bool Delete(string id, DateTime lastUpdate)
        {
            PatientEntity forDelete = GetById(id);
            DoConcurrencyCheckForDelete(forDelete);
            forDelete.DoConcurrencyCheck(lastUpdate);

            var filter = Builders<PatientEntity>.Filter.Eq(x => x.Id, id);
            var update = Builders<PatientEntity>.Update.Set(x => x.IsDeleted, true).Set(x => x.LastUpdate, DateTime.Now);
            return Collection.UpdateOne(filter, update).IsAcknowledged;
        }

        public bool ExistsPatientByIdentifier(IdentifierEntity identifier)
        {
            identifier = Ensure.IsNotNull(identifier, nameof(identifier));

            if (identifier.System.Equals(ResourceTypes.O4PatientId))
            {
                return ExistsPatietById(identifier.Value);
            }
            else
            {
                return Collection
                    .Find(x => !x.IsDeleted && x.Identifiers
                        .Any(y => y.Value.Equals(identifier.Value) && y.System.Equals(identifier.System)))
                    .CountDocuments() > 0;
            }
        }

        public bool ExistsPatientByObjectId(string id)
        {
            return Collection.Find(x => !x.IsDeleted && x.Id.Equals(id)).CountDocuments() > 0;
        }

        public List<IdentifierType> GetIdentifierTypes(IdentifierKind kind)
        {
            return this.IdentifierTypeCollection.AsQueryable().Where(x => x.Type.Equals(kind.ToString())).OrderBy(x => x.Name).ToList();
        }

        private IQueryable<PatientEntity> GetPatientFiltered(PatientFilter filter)
        {
            DateTime? beginDate = null;
            DateTime? endDate = null;
            IQueryable<PatientEntity> patientQuery = this.Collection.AsQueryable().Where(x => !x.IsDeleted);
            if (filter.BirthDate != null)
            {
                beginDate = filter.BirthDate ?? DateTime.Now;
                endDate = beginDate.Value.AddDays(1);
                patientQuery = patientQuery.Where(x => x.BirthDate >= beginDate && x.BirthDate <= endDate);
            }


            if (!string.IsNullOrEmpty(filter.City))
            {
                patientQuery = patientQuery.Where(x => x.Addresss != null && x.Addresss.City != null && x.Addresss.City.ToUpper(CultureInfo.InvariantCulture).Contains(filter.City.ToUpper(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(filter.Country))
            {
                patientQuery = patientQuery.Where(x => x.Addresss != null && x.Addresss.Country != null && x.Addresss.Country.ToUpper(CultureInfo.InvariantCulture).Contains(filter.Country.ToUpper(CultureInfo.InvariantCulture)));
            }

            if (!string.IsNullOrEmpty(filter.Family))
            {
                patientQuery = patientQuery.Where(x => x.Name != null && x.Name.Family != null && x.Name.Family.ToUpper(CultureInfo.InvariantCulture).Contains(filter.Family));
            }

            if (!string.IsNullOrEmpty(filter.Given))
            {
                patientQuery = patientQuery.Where(x => x.Name != null && x.Name.Given != null && x.Name.Given.ToUpper(CultureInfo.InvariantCulture).Contains(filter.Given));
            }
            if (!string.IsNullOrEmpty(filter.PostalCode))
            {
                patientQuery = patientQuery.Where(x => x.Addresss != null && x.Addresss.PostalCode != null && x.Addresss.PostalCode.ToUpper(CultureInfo.InvariantCulture).Contains(filter.PostalCode));
            }
            patientQuery = FilterByIdentifier(patientQuery, filter.IdentifierType, filter.IdentifierValue);
            return patientQuery;
        }

        private IQueryable<PatientEntity> FilterByIdentifier(IQueryable<PatientEntity> patientEntities, string system, string value)
        {
            IQueryable<PatientEntity> result = null;
            if (!string.IsNullOrEmpty(system) && !string.IsNullOrEmpty(value))
            {
                if (system.Equals(ResourceTypes.O4PatientId))
                {
                    result = patientEntities.Where(x => x.Id.Equals(value));
                }
                else
                {
                    result = patientEntities.Where(x => x.Identifiers.Any(y => !string.IsNullOrEmpty(y.System) && !string.IsNullOrEmpty(y.Value) && y.System.Equals(system) && y.Value.Equals(value)));
                }
            }
            else
            {
                result = patientEntities;
            }

            return result;
        }

        public List<PatientEntity> GetByParameters(PatientFhirFilter patientFilter)
        {
            patientFilter = Ensure.IsNotNull(patientFilter, nameof(patientFilter));

            IdentifierEntity entity = new IdentifierEntity();
            if (!string.IsNullOrEmpty(patientFilter.Identifier))
            {
                entity.System = patientFilter.Identifier.Split('|')[0];
                entity.Value = patientFilter.Identifier.Split('|')[1];
            }

            return Collection.Find(x => (patientFilter.Family == null || x.Name.Family.Equals(patientFilter.Family))
                                            && (patientFilter.Given == null || x.Name.Given.Equals(patientFilter.Given))
                                            && (patientFilter.City == null || x.Addresss.City.Equals(patientFilter.City))
                                            && (patientFilter.Country == null || x.Addresss.Country.Equals(patientFilter.Country))
                                            && (patientFilter.PostalCode == null || x.Addresss.PostalCode.Equals(patientFilter.PostalCode))
                                            && (patientFilter.State == null || x.Addresss.State.Equals(patientFilter.State))
                                            && (patientFilter.Identifier == null || (x.Identifiers.Any(y => y.System.Equals(entity.System))) && x.Identifiers.Any(y => y.Value.Equals(entity.Value)))
                                            && !x.IsDeleted).ToList();
        }

        public List<PatientEntity> GetByIds(List<string> ids)
        {
            return this.Collection.AsQueryable().Where(x => ids.Contains(x.Id) && !x.IsDeleted).ToList();
        }

        private void DoConcurrencyCheckForDelete(PatientEntity forDelete) 
        {
            if (forDelete == null)
            {
                throw new MongoDbConcurrencyDeleteException();
            }
        }

        public PatientEntity GetExpandedPatientById(string id, string organizationId)
        {
            var result = Collection.AsQueryable()
                .Where(x => x.Id.Equals(id))
                    .GroupJoin(
                        eocCollection.AsQueryable(),
                         p => p.Id,
                         o => o.PatientId,
                         (p, o) => new PatientEntity
                         {
                             Id = p.Id,
                             EntryDatetime = p.EntryDatetime,
                             LastUpdate = p.LastUpdate,
                             Name = p.Name,
                             Identifiers = p.Identifiers,
                             Addresss = p.Addresss,
                             Active = p.Active,
                             Gender = p.Gender,
                             BirthDate = p.BirthDate,
                             MultipleB = p.MultipleB,
                             ContactPerson = p.ContactPerson,
                             Telecoms = p.Telecoms,
                             Communications = p.Communications,
                             EpisodeOfCares = o as List<EpisodeOfCareEntity>
                         }
                   ).First();

            result.EpisodeOfCares = result.EpisodeOfCares.Where(e => !e.IsDeleted && e.OrganizationRef == organizationId).ToList();
            foreach (EpisodeOfCareEntity eoc in result.EpisodeOfCares) 
            {
               eoc.Encounters =  encounterCollection.AsQueryable()
                    .Where(x => x.EpisodeOfCareId == eoc.Id && !x.IsDeleted)
                    .GroupJoin(
                        formInstanceCollection.AsQueryable(),
                        p => p.Id,
                        o => o.EncounterRef,
                         (p, o) => new EncounterEntity
                         {
                             Id = p.Id,
                             EntryDatetime = p.EntryDatetime,
                             LastUpdate = p.LastUpdate,
                             EpisodeOfCareId = p.EpisodeOfCareId,
                             Status = p.Status,
                             Class = p.Class,
                             Type = p.Type,
                             ServiceType = p.ServiceType,
                             Period = p.Period,
                             FormInstances = o as List<FormInstance>
                         }
                    ).ToList();

                foreach (EncounterEntity encounter in eoc.Encounters) 
                {
                    encounter.FormInstances = encounter.FormInstances.Where(f => !f.IsDeleted).ToList();
                }

            }

            return result;
        }
    }
}
