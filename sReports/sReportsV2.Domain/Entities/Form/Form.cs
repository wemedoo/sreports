using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Enums;
using sReportsV2.Domain.Extensions;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserRef { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string OrganizationRef { get; set; }
        public List<FormChapter> Chapters { get; set; } = new List<FormChapter>();
        public FormDefinitionState State { get; set; }
        public string Language { get; set; }
        public string ThesaurusId { get; set; }
        public string Notes { get; set; }
        public FormState? FormState { get; set; }
        public DateTime? Date { get; set; }
        public DocumentProperties.DocumentProperties DocumentProperties { get; set; }
        public List<FormStatus> WorkflowHistory { get; set; }
        public FormEpisodeOfCare EpisodeOfCare { get; set; }
        public bool DisablePatientData { get; set; }
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
            this.SetFields(formValue.Fields);
        }

        public void UpdateWorkflowHistory(UserData userData, FormDefinitionState state)
        {
            userData = Ensure.IsNotNull(userData, nameof(userData));

            WorkflowHistory.Add(new FormStatus()
            {
                Created = DateTime.Now,
                Status = state,
                User = userData.Id
            });
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

        public List<string> GetAllThesaurusIds()
        {
            List<string> thesaurusList = new List<string>();
            thesaurusList.Add(this.ThesaurusId);
            foreach (FormChapter formChapter in this.Chapters)
            {
                var chapterThesaurusId = formChapter.ThesaurusId;
                thesaurusList.Add(chapterThesaurusId);
                thesaurusList.AddRange(formChapter.GetAllThesaurusIds());
            }

            return thesaurusList;
        }

        public void GenerateTranslation(List<ThesaurusEntry.ThesaurusEntry> entries, string language)
        {
            string activeLanguage = this.Language;
            this.Language = language;
            this.Title = entries.FirstOrDefault(x => x.O40MTId.Equals(ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);            

            foreach (FormChapter formChapter in this.Chapters)
            {
                formChapter.Title = entries.FirstOrDefault(x => x.O40MTId.Equals(formChapter.ThesaurusId))?.GetPreferredTermByTranslationOrDefault(language, activeLanguage);
                formChapter.Description = entries.FirstOrDefault(x => x.O40MTId.Equals(formChapter.ThesaurusId))?.GetDefinitionByTranslationOrDefault(language, activeLanguage);
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
           
            List<Field> allReferralsFields = new List<Field>();
            foreach (Form formInstance in formInstances)
            {
                allReferralsFields = allReferralsFields.Concat(formInstance.GetAllFieldsFromNonRepetititveFieldSets()).ToList();
            }

            this.SetFieldsFromNonRepetitiveFieldSets(allReferralsFields);
        }

        public void SetMatchedFieldSets(List<Form> formInstances)
        {
            formInstances = Ensure.IsNotNull(formInstances, nameof(formInstances));

            List<List<FieldSet>> referralsRepetiveFieldSets = new List<List<FieldSet>>();
            //var test = formInstances.SelectMany(x => x.GetFieldSetsByRepetitivity(true)).ToList();
            foreach (Form instance in formInstances)
            {
                referralsRepetiveFieldSets.AddRange(instance.GetFieldSetsByRepetitivity(true));
            }

            foreach (List<FieldSet> formFieldSet in this.GetFieldSetsByRepetitivity(true))
            {
                foreach (List<FieldSet> referralFieldSet in referralsRepetiveFieldSets)
                {
                    if (formFieldSet[0].IsReferable(referralFieldSet[0]))
                    {
                        formFieldSet.RemoveAt(0);
                        formFieldSet.AddRange(referralFieldSet);
                        break;
                    }
                }
            }
        }

        public void SetFieldsFromNonRepetitiveFieldSets(List<Field> allReferralsFields) 
        {
            foreach (Field field in this.GetAllFieldsFromNonRepetititveFieldSets())
            {
                Field referralField = allReferralsFields.FirstOrDefault(x => x.ThesaurusId == field.ThesaurusId);
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
            List<string> thesaurusesAdded = new List<string>();

            foreach (Form instance in formInstances)
            {
                ReferalInfo referalInfo = new ReferalInfo { Title = instance.Title, ReferrableFields = new List<KeyValue>() };

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

            List<ReferralForm> allReferrals = formInstances.Select(x => new ReferralForm{ Title = x.Title, Fields = x.GetAllFieldsFromNonRepetititveFieldSets() }).ToList();

            List<string> addedThesauruses = new List<string>();

            foreach (ReferralForm referral in allReferrals)
            {
                ReferalInfo referalInfo = new ReferalInfo { Title = referral.Title, ReferrableFields = new List<KeyValue>() };
                foreach (Field field in this.GetAllFieldsFromNonRepetititveFieldSets())
                {
                    Field referralField = referral.Fields.FirstOrDefault(x => x.ThesaurusId == field.ThesaurusId);

                    if (referralField != null && !addedThesauruses.Contains(referralField.ThesaurusId))
                    {
                        SetReferralInfo(referalInfo, field, referralField, addedThesauruses);                      
                    }
                }

                result.Add(referalInfo);
            }

            return result;
        }

        private void SetReferralInfo(ReferalInfo referalInfo, Field field, Field referralField, List<string> addedThesauruses) 
        {
            if (referralField is FieldString && ((FieldString)referralField).IsRepetitive && referralField.Value != null && referralField.Value.Count > 0)
            {
                referalInfo.ReferrableFields.Add(new KeyValue { Key = referralField.Label, Value = ((FieldString)referralField).GetReferrableRepetitiveValue() });
                addedThesauruses.Add(referralField.ThesaurusId);
            }

            if (IsFieldStringAndRepetitiveAndHasValue(referralField) || IsFieldSelectableAndHasValue(referralField))
            {
                referalInfo.ReferrableFields.Add(new KeyValue { Key = referralField.Label, Value = field.GetReferrableValue(referralField.Value[0]) });
                addedThesauruses.Add(referralField.ThesaurusId);
            }
        }

        private bool IsFieldStringAndRepetitiveAndHasValue(Field referralField) 
        {
            return referralField is FieldString && !((FieldString)referralField).IsRepetitive && referralField.Value != null && referralField.Value.Count > 0 && !string.IsNullOrWhiteSpace(referralField.Value?[0].Replace(",", " "));
        }

        private bool IsFieldSelectableAndHasValue(Field referralField) 
        {
            return referralField is FieldSelectable && referralField.Value != null && referralField.Value.Count > 0 && !string.IsNullOrWhiteSpace(referralField.Value?[0].Replace(",", " "));
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
            if (referralField is FieldString && ((FieldString)referralField).IsRepetitive)
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
                referalInfo.ReferrableFields.Add(new KeyValue { Key = $"{referralField.Label}({repetitionPosition})", Value = ((FieldString)referralField).GetReferrableRepetitiveValue() });
            };
        }

        public void SetNonRepetitiveFieldValueIntoReferralInfo(ReferalInfo referalInfo, Field referralField, int repetitionPosition)
        {
            Ensure.IsNotNull(referalInfo, nameof(referalInfo));
            Ensure.IsNotNull(referralField, nameof(referralField));

            if (!string.IsNullOrWhiteSpace(referralField.Value?[0].Replace(",", " ")))
            {
                referalInfo.ReferrableFields.Add(new KeyValue { Key = $"{referralField.Label}({repetitionPosition})", Value = referralField.GetReferrableValue(referralField.Value[0]) });
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
            this.UpdateWorkflowHistory(user, state);
        }

        public bool IsVersionChanged(Form formFromDatabase) 
        {
            Ensure.IsNotNull(formFromDatabase, nameof(formFromDatabase));

            return this.Version.Major != formFromDatabase.Version.Major || this.Version.Minor != formFromDatabase.Version.Minor;
        }

        public void SetFields(List<FieldValue> fields)
        {
            
            List<string> keys = fields.Select(x => x.InstanceId).ToList();
            fields = Ensure.IsNotNull(fields, nameof(fields));

            foreach (var list in this.GetAllListOfFieldSets())
            {
                List<string> fieldSetKeys = keys.Where(x => x.StartsWith($"{list[0].Id}-")).ToList();
                List<string> distinctedList = GetDistinctedList(fieldSetKeys);
                for (int i = 1; i < distinctedList.Count; i++)
                {
                    list.Add(list[0].Clone());
                }

                foreach (string key in fieldSetKeys)
                {
                    int position = Int32.Parse(key.Split('-')[1]);
                    string fieldId = key.Split('-')[2];
                    Field field = list[position].Fields.FirstOrDefault(x => x.Id == fieldId);
                    if(field != null)
                    {
                        field.Value = fields.FirstOrDefault(x => x.InstanceId == key).Value;
                    }
                }
            }
        }
        private List<string> GetDistinctedList(List<string> allKeysForFieldSetId)
        {
            List<string> listForDistinct = new List<string>();

            foreach (string value in allKeysForFieldSetId)
            {
                listForDistinct.Add(value.Split('-')[1]);
            }

            return listForDistinct.Distinct().ToList();
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

        public List<string> GetAllThesauruses()
        {
            List<string> fields = this.GetAllFields().Select(x => x.ThesaurusId).ToList();

            List<FormFieldValue> values = this.Chapters
                        .SelectMany(chapter => chapter.Pages
                            .SelectMany(page => page.ListOfFieldSets
                                .SelectMany(fieldSet => fieldSet
                                    .SelectMany(set => set.Fields.OfType<FieldSelectable>()
                                        .SelectMany(field => field.Values)
                                    )
                                )
                            )
                        ).ToList();

            List<string> valuesThesauruses = values.Select(x => x.ThesaurusId).ToList();

            fields.AddRange(valuesThesauruses);
            return fields;
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


    }
}
