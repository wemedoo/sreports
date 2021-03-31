using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.UserEntities;
using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Domain.Enums;
using sReportsV2.Domain.Entities.OrganizationEntities;
using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
using sReportsV2.Domain.Entities.PatientEntities;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Implementations;

namespace sReportsV2.Domain.Entities.FormInstance
{
    [BsonIgnoreExtraElements]
    public class FormInstance : Entity
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

        [BsonIgnore]
        public List<FormChapter> Chapters { get; set; } = new List<FormChapter>();
        public DocumentProperties.DocumentProperties DocumentProperties { get; set; }
        public List<string> Referrals { get; set; }

        [BsonIgnore]
        public UserEntities.User User { get; set; }
        
        [BsonIgnore]
        public Organization Organization { get; set; }

        [BsonIgnore]
        public PatientEntity Patient { get; set; }


        public FormInstance() { }
        public FormInstance(Form.Form form)
        {
            form = Ensure.IsNotNull(form, nameof(form));

            this.FormDefinitionId = form.Id.ToString();
            this.Title = form.Title;
            this.Version = form.Version;
            this.EntryDatetime = form.EntryDatetime;
            this.LastUpdate = DateTime.Now;
            this.Language = form.Language;
            this.DocumentProperties = form.DocumentProperties;
            this.ThesaurusId = form.ThesaurusId;
        }
        


        /*public Field GetFieldByThesaurus(string thesaurusId)
        {
            return GetAllFields().Where(x => x.ThesaurusId.Equals(thesaurusId)).FirstOrDefault();
        }*/
        public List<FieldSet> GetAllFieldSets(Form.Form formm)
        {
            Form.Form form = new Form.Form(this, formm);
            return form.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(listOfFS => listOfFS)
                            )
                        ).ToList();
        }

        /*public List<Field> GetAllFields()
        {
            //problem!
            FormService service = new FormService();
            Form.Form form = new Form.Form(this, service.GetForm(this.FormDefinitionId));
            return form.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(fieldSet => fieldSet
                                    .SelectMany(set => set.Fields
                                    )
                                )
                            )
                        ).ToList();
        }*/

        public void SetValueByThesaurusId(string thesaurusId, string value)
        {
            FieldValue field = this.Fields.FirstOrDefault(x => x.ThesaurusId == thesaurusId);
            if (field != null)
            {
                field.SetValue(value);
            }
        }

        public void SetFieldValue(string id, List<string> value)
        {
            FieldValue field = Fields.FirstOrDefault(x => x.Id.Equals(id));
            if (field != null)
            {
                field.Value = value;
            }
        }

        public string GetFieldValueById(string id)
        {
            return this.Fields.FirstOrDefault(x => x.Id.Equals(id))?.Value?[0];
        }

        public enum SmsState
        {
            WaitingForVerify,
            Verified,
            Notified
        }

    }
}
