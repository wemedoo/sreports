//using Hl7.Fhir.Model;
//using Hl7.Fhir.Serialization;
//using MongoDB.Driver;
//using sReportsV2.Domain.Entities.Common;
//using sReportsV2.Common.Constants;
//using sReportsV2.Domain.Entities.CustomFHIRClasses;
//using sReportsV2.Domain.Entities.Encounter;
//using sReportsV2.Domain.Entities.EpisodeOfCareEntities;
//using sReportsV2.Domain.Entities.FieldEntity;
//using sReportsV2.Domain.Entities.Form;
//using sReportsV2.Domain.Entities.FormInstance;
//using sReportsV2.Domain.Entities.PatientEntities;
//using sReportsV2.Domain.Entities.ThesaurusEntry;
//using sReportsV2.Common.Enums;
//using sReportsV2.Domain.FormValues;
//using sReportsV2.Domain.Services.Implementations;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using CodeSystem = sReportsV2.Domain.Entities.ThesaurusEntry.CodeSystem;
//using sReportsV2.Common.Entities.User;

//namespace sReportsV2.Initializer.PatientJson
//{
//    public class ImportPatient
//    {
//        public static List<string> listEncounterTypes = new List<string>();
//        public static Dictionary<string, string> encounterClass = new Dictionary<string, string>();
//        public PatientService patientService = new PatientService();
//        public ThesaurusEntryService thesaurusService = new ThesaurusEntryService();
//        public EnumService enumService = new EnumService();
//        public CodeSystemService codingSystemService = new CodeSystemService();
//        public EpisodeOfCareService episodeOfCareService = new EpisodeOfCareService();
//        public EncounterService encounterService = new EncounterService();
//        public EnumService encounterTypeService = new EnumService();
//        public IdentifierInitializer initializer = new IdentifierInitializer();
//        public UserData userData = new UserData();
//        public FormService formService = new FormService();
//        public FormInstanceService formInstanceService = new FormInstanceService();
//        Dictionary<string, string> encounterDictionary = new Dictionary<string, string>();
//        Dictionary<string, string> observationDictionary = new Dictionary<string, string>();
//        Dictionary<string, string> observationUnitDictionary = new Dictionary<string, string>();
//        Dictionary<string, List<CareTeam.ParticipantComponent>> careTeamDictionary = new Dictionary<string, List<CareTeam.ParticipantComponent>>();

//        public ImportPatient()
//        {
//            userData = initializer.CreateUserData();
//        }

//        public List<string> WriteAllDifferentEncounterTypes()
//        {
//            return listEncounterTypes;
//        }

//        public void LoadFromJson(string json, UserData user)
//        {
//            FhirJsonParser parser = new FhirJsonParser();
//            Bundle parsedPatient = parser.Parse<Bundle>(json);
//            string patientId = InsertPatient((Patient)parsedPatient.Entry[0].Resource);
//            string episodeOfCareId = InsertEpisodeOfCare(patientId, parsedPatient.Entry, user);
//            InsertEncounters(parsedPatient.Entry, episodeOfCareId);
//            GetAllObservation(parsedPatient.Entry);
//            GetAllCareTeams(parsedPatient.Entry);
//            InsertForm(parsedPatient.Entry, patientId, episodeOfCareId);
//        }

//        public void InsertForm(List<Bundle.EntryComponent> bundleEntries, string patientId, string episodeOfCareId)
//        {
//            foreach (Bundle.EntryComponent item in bundleEntries)
//            {
//                if (item.Resource.ResourceType == ResourceType.DiagnosticReport)
//                {
//                    var diagnosticReport = (DiagnosticReport)item.Resource;
//                    if (diagnosticReport.Meta.Profile.Contains("http://hl7.org/fhir/us/core/StructureDefinition/us-core-diagnosticreport-lab"))
//                    {
//                        Form form = new Form();
//                        Form existingForm = formService.GetFormByThesaurus(GetThesaurusId(diagnosticReport.Code.Text));
//                        if (existingForm == null)
//                        {
//                            InsertDiagnosticReportLabForm(form, diagnosticReport);
//                            InsertDiagnosticReportLabFormInstance(patientId, episodeOfCareId, form, diagnosticReport);
//                        }
//                        else
//                            InsertDiagnosticReportLabFormInstance(patientId, episodeOfCareId, existingForm, diagnosticReport);
//                    }
//                    else
//                    {
//                        Form form = new Form();
//                        Form existingForm = formService.GetFormByThesaurus(GetThesaurusId(diagnosticReport.Code.Coding.FirstOrDefault().Display));
//                        if (existingForm == null)
//                        {
//                            InsertDiagnosticReportNoteForm(form, diagnosticReport);
//                            InsertDiagnosticReportNoteFormInstance(patientId, episodeOfCareId, form, diagnosticReport);
//                        }
//                        else
//                            InsertDiagnosticReportNoteFormInstance(patientId, episodeOfCareId, existingForm, diagnosticReport);
//                    }
//                }
//                else if (item.Resource.ResourceType == ResourceType.CarePlan) 
//                {
//                    var carePlan = (CarePlan)item.Resource;
//                    Form form = new Form();
//                    Form existingForm = formService.GetFormByThesaurus(GetThesaurusId(carePlan.Category[1].Text));
//                    if (existingForm == null)
//                    {
//                        InsertCarePlanForm(form, carePlan);
//                        InsertCarePlanFormInstance(patientId, episodeOfCareId, form, carePlan);
//                    }
//                    else
//                        InsertCarePlanFormInstance(patientId, episodeOfCareId, existingForm, carePlan);
//                }
//            }
//        }

//        public void InsertCarePlanForm(Form form, CarePlan carePlan)
//        {
//            CreateForm(form, carePlan.Category[1].Text);
//            FormChapter formChapter = CreateChapter();
//            FormPage formPage = CreatePage();
//            List<FieldSet> fieldSets = new List<FieldSet>();
//            List<FieldSet> repetitiveFieldSets = new List<FieldSet>();
//            FieldSet fieldSet = CreateFieldSet();

//            fieldSet.Fields.Add(CreateCarePlanStatusField());
//            fieldSet.Fields.Add(CreateCarePlanIntentField());
//            fieldSet.Fields.Add(CreateDateTimeField("Start period"));

//            foreach (CareTeam.ParticipantComponent participant in careTeamDictionary.Where(x => x.Key == carePlan.CareTeam[0].Reference).FirstOrDefault().Value) 
//                fieldSet.Fields.Add(CreateTextField(participant.Role[0].Text));

//            FieldSet fieldSetActivity = CreateActivityFieldSet();
//            fieldSetActivity.Fields.Add(CreateTextField("Activity Details"));
//            fieldSetActivity.Fields.Add(CreateTextField("Activity Status"));
//            fieldSetActivity.Fields.Add(CreateTextField("Location"));
//            fieldSetActivity.IsRepetitive = true;

//            fieldSets.Add(fieldSet);
//            repetitiveFieldSets.Add(fieldSetActivity);
//            formPage.ListOfFieldSets.Add(fieldSets);
//            formPage.ListOfFieldSets.Add(repetitiveFieldSets);
//            formChapter.Pages.Add(formPage);
//            form.Chapters.Add(formChapter);
//            formService.InsertOrUpdate(form, userData);
//        }

//        public void InsertDiagnosticReportLabForm(Form form, DiagnosticReport diagnosticReport) 
//        {
//            CreateForm(form, diagnosticReport.Code.Text, diagnosticReport.Code.Coding);
//            FormChapter formChapter = CreateChapter();
//            FormPage formPage = CreatePage();
//            List<FieldSet> fieldSets = new List<FieldSet>();
//            FieldSet fieldSet = CreateFieldSet();

//            fieldSet.Fields.Add(CreateSelectField());
//            fieldSet.Fields.Add(CreateTextField("Category"));
//            fieldSet.Fields.Add(CreateTextField("Performer"));
//            fieldSet.Fields.Add(CreateDateTimeField("Effective Date Time"));
//            fieldSet.Fields.Add(CreateDateTimeField("Issued"));
//            int j = 0;
//            foreach (ResourceReference resourceReference in diagnosticReport.Result)
//            {
//                string unit = observationUnitDictionary.Where(x => x.Key == diagnosticReport.Result[j].Reference).FirstOrDefault().Value;
//                if (unit != null)
//                    fieldSet.Fields.Add(CreateNumericField(resourceReference.Display, unit));
//                else
//                    fieldSet.Fields.Add(CreateTextField(resourceReference.Display));

//                j++;
//            }

//            fieldSets.Add(fieldSet);
//            formPage.ListOfFieldSets.Add(fieldSets);
//            formChapter.Pages.Add(formPage);
//            form.Chapters.Add(formChapter);
//            formService.InsertOrUpdate(form, userData);
//        }

//        public void InsertDiagnosticReportNoteForm(Form form, DiagnosticReport diagnosticReport)
//        {
//            CreateForm(form, diagnosticReport.Code.Coding.FirstOrDefault().Display, diagnosticReport.Code.Coding);
//            FormChapter formChapter = CreateChapter();
//            FormPage formPage = CreatePage();
//            List<FieldSet> fieldSets = new List<FieldSet>();
//            FieldSet fieldSet = CreateFieldSet();

//            fieldSet.Fields.Add(CreateSelectField());
//            fieldSet.Fields.Add(CreateTextField("Category"));
//            fieldSet.Fields.Add(CreateTextField("Performer"));
//            fieldSet.Fields.Add(CreateDateTimeField("Effective Date Time"));
//            fieldSet.Fields.Add(CreateDateTimeField("Issued"));
//            fieldSet.Fields.Add(CreateTextareaField("Presented Form"));

//            fieldSets.Add(fieldSet);
//            formPage.ListOfFieldSets.Add(fieldSets);
//            formChapter.Pages.Add(formPage);
//            form.Chapters.Add(formChapter);
//            formService.InsertOrUpdate(form, userData);
//        }

//        public void CreateForm(Form form, string title, [Optional] List<Coding> codings) 
//        {
//            form.Title = title;
//            form.Version = new Domain.Entities.Form.Version();
//            form.Version.Major = 1;
//            form.Version.Minor = 1;
//            form.Chapters = new List<FormChapter>();
//            form.UserId = userData.Id;
//            form.OrganizationId = userData.ActiveOrganization;
//            form.Language = "en";
//            form.DisablePatientData = true;
//            form.ThesaurusId = GetThesaurusId(title, codings);
//        }

//        public FormChapter CreateChapter() 
//        {
//            FormChapter formChapter = new FormChapter();
//            formChapter.Title = "Chapter 1";
//            formChapter.Description = ".";
//            formChapter.ThesaurusId = GetThesaurusId(formChapter.Title);
//            formChapter.Id = Guid.NewGuid().ToString().Replace("-", "");
//            formChapter.Pages = new List<FormPage>();

//            return formChapter;
//        }
//        public FormPage CreatePage()
//        {
//            FormPage formPage = new FormPage();
//            formPage.Title = "Page 1";
//            formPage.Description = ".";
//            formPage.Id = Guid.NewGuid().ToString().Replace("-", "");
//            formPage.ListOfFieldSets = new List<List<FieldSet>>();
//            formPage.ThesaurusId = GetThesaurusId(formPage.Title);

//            return formPage;
//        }

//        public FieldSet CreateFieldSet()
//        {
//            FieldSet fieldSet = new FieldSet();
//            fieldSet.Label = "FieldSet";
//            fieldSet.Description = ".";
//            fieldSet.Id = Guid.NewGuid().ToString().Replace("-", "");
//            fieldSet.Fields = new List<Field>();
//            fieldSet.ThesaurusId = GetThesaurusId(fieldSet.Label);

//            return fieldSet;
//        }

//        public FieldSet CreateActivityFieldSet()
//        {
//            FieldSet fieldSet = new FieldSet();
//            fieldSet.Label = "Care Plan Activity";
//            fieldSet.Description = ".";
//            fieldSet.Id = Guid.NewGuid().ToString().Replace("-", "");
//            fieldSet.Fields = new List<Field>();
//            fieldSet.ThesaurusId = GetThesaurusId(fieldSet.Label);

//            return fieldSet;
//        }

//        public FieldSelect CreateSelectField() 
//        {
//            FieldSelect field = new FieldSelect();
//            field.Label = "Status";
//            field.Description = ".";
//            field.ThesaurusId = GetThesaurusId("Status");
//            field.Id = Guid.NewGuid().ToString().Replace("-", "");
//            field.Value = new List<string>();

//            field.Values.Add(CreateStatusValues("Final"));
//            field.Values.Add(CreateStatusValues("Registered"));
//            field.Values.Add(CreateStatusValues("Partial"));
//            field.Values.Add(CreateStatusValues("Preliminary"));
//            field.Values.Add(CreateStatusValues("Amended"));
//            field.Values.Add(CreateStatusValues("Corrected"));
//            field.Values.Add(CreateStatusValues("Appended"));
//            field.Values.Add(CreateStatusValues("Cancelled"));
//            field.Values.Add(CreateStatusValues("Entered in Error"));
//            field.Values.Add(CreateStatusValues("Unknown"));

//            return field;
//        }

//        public FieldSelect CreateCarePlanStatusField()
//        {
//            FieldSelect field = new FieldSelect();
//            field.Label = "Status";
//            field.Description = ".";
//            field.ThesaurusId = GetThesaurusId("Status");
//            field.Id = Guid.NewGuid().ToString().Replace("-", "");
//            field.Value = new List<string>();

//            field.Values.Add(CreateStatusValues("Draft"));
//            field.Values.Add(CreateStatusValues("Active"));
//            field.Values.Add(CreateStatusValues("On Hold"));
//            field.Values.Add(CreateStatusValues("Revoked"));
//            field.Values.Add(CreateStatusValues("Completed"));
//            field.Values.Add(CreateStatusValues("Entered in Error"));
//            field.Values.Add(CreateStatusValues("Unknown"));

//            return field;
//        }

//        public FieldSelect CreateCarePlanIntentField()
//        {
//            FieldSelect field = new FieldSelect();
//            field.Label = "Intent";
//            field.Description = ".";
//            field.ThesaurusId = GetThesaurusId("Intent");
//            field.Id = Guid.NewGuid().ToString().Replace("-", "");
//            field.Value = new List<string>();

//            field.Values.Add(CreateStatusValues("Proposal"));
//            field.Values.Add(CreateStatusValues("Plan"));
//            field.Values.Add(CreateStatusValues("Order"));
//            field.Values.Add(CreateStatusValues("Option"));

//            return field;
//        }

//        public Field CreateTextField(string label) 
//        {
//            FieldText field = new FieldText();
//            field.Label = label;
//            field.Description = label;
//            field.ThesaurusId = GetThesaurusId(label);
//            field.Id = Guid.NewGuid().ToString().Replace("-", "");
//            field.Value = new List<string>();

//            return field;
//        }

//        public Field CreateNumericField(string label, string unit)
//        {
//            Field field = new FieldNumeric();
//            field.Label = label;
//            field.Description = label;
//            field.Unit = unit;
//            field.ThesaurusId = GetThesaurusId(label);
//            field.Id = Guid.NewGuid().ToString().Replace("-", "");
//            field.Value = new List<string>();

//            return field;
//        }

//        public Field CreateDateTimeField(string label) 
//        {
//            Field field = new FieldDatetime();
//            field.Label = label;
//            field.Description = label;
//            field.ThesaurusId = GetThesaurusId(label);
//            field.Id = Guid.NewGuid().ToString().Replace("-", "");
//            field.Value = new List<string>();

//            return field;
//        }

//        public Field CreateTextareaField(string label) 
//        {
//            Field field = new FieldTextArea();
//            field.Label = label;
//            field.Description = label;
//            field.ThesaurusId = GetThesaurusId(label);
//            field.Id = Guid.NewGuid().ToString().Replace("-", "");
//            field.Value = new List<string>();

//            return field;
//        }

//        public FormFieldValue CreateStatusValues(string label) 
//        {
//            FormFieldValue fieldValue = new FormFieldValue();
//            fieldValue.Label = label;
//            fieldValue.Value = label;
//            fieldValue.ThesaurusId = GetThesaurusId(label);
//            fieldValue.Id = Guid.NewGuid().ToString().Replace("-", "");

//            return fieldValue;
//        }

//        public string GetThesaurusId(string name, [Optional] List<Coding> codings) 
//        {
//            ThesaurusEntry thesaurusEntry = thesaurusService.GetByName(name);
//            string thesaurusId;
//            if (thesaurusEntry == null)
//            {
//                thesaurusEntry = initializer.CreateThesaurusEntry(name);
//                thesaurusService.InsertOrUpdate(thesaurusEntry, userData);
//                thesaurusId = thesaurusEntry.O40MTId;
//            }
//            else
//                thesaurusId = thesaurusEntry.O40MTId;
            
//            if (codings != null)
//            {
//                thesaurusEntry.Codes = new List<O4CodeableConcept>();
//                foreach (Coding coding in codings)
//                    InsertCodableConcept(thesaurusEntry, coding);

//                thesaurusService.InsertOrUpdate(thesaurusEntry, userData);
//            }

//            return thesaurusId;
//        }

//        public void GetAllObservation(List<Bundle.EntryComponent> bundleEntries) 
//        {
//            foreach (Bundle.EntryComponent entryComponent in bundleEntries)
//            {
//                if (entryComponent.Resource.ResourceType == ResourceType.Observation)
//                {
//                    var observation = (Observation)entryComponent.Resource;
//                    if (observation.Value != null)
//                    {
//                        if (observation.Value.Children.Count() > 1)
//                        {
//                            observationDictionary.Add(entryComponent.FullUrl, observation.Value.Children.ToList()[0].ToString());
//                            observationUnitDictionary.Add(entryComponent.FullUrl, observation.Value.Children.ToList()[1].ToString());
//                        }
//                        else
//                            observationDictionary.Add(entryComponent.FullUrl, observation.Value.ToString());
//                    }
//                }
//            }
//        }

//        public void GetAllCareTeams(List<Bundle.EntryComponent> bundleEntries)
//        {
//            foreach (Bundle.EntryComponent entryComponent in bundleEntries)
//            {
//                if (entryComponent.Resource.ResourceType == ResourceType.CareTeam)
//                {
//                    var careTeam = (CareTeam)entryComponent.Resource;
//                    if (careTeam.Participant != null)
//                        careTeamDictionary.Add(entryComponent.FullUrl, careTeam.Participant);
//                }
//            }
//        }

//        public void InsertDiagnosticReportLabFormInstance(string patientId, string episodeOfCareId, Form form, DiagnosticReport diagnosticReport)
//        {
//            FormInstance result = CreateFormInstance(form, patientId, episodeOfCareId, diagnosticReport);
//            int j = 0;
//            for (int i = 0; i < form.Chapters[0].Pages[0].ListOfFieldSets[0][0].Fields.Count(); i++)
//            {
//                FieldValue fieldValue = CreateFormInstanceFieldValue(form, i, 0, 0);
//                if (i == 0)
//                    fieldValue.Value.Add(diagnosticReport.Status.ToString());
//                else if (i == 1)
//                    fieldValue.Value.Add(diagnosticReport.Category[0].Coding[0].Display);
//                else if (i == 2)
//                    fieldValue.Value.Add(diagnosticReport.Performer[0].Display);
//                else if (i == 3)
//                    fieldValue.Value.Add(diagnosticReport.Effective.ToString().Substring(0, 16));
//                else if (i == 4)
//                    fieldValue.Value.Add(diagnosticReport.IssuedElement.ToString().Substring(0, 16));
//                else
//                {
//                    fieldValue.Value.Add(observationDictionary.Where(x => x.Key == diagnosticReport.Result[j].Reference).FirstOrDefault().Value);
//                    j++;
//                }

//                result.Fields.Add(fieldValue);
//            }

//            formInstanceService.InsertOrUpdate(result);
//        }

//        public void InsertCarePlanFormInstance(string patientId, string episodeOfCareId, Form form, CarePlan carePlan)
//        {
//            FormInstance result = CreateFormInstanceCarePlan(form, patientId, episodeOfCareId, carePlan);
//            int j = 0;
//            for (int i = 0; i < form.Chapters[0].Pages[0].ListOfFieldSets[0][0].Fields.Count(); i++)
//            {
//                FieldValue fieldValue = CreateFormInstanceFieldValue(form, i, 0, 0);
//                if (i == 0)
//                    fieldValue.Value.Add(carePlan.Status.ToString());
//                else if (i == 1)
//                    fieldValue.Value.Add(carePlan.Intent.ToString());
//                else if (i == 2)
//                    fieldValue.Value.Add(carePlan.Period.Start.ToString().Substring(0, 16));
//                else
//                {
//                    fieldValue.Value.Add(careTeamDictionary.Where(x => x.Key == carePlan.CareTeam[0].Reference).FirstOrDefault().Value[j].Member.Display);
//                    j++;
//                }

//                result.Fields.Add(fieldValue);
//            }

//            int repetitiveInd = 0;
//            foreach (var it in carePlan.Activity)
//            {
//                FieldValue activitiyDetails = CreateFormInstanceFieldValue(form, 0, 1, repetitiveInd);
//                FieldValue activityStatus = CreateFormInstanceFieldValue(form, 1, 1, repetitiveInd);
//                FieldValue activityLocation = CreateFormInstanceFieldValue(form, 2, 1, repetitiveInd);
//                activitiyDetails.Value.Add(it.Detail.Code.Text);
//                activityStatus.Value.Add(it.Detail.Status.ToString());
//                activityLocation.Value.Add(it.Detail.Location.Display);
//                result.Fields.Add(activitiyDetails);
//                result.Fields.Add(activityStatus);
//                result.Fields.Add(activityLocation);
//                repetitiveInd++;
//            }

//            formInstanceService.InsertOrUpdate(result);
//        }

//        public void InsertDiagnosticReportNoteFormInstance(string patientId, string episodeOfCareId, Form form, DiagnosticReport diagnosticReport)
//        {
//            FormInstance result = CreateFormInstance(form, patientId, episodeOfCareId, diagnosticReport);
//            for (int i = 0; i < form.Chapters[0].Pages[0].ListOfFieldSets[0][0].Fields.Count(); i++)
//            {
//                FieldValue fieldValue = CreateFormInstanceFieldValue(form, i, 0, 0);
//                if (i == 0)
//                    fieldValue.Value.Add(diagnosticReport.Status.ToString());
//                else if (i == 1)
//                    fieldValue.Value.Add(diagnosticReport.Category[0].Coding[0].Display + ", " + diagnosticReport.Category[0].Coding[1].Display);
//                else if (i == 2)
//                    fieldValue.Value.Add(diagnosticReport.Performer[0].Display);
//                else if (i == 3)
//                    fieldValue.Value.Add(diagnosticReport.Effective.ToString().Substring(0, 16));
//                else if (i == 4)
//                    fieldValue.Value.Add(diagnosticReport.IssuedElement.ToString().Substring(0, 16));
//                else if (i == 5)
//                {
//                    if (diagnosticReport.PresentedForm.Count > 0)
//                    {
//                        PresentedForm presentedForm = new PresentedForm()
//                        {
//                            Content = System.Text.Encoding.UTF8.GetString(diagnosticReport.PresentedForm[0].Data)
//                        };
//                        fieldValue.Value.Add(presentedForm.Content);
//                    }
//                }

//                result.Fields.Add(fieldValue);
//            }

//            formInstanceService.InsertOrUpdate(result);

//        }

//        public FormInstance CreateFormInstance(Form form, string patientId, string episodeOfCareId, DiagnosticReport diagnosticReport) 
//        {
//            FormInstance result = new FormInstance(form)
//            {
//                UserId = userData.Id,
//                OrganizationId = userData.ActiveOrganization,
//                PatientId = patientId,
//                EpisodeOfCareRef = episodeOfCareId,
//                Notes = "",
//                Date = DateTime.Now,
//                FormState = FormState.Finished,
//                Chapters = form.Chapters
//            };
//            result.EncounterRef = encounterDictionary.Where(x => x.Key == diagnosticReport.Encounter.Reference).FirstOrDefault().Value;
//            result.Fields = new List<Domain.FormValues.FieldValue>();

//            return result;
//        }

//        public FormInstance CreateFormInstanceCarePlan(Form form, string patientId, string episodeOfCareId, CarePlan carePlan)
//        {
//            FormInstance result = new FormInstance(form)
//            {
//                UserId = userData.Id,
//                OrganizationId = userData.ActiveOrganization,
//                PatientId = patientId,
//                EpisodeOfCareRef = episodeOfCareId,
//                Notes = "",
//                Date = DateTime.Now,
//                FormState = FormState.Finished,
//                Chapters = form.Chapters
//            };
//            result.EncounterRef = encounterDictionary.Where(x => x.Key == carePlan.Encounter.Reference).FirstOrDefault().Value;
//            result.Fields = new List<Domain.FormValues.FieldValue>();

//            return result;
//        }

//        public FieldValue CreateFormInstanceFieldValue(Form form, int fieldPosition, int fieldSetsPosition, int repetitiveInd) 
//        {
//            FieldValue fieldValue = new FieldValue();
//            fieldValue.InstanceId = form.Chapters[0].Pages[0].ListOfFieldSets[fieldSetsPosition][0].Id + "-" + repetitiveInd + "-" + form.Chapters[0].Pages[0].ListOfFieldSets[fieldSetsPosition][0].Fields[fieldPosition].Id + "-1";
//            fieldValue.ThesaurusId = form.Chapters[0].Pages[0].ListOfFieldSets[fieldSetsPosition][0].Fields[fieldPosition].ThesaurusId;
//            fieldValue.Type = form.Chapters[0].Pages[0].ListOfFieldSets[fieldSetsPosition][0].Fields[fieldPosition].Type;
//            fieldValue.Id = form.Chapters[0].Pages[0].ListOfFieldSets[fieldSetsPosition][0].Fields[fieldPosition].Id;
//            fieldValue.Value = new List<string>();

//            return fieldValue;
//        }

//        public string InsertEpisodeOfCare(string patientId, List<Bundle.EntryComponent> bundleEntries, UserData user)
//        {
//            EpisodeOfCareEntity episodeOfCare = new EpisodeOfCareEntity();
//            episodeOfCare.Description = "test";
//            episodeOfCare.Status = EOCStatus.Active;
//            episodeOfCare.Type = "10489";
//            episodeOfCare.DiagnosisRole = "12226";
//            episodeOfCare.OrganizationRef = userData.ActiveOrganization;
//            episodeOfCare.PatientId = patientId;
//            episodeOfCare.Period = new Domain.Entities.Common.Period();
//            Encounter encounter = (Encounter)bundleEntries.Where(x => x.Resource.ResourceType == ResourceType.Encounter).FirstOrDefault().Resource;
//            episodeOfCare.Period.Start = DateTime.Parse(encounter.Period.Start).Date;
//            episodeOfCareService.InsertOrUpdate(episodeOfCare, user);

//            return episodeOfCare.Id;
//        }

//        public void InsertEncounters(List<Bundle.EntryComponent> bundleEntries, string episodeOfCareId)
//        {
//            foreach (Bundle.EntryComponent entryComponent in bundleEntries)
//            {
//                if (entryComponent.Resource.ResourceType == ResourceType.Encounter)
//                {
//                    var encounter = (Encounter)entryComponent.Resource;
//                    EncounterEntity encounterEntity = new EncounterEntity();
//                    encounterEntity.Status = thesaurusService.GetByName(encounter.Status.ToString()).O40MTId;
//                    encounterEntity.Class = SetEncounterClassification(encounter.Class.Code);
//                    encounterEntity.Type = InsertEncounterType(encounter.Type[0].Text);
//                    encounterEntity.ServiceType = "11087";
//                    encounterEntity.Period = new PeriodDatetime();
//                    encounterEntity.Period.Start = DateTime.Parse(encounter.Period.Start.Substring(0, 16));
//                    encounterEntity.Period.End = DateTime.Parse(encounter.Period.End.Substring(0, 16));
//                    encounterEntity.EpisodeOfCareId = episodeOfCareId;

//                    encounterService.Insert(encounterEntity);
//                    encounterDictionary.Add(entryComponent.FullUrl, encounterEntity.Id);
//                }
//            }
//        }

//        public string InsertEncounterType(string name)
//        {
//            string thesaurusId = "";
//            if(!listEncounterTypes.Contains(name))
//                listEncounterTypes.Add(name);

//            ThesaurusEntry thesaurusEntity = thesaurusService.GetByName(name);
//            if (thesaurusEntity != null)
//            {
//                thesaurusId = thesaurusEntity.O40MTId;
//            }
//            else
//            {
//                ThesaurusEntry thesaurusEntry = initializer.CreateThesaurusEntry(name);
//                thesaurusService.InsertOrUpdate(thesaurusEntry, userData);
//                thesaurusId = thesaurusEntry.O40MTId;
//            }
//            if (!encounterTypeService.GetAll().Where(x => x.Type.Equals(PredefinedTypes.EncounterType)).ToList().Exists(x => x.ThesaurusId == thesaurusId))
//            {
//                EnumEntry enumEntry = new EnumEntry();
//                enumEntry.ThesaurusId = thesaurusId;
//                enumEntry.Type = PredefinedTypes.EncounterType;
//                encounterTypeService.Insert(enumEntry, userData.ActiveOrganization);
//            }

//            return thesaurusId;
//        }

//        public string SetEncounterClassification(string name)
//        {
//            string thesaurusId = "";
//            if (encounterClass.ContainsKey(name))
//            {
//                thesaurusId = encounterClass[name];
//            }
//            else
//            {
//                switch (name)
//                {
//                    case "AMB":
//                        thesaurusId = thesaurusService.GetByName("Ambulatory").O40MTId;
//                        break;
//                    case "EMER":
//                        thesaurusId = thesaurusService.GetByName("Emergency").O40MTId;
//                        break;
//                    case "FLD":
//                        thesaurusId = thesaurusService.GetByName("Field").O40MTId;
//                        break;
//                    case "HH":
//                        thesaurusId = thesaurusService.GetByName("Home Health").O40MTId;
//                        break;
//                    case "IMP":
//                        thesaurusId = thesaurusService.GetByName("Inpatient Encounter").O40MTId;
//                        break;
//                    case "ACUTE":
//                        thesaurusId = thesaurusService.GetByName("Inpatient Acute").O40MTId;
//                        break;
//                    case "NONAC":
//                        thesaurusId = thesaurusService.GetByName("Inpatient Non Acute").O40MTId;
//                        break;
//                    case "OBSENC":
//                        thesaurusId = thesaurusService.GetByName("Observation Encounter").O40MTId;
//                        break;
//                    case "PRENC":
//                        thesaurusId = thesaurusService.GetByName("Pre Admission").O40MTId;
//                        break;
//                    case "SS":
//                        thesaurusId = thesaurusService.GetByName("Short Stay").O40MTId;
//                        break;
//                    case "VR":
//                        thesaurusId = thesaurusService.GetByName("Virtual").O40MTId;
//                        break;
//                    default:
//                        return thesaurusId = thesaurusService.GetByName("Not Applicable").O40MTId;
//                }
//                encounterClass.Add(name, thesaurusId);
//            }

//            return thesaurusId;
//        }

//        public string InsertPatient(Patient patient)
//        {
//            PatientEntity patientEntity = new PatientEntity()
//            {
//                Active = true,
//                Name = new Name()
//                {
//                    Given = patient.Name[0].Given.FirstOrDefault(),
//                    Family = patient.Name[0].Family
//                },
//                Addresss = new Domain.Entities.PatientEntities.Address()
//                {
//                    Street = patient.Address[0].Line.FirstOrDefault(),
//                    State = patient.Address[0].State,
//                    Country = patient.Address[0].Country,
//                    City = patient.Address[0].City,
//                    PostalCode = patient.Address[0].PostalCode
//                },
//                Gender = (Gender)patient.Gender,
//                BirthDate = DateTime.Parse(patient.BirthDate)
//            };

//            SetPatientContact(patient, patientEntity);
//            SetPatientIdentifiers(patient, patientEntity);
//            SetPatientTelecoms(patient, patientEntity);
//            SetPatientCommunications(patient, patientEntity);

//            patientService.Insert(patientEntity);

//            return patientEntity.Id;
//        }

//        public void SetPatientContact(Patient patient, PatientEntity patientEntity)
//        {
//            if (patient.Contact.Count > 0)
//            {
//                patientEntity.ContactPerson = new Contact()
//                {
//                    Address = new Domain.Entities.PatientEntities.Address()
//                    {
//                        Street = patient.Contact[0].Address.Line.FirstOrDefault(),
//                        State = patient.Contact[0].Address.State,
//                        Country = patient.Contact[0].Address.Country,
//                        City = patient.Contact[0].Address.City,
//                        PostalCode = patient.Contact[0].Address.PostalCode
//                    },
//                    Name = new Name()
//                    {
//                        Given = patient.Contact[0].Name.Given.FirstOrDefault(),
//                        Family = patient.Contact[0].Name.Family
//                    },
//                    Gender = patient.Contact[0].Gender.ToString(),
//                    Relationship = patient.Contact[0].Relationship[0].Text
//                };

//                patientEntity.ContactPerson.Telecoms = new List<Telecom>();
//                foreach (ContactPoint contactPoint in patient.Contact[0].Telecom)
//                {
//                    Telecom telecom = new Telecom();
//                    telecom.System = contactPoint.System.ToString();
//                    telecom.Use = contactPoint.Use.ToString();
//                    telecom.Value = contactPoint.Value;
//                    patientEntity.ContactPerson.Telecoms.Add(telecom);
//                }
//            }
//        }

//        public void SetPatientIdentifiers(Patient patient, PatientEntity patientEntity)
//        {
//            patientEntity.Identifiers = new List<IdentifierEntity>();
//            foreach (Identifier identifier in patient.Identifier)
//            {
//                IdentifierEntity identifierEntity = new IdentifierEntity();
//                identifierEntity.Use = identifier.Use.ToString();
//                identifierEntity.Value = identifier.Value;
//                if (identifier.Type != null)
//                {
//                    identifierEntity.System = InsertThesaurus(identifier.Type);
//                    identifierEntity.Type = identifier.Type.Text;
//                }
//                else
//                    identifierEntity.System = InsertThesaurus(new CodeableConcept() { Text = "Synthea identifier type" });

//                patientEntity.Identifiers.Add(identifierEntity);
//            }
//        }

//        public void SetPatientTelecoms(Patient patient, PatientEntity patientEntity)
//        {
//            patientEntity.Telecoms = new List<Telecom>();
//            foreach (ContactPoint contact in patient.Telecom)
//            {
//                Telecom telecom = new Telecom();
//                telecom.System = contact.System.ToString();
//                telecom.Use = contact.Use.ToString();
//                telecom.Value = contact.Value;
//                patientEntity.Telecoms.Add(telecom);
//            }
//        }

//        public void SetPatientCommunications(Patient patient, PatientEntity patientEntity)
//        {
//            patientEntity.Communications = new List<Domain.Entities.PatientEntities.Communication>();
//            foreach (Patient.CommunicationComponent communicationComponent in patient.Communication)
//            {
//                Domain.Entities.PatientEntities.Communication communication = new Domain.Entities.PatientEntities.Communication();
//                communication.Language = communicationComponent.Language.Coding[0].Code.Split('-')[0];
//                patientEntity.Communications.Add(communication);
//            }
//            patientEntity.Communications.First(x => x.Preferred == false).Preferred = true;
//        }

//        public string InsertThesaurus(CodeableConcept type) 
//        {
//            ThesaurusEntry thesaurusEntry = null;
//            if (thesaurusService.GetThesaurusCount(type.Text) == 0)
//            {
//                thesaurusEntry = initializer.CreateThesaurusEntry(type.Text);
//                thesaurusEntry.Codes = new List<O4CodeableConcept>();
//                for(int i = 0; i < type.Coding.Count; i++)
//                {
//                    InsertCodableConcept(thesaurusEntry, type.Coding[i]);
//                    InsertCodingSystem(type.Coding[i].System);
//                }
//            }
//            else 
//            {
//                thesaurusEntry = thesaurusService.GetByName(type.Text);
//                for (int i = 0; i < type.Coding.Count; i++) 
//                {
//                    if (!thesaurusEntry.Codes.Exists(x => x.System == type.Coding[i].System))
//                    {
//                        InsertCodableConcept(thesaurusEntry, type.Coding[i]);
//                    }
//                    InsertCodingSystem(type.Coding[i].System);
//                }
//            }
//            thesaurusService.InsertOrUpdate(thesaurusEntry, userData);

//            return InsertIdentifierType(type);
//        }

//        public string InsertIdentifierType(CodeableConcept type)
//        {
//            string thesaurusId = thesaurusService.GetByName(type.Text).O40MTId;
//            if (!enumService.GetAll().Where(x=>x.Type==IdentifierKind.PatientIdentifierType.ToString()).Any(x => !x.IsDeleted && x.ThesaurusId == thesaurusId))
//                initializer.InsertIdentifierType(IdentifierKind.PatientIdentifierType, thesaurusId);

//            return thesaurusId;
//        }

//        public void InsertCodingSystem(string system) 
//        {
//            if (!codingSystemService.GetAll().Exists(x => x.Value == system))
//            {
//                CodeSystem codeSystem = new CodeSystem();
//                codeSystem.Label = system;
//                codeSystem.Value = system;
//                codingSystemService.Insert(codeSystem);
//            }
//        }

//        public void InsertCodableConcept(ThesaurusEntry thesaurusEntry, Coding coding) 
//        {
//            O4CodeableConcept codeableConcept = new O4CodeableConcept();
//            codeableConcept.Code = coding.Code;
//            codeableConcept.System = coding.System;
//            codeableConcept.Value = coding.Display;
//            codeableConcept.VersionPublishDate = DateTime.Now.Date;
//            if (!thesaurusEntry.Codes.Contains(codeableConcept))
//                thesaurusEntry.Codes.Add(codeableConcept);
//        }
//    }
//}
