using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.FormValues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.FormInstance
{
    public class FormOverridden
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string FormDefinitionId { get; set; }
        public string Title { get; set; }
        public sReportsV2.Domain.Entities.Form.Version Version { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserRef { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrganizationRef { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string EncounterRef { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string PatientRef { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string EpisodeOfCareRef { get; set; }
        public string Notes { get; set; }
        public FormState? FormState { get; set; }
        public DateTime? Date { get; set; }
        public string ThesaurusId { get; set; }
        public string Language { get; set; }
        public List<FieldValue> Fields { get; set; }
        public int DocumentsCount { get; set; }



        [BsonIgnore]
        public List<FormChapter> Chapters { get; set; } = new List<FormChapter>();
        public List<string> Referrals { get; set; }

        [BsonIgnore]
        public int UserId { get; set; }

        [BsonIgnore]
        public int OrganizationId { get; set; }

        [BsonIgnore]
        public PatientEntity Patient { get; set; }
        public DocumentPropertiesOverriden DocumentProperties { get; set; }
    }
}
