using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Encounter;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.EpisodeOfCareEntities
{
    [BsonIgnoreExtraElements]
    public class EpisodeOfCareEntity : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientId { get; set; }
        public List<EpisodeOfCareStatus> ListHistoryStatus { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrganizationRef { get; set; }
        public EOCStatus Status { get; set; }
        public string Type { get; set; }
        public string DiagnosisCondition { get; set; }
        public string DiagnosisRole { get; set; }
        public string DiagnosisRank { get; set; }
        public Period Period { get; set; }
        public string Description { get; set; }
        [BsonIgnore]
        public List<EncounterEntity> Encounters { get; set; }
        public void UpdateHistory()
        {
            EpisodeOfCareStatus newStatus = new EpisodeOfCareStatus();
            if (this.ListHistoryStatus == null || this.ListHistoryStatus.Count == 0)
            {
                this.ListHistoryStatus = new List<EpisodeOfCareStatus>();
                newStatus.StatusValue = this.Status;
                newStatus.StartTime = DateTime.Now;
                this.ListHistoryStatus.Add(newStatus);
            }
            else
            {
                newStatus.StatusValue = this.Status;
                DateTime now = DateTime.Now;
                newStatus.StartTime = now;
                EpisodeOfCareStatus status = ListHistoryStatus.FirstOrDefault();
                status.EndTime = now;
                this.ListHistoryStatus.Add(newStatus);
            }            
        }

        /*public void CreateEncounter(string encounterId)
        {
            if(Encounters == null)
            {
                Encounters = new List<string>();
            }

            Encounters.Add(encounterId);
        }*/
    }
}
