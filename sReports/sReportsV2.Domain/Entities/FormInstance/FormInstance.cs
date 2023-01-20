using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.FormValues;
using sReportsV2.Common.Extensions;
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
        public int EncounterRef { get; set; }
        public int EpisodeOfCareRef { get; set; }
        public string Notes { get; set; }
        public FormState? FormState { get; set; }
        public DateTime? Date { get; set; }
        public int ThesaurusId { get; set; }
        public string Language { get; set; }
        public List<FieldValue> Fields { get; set; }

        [BsonIgnore]
        public List<FormChapter> Chapters { get; set; } = new List<FormChapter>();
        public DocumentProperties.DocumentProperties DocumentProperties { get; set; }
        public List<string> Referrals { get; set; }

        public int UserId { get; set; }
        
        public int OrganizationId { get; set; }

        public int PatientId { get; set; }
        public List<FormInstanceStatus> WorkflowHistory { get; set; }

        public FormInstance() { }
        public FormInstance(Form.Form form)
        {
            form = Ensure.IsNotNull(form, nameof(form));

            this.FormDefinitionId = form.Id.ToString();
            this.Title = form.Title;
            this.Version = form.Version;
            this.EntryDatetime = form.EntryDatetime;
            this.Language = form.Language;
            this.DocumentProperties = form.DocumentProperties;
            this.ThesaurusId = form.ThesaurusId;
            SetLastUpdate();
        }

        public void Copy(FormInstance entity, FormInstanceStatus formInstanceStatus)
        {
            base.Copy(entity);
            this.WorkflowHistory = entity?.WorkflowHistory ?? new List<FormInstanceStatus>();
            this.SetWorkflow(formInstanceStatus);
        }

        public Field GetFieldByThesaurus(int thesaurusId)
        {
            return GetAllFields().Where(x => x.ThesaurusId == thesaurusId).FirstOrDefault();
        }

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

        public List<Field> GetAllFields()
        {
            //problem!
            FormDAL service = new FormDAL();
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
        }

        public void SetValueByThesaurusId(int thesaurusId, string value)
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

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
            foreach (FieldValue field in this.Fields)
            {
                field.ThesaurusId = field.ThesaurusId == oldThesaurus ? newThesaurus : field.ThesaurusId;
            }
        }

        private void SetWorkflow(FormInstanceStatus formInstanceStatus)
        {
            if(formInstanceStatus != null)
            {
                WorkflowHistory.Add(formInstanceStatus);
            }
        }

        public FormInstanceStatus GetCurrentFormInstanceStatus(int? userId, bool isSigned = false)
        {
            FormInstanceStatus formInstanceStatus = null;

            if (userId.HasValue)
            {
                formInstanceStatus = new FormInstanceStatus(FormState.Value, userId.Value, isSigned);
            }

            return formInstanceStatus;
        }
    }
}
