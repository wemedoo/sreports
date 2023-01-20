using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;
using Newtonsoft.Json;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Data;
using sReportsV2.Domain.FormValues;
using sReportsV2.Domain.Sql.Entities.Common;
using sReportsV2.Domain.Sql.Entities.Patient;
using sReportsV2.Common.Constants;
using sReportsV2.DTOs.CustomEnum.DataOut;

namespace Chapters
{
    public class PdfFormParser
    {
        private PdfDocument pdfDocument;
        private PdfAcroForm pdfAcroForm;
        private readonly Form formJson;
        public Patient Patient { get; set; }
        private readonly string basePath;
        private readonly List<CustomEnum> identifierTypes;

        public List<sReportsV2.Domain.FormValues.FieldValue> Fields = new List<sReportsV2.Domain.FormValues.FieldValue>();
        private readonly List<CustomEnumDataOut> countries;

        public PdfFormParser(string jsonFormPath, string path)
        {
            basePath = path;
            formJson = JsonConvert.DeserializeObject<Form>(File.ReadAllText(jsonFormPath));
        }

        public PdfFormParser(Form form, PdfDocument document, List<CustomEnum> identifierTypes, List<CustomEnumDataOut> countries)
        {
            this.pdfDocument = document;
            this.formJson = form;
            this.identifierTypes = identifierTypes;
            this.countries = countries;
        }

        public Form GetForm()
        {
            return formJson;
        }
        public void AddRepetititveFieldSets(List<FieldSet> list, int count) 
        {
            while (true)
            {
                foreach (Field field in list[0].Fields)
                {
                    field.Value = null;
                }

                if (list.Count >= count)
                {
                    break;
                }

                list.Add(list[0].Clone());
            }
        }

        public Form ReadFieldsFromPdf()
        {
            pdfAcroForm = PdfAcroForm.GetAcroForm(pdfDocument, true);

            var keys = pdfAcroForm.GetFormFields().Select(x => x.Key).ToList();
            foreach (List<FieldSet> list in formJson.GetAllListOfFieldSets()) 
            {
                var allKeysForFieldSetId = keys.Where(x => x.Split('-')[0].Equals(list[0].Id));
                List<string> fieldSetIdentificators = new List<string>();
                foreach (var key in allKeysForFieldSetId) 
                {
                    fieldSetIdentificators.Add(key.Split('-').Last().Split('.')[0]);
                }

               int fieldSetCount =  fieldSetIdentificators.Distinct().ToList().Count();
               AddRepetititveFieldSets(list, fieldSetCount);
            }

            ParseFields();
            ParsePatient();

            if (Patient != null)
            {
                InsertTelecomCheckboxValue(Patient.PatientTelecoms.Select(t => t.System), "TelecomCheckBox");
                InsertTelecomCheckboxValue(Patient.Contact.Telecoms.Select(t => t.System), "ContactTelecomCheckBox");
            }

            formJson.FormState = sReportsV2.Common.Enums.FormState.Finished;
            return formJson;
        }

        public void InsertTelecomCheckboxValue(IEnumerable<string> telecomSystems, string checkboxType)
        {
            string value = string.Empty;
            List<string> values = new List<string>();
            Field formField = formJson.GetAllFields().FirstOrDefault(x => x.FhirType != null && x.FhirType.Equals(checkboxType));

            foreach (var telecomSystem in telecomSystems)
            {
                values.Add(telecomSystem);
            }

            formField.SetValue(formField != null ? string.Join(",", values) : "");

        }

        public string GetFieldId(string key)
        {
            string[] splitedKey = key.Split('-');

            return splitedKey[1];
        }

        public int GetCheckBoxThesaurus(string key)
        {
            string[] splitedKey = key.Split('-');

            return Int32.Parse(splitedKey[4]);
        }

        public string GetLastPartOfKey(string key)
        {
            string[] splitedKey = key.Split('-');

            return splitedKey.Last();
        }

        public void ParseFields()
        {
            foreach (var fieldFromPdf in pdfAcroForm.GetFormFields())
            {
                string key = fieldFromPdf.Key;
                string[] partsOfKey = fieldFromPdf.Key.Split('-');
                if (partsOfKey.Count() > 1)
                {
                    string fieldSetId = partsOfKey[0];
                    List<FieldSet> listFieldSet = formJson.GetListOfFieldSetsByFieldSetId(fieldSetId);
                    int fieldSetPosition = Int32.Parse(partsOfKey.Last().Split('.')[0]);
                    FieldSet fieldSet = listFieldSet[fieldSetPosition];
                    Field field = fieldSet.Fields.FirstOrDefault(x => x.Id.Equals(partsOfKey[1]));
                    field.InstanceId = $"{fieldSet.Id}-{fieldSetPosition}-{field.Id}-1";
                    SetField(field, partsOfKey, fieldFromPdf);

                    RemoveIfExist(Fields, field.InstanceId);
                    Fields.Add(new sReportsV2.Domain.FormValues.FieldValue() { Id = field.Id, ThesaurusId = field.ThesaurusId, InstanceId = field.InstanceId, Type = field.Type, Value = field.Value });
                }
                else 
                {
                    SetNoteAndDate(key, fieldFromPdf);
                }
            }
        }
        public void SetNoteAndDate(string key, KeyValuePair<string, PdfFormField> fieldFromPdf) 
        {
            if (key == "note")
            {
                formJson.Notes = fieldFromPdf.Value.GetValueAsString();
            }
            if (key == "date")
            {
                if (!string.IsNullOrWhiteSpace(fieldFromPdf.Value.GetValueAsString()))
                {
                    formJson.Date = DateTime.Parse(fieldFromPdf.Value.GetValueAsString()).ToUniversalTime();
                }
                else 
                {
                    formJson.Date = DateTime.Now;

                }
            }
        }

        public void RemoveIfExist(List<FieldValue> Fields, string instanceId) 
        {
            FieldValue fieldValue = Fields.FirstOrDefault(x => x.InstanceId == instanceId);
            if (fieldValue != null)
            {
                Fields.Remove(fieldValue);
            }
        }

        public void SetField(Field field, string[] partsOfKey, KeyValuePair<string,PdfFormField> fieldFromPdf) 
        {
            if (field.Type == PdfGeneratorType.Checkbox)
            {
                field.SetValue(GetValueCheckbox(field.Id, ((FieldSelectable)field).Values, partsOfKey.Last().Split('.')[0]));
            }
            else if (field.Type == PdfGeneratorType.Select)
            {
                field.SetValue(((FieldSelectable)field).Values.FirstOrDefault(x => x.Label.Equals(fieldFromPdf.Value.GetValueAsString()))?.Value);
            }
            else if (field.Type == PdfGeneratorType.Radio)
            {
                string value = fieldFromPdf.Value.GetValueAsString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (Int32.TryParse(value, out int thesaurus))
                    {
                        field.SetValue(((FieldSelectable)field).Values.FirstOrDefault(x => x.ThesaurusId.Equals(thesaurus)).ThesaurusId.ToString());
                    }
                }
            }
            else if (field.Type == PdfGeneratorType.Calculative)
            {
                SetCalculativeFieldValue(field);
            }
            else
            {
                field.SetValue(fieldFromPdf.Value.GetValueAsString());
            }
        }

        public void SetCalculativeFieldValue(Field field) 
        {
            var fields = formJson.GetAllFields();
            FieldCalculative fieldCalculative = field as FieldCalculative;
            string formula = fieldCalculative.Formula;
            foreach (string id in fieldCalculative.IdentifiersAndVariables.Keys)
            {
                string value = fields.FirstOrDefault(x => x.Id.Equals(id)).Value?[0];
                formula = formula.Replace($"[{fieldCalculative.IdentifiersAndVariables[id]}]", value);
            }
            
            field.SetValue(ParseFormula(formula));
        }

        public List<string> GetRepetitiveValues(string formFieldId) 
        {
            List<string> result = new List<string>();

            var fields = pdfAcroForm.GetFormFields().Where(x => x.Value.GetValue() != null && GetFieldId(x.Key) == formFieldId);
            if (fields != null) 
            {
                foreach (var field in fields) 
                {
                    var repetitiveValue = field.Value.GetValueAsString();
                    if (!string.IsNullOrWhiteSpace(repetitiveValue)) 
                    {
                        result.Add(repetitiveValue);
                    }
                }
            }

            return result;            
        }

        public List<string> GetRepetitiveValuesForNumber(string formFieldId)
        {
            List<string> result = new List<string>();

            var fields = pdfAcroForm.GetFormFields().Where(x => x.Value.GetValue() != null && GetFieldId(x.Key) == formFieldId);
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    var stringValueOfNumber = field.Value.GetValueAsString();
                    if (!string.IsNullOrWhiteSpace(stringValueOfNumber))
                    {
                        Double.TryParse(stringValueOfNumber, out double doubleValue);

                        if (doubleValue != 0 || stringValueOfNumber.Equals("0"))
                        {
                            result.Add(stringValueOfNumber);
                        }

                    }
                }
            }

            return result;
        }


        public string GetValueNumber(string formFieldId)
        {
            string result = "";
            var field = pdfAcroForm.GetFormFields().FirstOrDefault(x => x.Value.GetValue() != null && GetFieldId(x.Key) == formFieldId);
            if (field.Value != null)
            {
                string stringValueOfNumber = field.Value.GetValueAsString();
                Double.TryParse(stringValueOfNumber, out double doubleValue);

                if (doubleValue != 0 || stringValueOfNumber.Equals("0"))
                {
                    result = stringValueOfNumber;
                }
            }
            return result;
        }

        public string GetValueCheckbox(string formFieldId, List<FormFieldValue> formFieldValues,string fieldSetPosition)
        {
            int checkBoxPosition = 0;
            List<string> valuesArray = new List<string>();
            foreach (var field in pdfAcroForm.GetFormFields())
            {
                if (field.Value.GetValue() != null && field.Key.Split('-').Count() > 1 && GetFieldId(field.Key) == formFieldId && fieldSetPosition.Equals(field.Key.Split('-').Last()))
                {
                    if (field.Value.GetValue().ToString() == "/Yes")
                    {
                        valuesArray.Add(formFieldValues.FirstOrDefault(x => x.ThesaurusId.Equals(GetCheckBoxThesaurus(field.Key))).Value);
                    }
                    checkBoxPosition++;

                }
            }
            return string.Join(",", valuesArray);
        }
        public string GetValue(string formFieldId)
        {
            string result = "";
            
            var field = pdfAcroForm.GetFormFields().FirstOrDefault(x => x.Value.GetValue() != null && GetFieldId(x.Key) == formFieldId);
            if (field.Value != null)
            {
                string fieldId = GetFieldId(field.Key);
                if (field.Key.Split('-')[0] == "Button")
                {
                    FieldSelectable formField = (FieldSelectable) formJson.GetField(fieldId);

                    string thesaurus = field.Value.GetValueAsString();
                    if (!string.IsNullOrWhiteSpace(thesaurus))
                    {
                        result = formField.Values.FirstOrDefault(x => x.ThesaurusId.ToString().Equals(thesaurus)).ThesaurusId.ToString();
                    }

                }
               
                else
                {
                    result = field.Value.GetValueAsString();
                }
               
            }
            return result;
        }

        private string ParseFormula(string formula)
        {
            try
            {
                object calculatedResult = new DataTable().Compute(formula, "");
                return Math.Round(Convert.ToDouble(calculatedResult), 4).ToString();
            }
            catch (Exception)
            {
                return "";
            }
        }

        private void ParsePatient()
        {
            PatientParser patientParser = new PatientParser(identifierTypes, countries);
            Patient = patientParser.ParsePatientChapter(formJson.Chapters.FirstOrDefault(x => x.ThesaurusId.ToString().Equals(ResourceTypes.PatientThesaurus)));
        }
    }
}


