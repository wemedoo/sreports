using sReportsV2.Domain.Entities.Common;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Entities.FieldEntity;
using sReportsV2.Domain.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.Common.Entities.User;

namespace Generator
{
    public class ThesaurusGenerator : ThesaurusCommon
    {
        public ThesaurusDAL thesaurusDAL = new ThesaurusDAL(new sReportsV2.DAL.Sql.Sql.SReportsContext());
        public FormDAL formService = new FormDAL();
        UserData userData;

        public void GenerateThesauruses(Form form, UserData user) 
        {
            userData = user;

            if (!string.IsNullOrWhiteSpace(form.Title)) 
            {
                form.ThesaurusId = GetNewThesaurus(form.Title);
            }

            GenerateThesaurusesForChapters(form.Chapters);

        }

        private void GenerateThesaurusesForChapters(List<FormChapter> chapters) 
        {
            foreach (FormChapter chapter in chapters)
            {
                if (!string.IsNullOrWhiteSpace(chapter.Title))
                {
                    chapter.ThesaurusId = GetNewThesaurus(chapter.Title);
                }

                GenerateThesaurusesForPages(chapter.Pages);
            }
        }

        private void GenerateThesaurusesForPages(List<FormPage> pages)
        {
            foreach (FormPage page in pages)
            {
                if (!string.IsNullOrWhiteSpace(page.Title))
                {
                    page.ThesaurusId = GetNewThesaurus(page.Title);
                }

                GenerateThesaurusesForFieldSets(page.ListOfFieldSets);
            }
        }

        private void GenerateThesaurusesForFieldSets(List<List<FieldSet>> fieldSets)
        {
            foreach (List<FieldSet> listOfFS in fieldSets)
            {
                foreach(FieldSet fieldSet in listOfFS)
                {
                    if (!string.IsNullOrWhiteSpace(fieldSet.Label))
                    {
                        fieldSet.ThesaurusId = GetNewThesaurus(fieldSet.Label);
                    }

                    GenerateThesaurusesForFields(fieldSet.Fields);
                }

            }
        }

        private void GenerateThesaurusesForFields(List<Field> fields)
        {
            foreach (Field field in fields)
            {
                if (!string.IsNullOrWhiteSpace(field.Label))
                {
                    field.ThesaurusId = GetNewThesaurus(field.Label);
                }

                GenerateThesaurusesForFieldValues((field as FieldSelectable)?.Values);
            }
        }

        private void GenerateThesaurusesForFieldValues(List<FormFieldValue> values) 
        {
            if (values != null) 
            {
                foreach (FormFieldValue value in values) 
                {
                    if (!string.IsNullOrWhiteSpace(value.Label)) 
                    {
                        value.ThesaurusId = GetNewThesaurus(value.Label);
                    }
                }
            }
        }

        private int GetNewThesaurus(string label, string description = null)
        {
            ThesaurusEntry thesaurus = CreateThesaurus(label, description);
            thesaurusDAL.InsertOrUpdate(thesaurus);

            return thesaurus.ThesaurusEntryId;
        }
    }
}
