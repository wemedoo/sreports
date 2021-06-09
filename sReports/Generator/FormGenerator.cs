using Microsoft.VisualBasic.FileIO;
using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.Common.Enums;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Common.Entities.User;

namespace Generator
{
    public class FormGenerator : ThesaurusCommon
    {
        public ThesaurusDAL thesaurusDAL = new ThesaurusDAL(new SReportsContext());
        public FormDAL formService = new FormDAL();
        public UserData userData;
        int id = 0;
        string[] classIndicators;


        public FormGenerator() { }

        public FormGenerator(UserData user) 
        {
            userData = user;
        }

        public Form GetFormFromCsv(string formName)
        {
            List<string[]> csvRows = GetRowsFromCsv();

            Form form = new Form();
            form.State = FormDefinitionState.ReadyForDataCapture;
            form.Version = new sReportsV2.Domain.Entities.Form.Version() { Major = 1, Minor = 1, Id = Guid.NewGuid().ToString() };
            form.Language = "en";
            form.Title = formName;
            form.UserId = userData.Id;
            form.OrganizationId = userData.ActiveOrganization.GetValueOrDefault();
            form.ThesaurusId = GetNewThesaurus(formName);
            form.Chapters = new List<FormChapter>();

            FormChapter chapter = new FormChapter();
            chapter.Id = GetNewId();
            chapter.ThesaurusId = GetNewThesaurus(formName);
            chapter.Title = formName;
            chapter.Pages = new List<FormPage>();

            FormPage page = new FormPage();
            page.Id = GetNewId();
            page.ThesaurusId = GetNewThesaurus(".");
            page.ListOfFieldSets = new List<List<FieldSet>>();

            FieldSet fieldSet = new FieldSet();
            fieldSet.Id = GetNewId();
            fieldSet.ThesaurusId = GetNewThesaurus(".");
            fieldSet.Fields = new List<Field>();

            foreach (string[] row in csvRows)
            {
                FieldSelectable field = new FieldSelectable();
                field.Id = GetNewId();
                field.ThesaurusId = GetNewThesaurus(row[0]);
                field.Type = "radio";
                field.FhirType = "Observation";
                field.Label = row[0];
                field.Values = new List<FormFieldValue>();

                int optionCounter = 0;

                SetFieldValues(field, ref optionCounter, row);
                SetType(field, optionCounter);

                fieldSet.Fields.Add(field);
            }

            page.ListOfFieldSets.Add(new List<FieldSet>() { fieldSet });
            chapter.Pages.Add(page);
            form.Chapters.Add(chapter);

            return form;
        }

        private void SetType(Field field, int optionCounter) 
        {
            if (optionCounter == 1)
            {
                field.Type = "checkbox";
            }
        }

        private void SetFieldValues(FieldSelectable field, ref int optionCounter, string[] row) 
        {
            for (int i = 1; i < row.Length; i++)
            {
                if (row[i] != "-")
                {
                    optionCounter++;
                    string concatenatedValue = $"{classIndicators[i]}-{row[i].Replace("\n", " ")}";
                    int thesaurusId = GetNewThesaurus(concatenatedValue);
                    FormFieldValue value = new FormFieldValue() { Label = concatenatedValue, Value = concatenatedValue, ThesaurusId = thesaurusId };
                    field.Values.Add(value);
                }
            }
        }

        private string GetNewId() 
        {
            id++;
            return id.ToString();
        }

        private int GetNewThesaurus(string label, string description = null) 
        {
            ThesaurusEntry thesaurus =  CreateThesaurus(label, description);
            thesaurusDAL.InsertOrUpdate(thesaurus);

            return thesaurus.Id;
        }
        private List<string[]> GetRowsFromCsv()
        {
            List<string[]> allRows = new List<string[]>();
            using (TextFieldParser parser = new TextFieldParser(@"C:\Users\Sotex\Desktop\CTCAE.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fieldss = parser.ReadFields();
                    allRows.Add(fieldss);
                }
            }

            classIndicators = allRows[1];
            //remove first 2 line in rows its comment and class indicator
            allRows.RemoveAt(0);
            allRows.RemoveAt(0);

            return allRows;
        }

    }
}
