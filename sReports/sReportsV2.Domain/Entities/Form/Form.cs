using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Common.Enums;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using sReportsV2.Common.Constants;
using sReportsV2.Common.Extensions;
using sReportsV2.Common.Entities.User;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class Form : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public FormAbout About { get; set; }
        public string Title { get; set; }
        public Version Version { get; set; }
        public List<FormChapter> Chapters { get; set; } = new List<FormChapter>();
        public FormDefinitionState State { get; set; }
        public string Language { get; set; }

        [BsonRepresentation(BsonType.Int32, AllowTruncation = true)]
        public int ThesaurusId { get; set; }
        public string Notes { get; set; }
        public FormState? FormState { get; set; }
        public DateTime? Date { get; set; }
        public DocumentProperties.DocumentProperties DocumentProperties { get; set; }
        public List<FormStatus> WorkflowHistory { get; set; }
        public FormEpisodeOfCare EpisodeOfCare { get; set; }
        public bool DisablePatientData { get; set; }
        public string Description { get; set; }
        public int DocumentsCount { get; set; }
        public int UserId { get; set; }
        public int OrganizationId { get; set; }
        public Form() 
        {
            WorkflowHistory = new List<FormStatus>();
        }
        public Form(FormInstance.FormInstance formValue, Form form)
        {
            formValue = Ensure.IsNotNull(formValue, nameof(formValue));
            form = Ensure.IsNotNull(form, nameof(form));

            this.Id = formValue.FormDefinitionId;
            this.About = form.About;
            this.Chapters = form.Chapters;
            this.Title = formValue.Title;
            this.Version = formValue.Version;
            this.Language = formValue.Language;
            this.ThesaurusId = form.ThesaurusId;
            this.DocumentProperties = formValue.DocumentProperties;
            this.Notes = formValue.Notes;
            this.Date = formValue.Date;
            this.FormState = formValue.FormState;
            this.UserId = formValue.UserId;
            this.OrganizationId = formValue.OrganizationId;
            this.SetFields(formValue.Fields);
        }

        public Field GetFieldById(string id)
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(listOfFS => listOfFS
                                    .SelectMany(set => set.Fields
                                )
                            )
                         )
                        ).FirstOrDefault(x => x.Id == id);
        }

        public List<Field> GetAllFields()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(listOfFS => listOfFS
                                    .SelectMany(set => set.Fields
                                )
                            )
                         )
                        ).ToList();
        }
        public List<Field> GetAllFieldsFromListOfFieldSets()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(list => list.SelectMany(set => set.Fields)
                                )
                            )
                        ).ToList();
        }

        public List<Field> GetAllFieldsFromNonRepetititveFieldSets()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets.Where(x => x[0].IsRepetitive == false)
                                .SelectMany(list => list.SelectMany(set => set.Fields)
                                )
                            )
                        ).ToList();
        }
        public List<Field> GetAllNonPatientFields()
        {
            return this.Chapters.Where(x => !x.ThesaurusId.Equals("9356"))
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(listOfFS => listOfFS
                                    .SelectMany(set => set.Fields
                                    )
                                )
                           )
                        ).ToList();
        }

        public List<FieldSet> GetAllFieldSets()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(listOfFS => listOfFS)
                            )
                        ).ToList();
        }

        public List<FieldSet> GetListOfFieldSetsByFieldSetId(string fsId)
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                            )).FirstOrDefault(x => x.Contains(this.GetAllFieldSetsByLists().FirstOrDefault(y => y.Id == fsId)));

        }
        public List<FieldSet> GetAllFieldSetsByLists()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(list => list)
                            )
                        ).ToList();
        }

        public Field GetField(string id)
        {
            return GetAllFields().Where(field => field.Id != null && field.Id.Equals(id)).FirstOrDefault();
        }

        public List<int> GetAllThesaurusIds()
        {
            List<int> thesaurusList = new List<int>
            {
                this.ThesaurusId
            };
            foreach (FormChapter formChapter in this.Chapters)
            {
                thesaurusList.Add(formChapter.ThesaurusId);
                thesaurusList.AddRange(formChapter.GetAllThesaurusIds());
            }

            return thesaurusList;
        }

        public void GenerateTranslation(List<sReportsV2.Domain.Sql.Entities.ThesaurusEntry.ThesaurusEntry> entries, string language)
        {
            string activeLanguage = this.Language;
            this.Language = language;
            this.Title = entries.FirstOrDefault(x => x.Id.Equals(ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);            

            foreach (FormChapter formChapter in this.Chapters)
            {
                formChapter.Title = entries.FirstOrDefault(x => x.Id.Equals(formChapter.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
                formChapter.Description = entries.FirstOrDefault(x => x.Id.Equals(formChapter.ThesaurusId))?.GetDefinitionByTranslationOrDefault(language, activeLanguage);
                formChapter.GenerateTranslation(entries, language, activeLanguage);
            }
        }

        public void SetFieldValue(string fieldId, string value)
        {
            Field field = this.GetAllFields().FirstOrDefault(x => x.Id.Equals(fieldId));
            if (field != null)
            {
                field.SetValue(value);
            }
        }

        public List<List<FieldSet>> GetFieldSetsByRepetitivity(bool isRepetitive) 
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                            )
                        ).Where(list => list[0].IsRepetitive == isRepetitive).ToList();

        }

       
        public void SetValuesFromReferrals(List<Form> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));            
            this.SetMatchedFieldSets(formInstances);           
            List<Field> allReferralsFields = formInstances.SelectMany(x => x.GetAllFieldsFromNonRepetititveFieldSets()).ToList();           
            SetFieldsFromNonRepetitiveFieldSets(allReferralsFields);
        }

        public void SetMatchedFieldSets(List<Form> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));
            List<List<FieldSet>> referralsRepetiveFieldSets = formInstances.SelectMany(x => x.GetFieldSetsByRepetitivity(true)).ToList();           
            foreach (List<FieldSet> formFieldSet in this.GetFieldSetsByRepetitivity(true))
            {
                foreach (List<FieldSet> referralFieldSet in referralsRepetiveFieldSets)
                {
                    if (formFieldSet[0].IsReferable(referralFieldSet[0]))
                    {
                        SetInstanceId(referralFieldSet, formFieldSet);
                        formFieldSet.RemoveAt(0);
                        formFieldSet.AddRange(referralFieldSet);
                        break;
                    }
                }
            }
        }

        private void SetInstanceId(List<FieldSet> referralFieldSet, List<FieldSet> formFieldSet) 
        {
            foreach (FieldSet item in referralFieldSet)
            {
                item.Id = formFieldSet[0].Id;
            }
        }

        public void SetFieldsFromNonRepetitiveFieldSets(List<Field> allReferralsFields) 
        {
            foreach (Field field in this.GetAllFieldsFromNonRepetititveFieldSets())
            {
                Field referralField = allReferralsFields.FirstOrDefault(x => x.ThesaurusId == field.ThesaurusId && x.Type == field.Type);
                if (referralField != null && referralField.Value != null)
                {
                    field.Value = referralField.Value;
                };
            }
        }

        public List<ReferalInfo> GetReferalInfoFromRepetitiveFieldSets(List<Form> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));

            List<ReferalInfo> result = new List<ReferalInfo>();
            List<int> thesaurusesAdded = new List<int>();

            foreach (Form instance in formInstances)
            {
                ReferalInfo referalInfo = new ReferalInfo
                {
                    Id = instance.Id,
                    VersionId = instance.Version.Id,
                    Title = instance.Title,
                    ThesaurusId = instance.ThesaurusId,
                    LastUpdate = instance.LastUpdate,
                    UserId = instance.UserId,
                    OrganizationId = instance.OrganizationId,
                    ReferrableFields = new List<KeyValue>()
                };

                foreach (List<FieldSet> formFieldSet in this.GetFieldSetsByRepetitivity(true))
                {
                    foreach (List<FieldSet> referralFieldSet in instance.GetFieldSetsByRepetitivity(true))
                    {
                        if (formFieldSet[0].IsReferable(referralFieldSet[0]))
                        {
                            if (!thesaurusesAdded.Contains(formFieldSet[0].ThesaurusId)) 
                            {
                                thesaurusesAdded.Add(formFieldSet[0].ThesaurusId);
                                AddReferrableFieldSet(referalInfo, referralFieldSet);
                            }
                        }
                    }
                }

                result.Add(referalInfo);
            }

            return result;
        }

        public List<ReferalInfo> GetReferalInfoFromNonRepetitiveFieldSets(List<Form> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));
            List<ReferalInfo> result = new List<ReferalInfo>();
            List<ReferralForm> allReferrals = formInstances
                .Select(x => new ReferralForm
                { 
                    Id = x.Id,
                    Title = x.Title,
                    VersionId = x.Version.Id,
                    ThesaurusId = x.ThesaurusId,
                    LastUpdate = x.LastUpdate,
                    UserId = x.UserId,
                    OrganizationId = x.OrganizationId,
                    Fields = x.GetAllFieldsFromNonRepetititveFieldSets() 
                })
                .ToList();


            foreach (ReferralForm referral in allReferrals)
            {
                ReferalInfo referalInfo = new ReferalInfo
                {
                    Id = referral.Id,
                    VersionId = referral.VersionId,
                    Title = referral.Title,
                    ThesaurusId = referral.ThesaurusId,
                    LastUpdate = referral.LastUpdate,
                    UserId = referral.UserId,
                    OrganizationId = referral.OrganizationId,
                    ReferrableFields = new List<KeyValue>()
                };

                foreach (Field field in this.GetAllFieldsFromNonRepetititveFieldSets())
                {
                    Field referralField = referral.Fields.FirstOrDefault(x => x.ThesaurusId == field.ThesaurusId && x.Type == field.Type);

                    if (referralField != null/* && !addedThesauruses.Contains(referralField.ThesaurusId)*/)
                    {
                        SetReferralInfo(referalInfo, field, referralField);                      
                    }
                }
                result.Add(referalInfo);
            }

            return result;
        }

        private void SetReferralInfo(ReferalInfo referalInfo, Field field, Field referralField) 
        {
            if (referralField is FieldString @string && @string.IsRepetitive && referralField.Value != null && referralField.Value.Count > 0)
            {
                referalInfo.ReferrableFields.Add(new KeyValue 
                { 
                    Key = referralField.Label, 
                    Value = @string.GetReferrableRepetitiveValue(), 
                    ThesaurusId = referralField.ThesaurusId 
                });

            }

            if (IsFieldStringAndRepetitiveAndHasValue(referralField) || IsFieldSelectableAndHasValue(referralField))
            {
                referalInfo.ReferrableFields.Add(new KeyValue 
                { 
                    Key = referralField.Label, 
                    Value = field.GetReferrableValue(referralField.Value[0]), 
                    ThesaurusId = referralField.ThesaurusId 
                });
            }
        }

        private bool IsFieldStringAndRepetitiveAndHasValue(Field referralField) 
        {
            return referralField is FieldString @string
                && !@string.IsRepetitive 
                && referralField.HasValue();
        }

        private bool IsFieldSelectableAndHasValue(Field referralField) 
        {
            return referralField is FieldSelectable && referralField.HasValue();
        }

        public void AddReferrableFieldSet(ReferalInfo referalInfo, List<FieldSet> referralFieldSet) 
        {
            Ensure.IsNotNull(referalInfo, nameof(referalInfo));
            Ensure.IsNotNull(referralFieldSet, nameof(referralFieldSet));

            for (int i = 0; i < referralFieldSet.Count; i++)
            {
                foreach (Field referralField in referralFieldSet[i].Fields)
                {
                    SetReferralFieldIntoReferralInfo(referalInfo, referralField, i);
                }
            }
        }

        public void SetReferralFieldIntoReferralInfo(ReferalInfo referalInfo, Field referralField, int repetitionPosition) 
        {
            Ensure.IsNotNull(referralField, nameof(referralField));
            if (referralField is FieldString @string && @string.IsRepetitive)
            {
                SetRepetitiveFieldValueIntoReferralInfo(referalInfo, referralField, repetitionPosition);
            }
            else
            {
                SetNonRepetitiveFieldValueIntoReferralInfo(referalInfo, referralField, repetitionPosition);
            }
        }

        public void SetRepetitiveFieldValueIntoReferralInfo(ReferalInfo referalInfo, Field referralField, int repetitionPosition) 
        {
            Ensure.IsNotNull(referalInfo, nameof(referalInfo));
            Ensure.IsNotNull(referralField, nameof(referralField));

            if (referralField.Value != null && referralField.Value.Count > 0)
            {
                referalInfo.ReferrableFields.Add(new KeyValue 
                { 
                    Key = $"{referralField.Label}({repetitionPosition})", 
                    Value = ((FieldString)referralField).GetReferrableRepetitiveValue(), 
                    ThesaurusId = referralField.ThesaurusId 
                });
            };
        }

        public void SetNonRepetitiveFieldValueIntoReferralInfo(ReferalInfo referalInfo, Field referralField, int repetitionPosition)
        {
            Ensure.IsNotNull(referalInfo, nameof(referalInfo));
            Ensure.IsNotNull(referralField, nameof(referralField));

            if (referralField.HasValue())
            {
                referalInfo.ReferrableFields.Add(new KeyValue 
                { 
                    Key = $"{referralField.Label}({repetitionPosition})", 
                    Value = referralField.GetReferrableValue(referralField.Value[0]), 
                    ThesaurusId = referralField.ThesaurusId 
                });
            };
        }

        public List<ReferalInfo> GetValuesFromReferrals(List<Form> formInstances)
        {
            List<ReferalInfo> result = new List<ReferalInfo>();

            result.AddRange(this.GetReferalInfoFromRepetitiveFieldSets(formInstances));
            result.AddRange(this.GetReferalInfoFromNonRepetitiveFieldSets(formInstances));

            return result;
        }

        public void SetWorkflow(UserData user, FormDefinitionState state)
        {
            user = Ensure.IsNotNull(user, nameof(user));

            if (WorkflowHistory == null)
            {
                WorkflowHistory = new List<FormStatus>();
            }

            WorkflowHistory.Add(new FormStatus()
            {
                Created = DateTime.Now,
                Status = state,
                UserId = user.Id
            });
        }

        public bool IsVersionChanged(Form formFromDatabase) 
        {
            Ensure.IsNotNull(formFromDatabase, nameof(formFromDatabase));

            return this.Version.Major != formFromDatabase.Version.Major || this.Version.Minor != formFromDatabase.Version.Minor;
        }

        public void SetFields(List<FieldValue> fields)
        {

            fields = Ensure.IsNotNull(fields, nameof(fields));
            List<string> keys = fields.Select(x => x.InstanceId).ToList();
            foreach (var list in this.GetAllListOfFieldSets())
            {
                List<string> fieldSetKeys = keys.Where(x => x.StartsWith($"{list[0].Id}-")).ToList();
                List<string> distinctedList = GetDistinctedListOfFieldSetsKeys(fieldSetKeys);
                for (int i = 1; i < distinctedList.Count; i++)
                {
                    list.Add(list[0].Clone());
                }

                foreach (string key in fieldSetKeys)
                {
                    if (key != null) 
                    {
                        int position = Int32.Parse(key.Split('-')[1]);
                        string fieldId = key.Split('-')[2];
                        list[position].Fields.FirstOrDefault(x => x.Id == fieldId).Value = fields.FirstOrDefault(x => x.InstanceId == key).Value;
                    }                   
                }
            }
        }
        private List<string> GetDistinctedListOfFieldSetsKeys(List<string> allKeysForFieldSetId)
        {
            return allKeysForFieldSetId
                .Select(x => x.Split('-')[1])
                .Distinct()
                .ToList();
        }

        public List<List<FieldSet>> GetAllListOfFieldSets()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                            )
                        )
                        .ToList();
        }

        public void SetFieldsValuesFromFormValue(FormInstance.FormInstance formInstance)
        {
            formInstance = Ensure.IsNotNull(formInstance, nameof(formInstance));
            List<Field> allFields = GetAllFields();
            foreach(FieldValue fieldValue in formInstance.Fields)
            {
                Field field = allFields.FirstOrDefault(x => x.Id.Equals(fieldValue.Id));
                if(field != null)
                {
                    field.Value = fieldValue.Value;
                }
            }
        }

        public List<Field> GetAllFieldsWhichAreDependable()
        {
            IEnumerable<Field> fields = GetAllFields();
            IEnumerable<FormFieldDependable> dependables = fields.OfType<FieldSelectable>().SelectMany(x => x.Dependables);
            return fields
                .Where(x => dependables.Select(y => y.ActionParams).Contains(x.Id))
                .ToList();
        }

        public List<Field> GetAllObservation()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(fieldSet => fieldSet
                                    .SelectMany(set => set.Fields.Where(y => y.FhirType != null).Where(x => x.FhirType.Equals(ResourceTypes.Observation))
                                    )
                                )
                            )
                        ).ToList();
        }
        public List<Field> GetAllProcedure()
        {
            return this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(fieldSet => fieldSet
                                    .SelectMany(set => set.Fields.Where(y => y.FhirType != null).Where(x => x.FhirType.Equals(ResourceTypes.Procedure))
                                    )
                                )
                            )
                        ).ToList();
        }

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus) 
        {
            this.ThesaurusId = this.ThesaurusId == oldThesaurus ? newThesaurus : this.ThesaurusId;
            foreach (FormChapter chapter in this.Chapters) 
            {
                chapter.ReplaceThesauruses(oldThesaurus, newThesaurus);
            }
        }
    }
}
