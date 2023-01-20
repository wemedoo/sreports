using Microsoft.VisualBasic.FileIO;
using sReportsV2.Common.Enums;
using sReportsV2.Common.Enums.DocumentPropertiesEnums;
using sReportsV2.Domain.Entities.DocumentProperties;
using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.DAL.Sql.Implementations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.Common.Entities.User;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.Domain.Sql.Entities.User;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;

namespace sReportsV2.LoincParser.LoincParser
{
    public class LoincParser
    {
        public Dictionary<string, string> dictionary = new Dictionary<string, string>();
        public List<LoincDocument> documents = new List<LoincDocument>();
        public string basePath = string.Empty;
        public void ImportThesaurusesAndFormsFromLoinc(string path) 
        {
            basePath = path;
            ImportSettingsService importService = new ImportSettingsService();
            if (!importService.ExistSettings("LoincImport", 1))
            {
                //ParseDocumentsDomain(path);
                ParseOwl(path);
                //GenerateCsvFromResult(path);

                importService.InsertOne(new Domain.Entities.Common.ImportSettings() { Type = "LoincImport", Version = 1 });

            }
        }

        public void GenerateCsvFromResult(string path) 
        {
            Type itemType = typeof(LoincDocument);
            var props = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .OrderBy(p => p.Name);

            using (var writer = new StreamWriter(Path.Combine(path, "Test.csv")))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in documents)
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
            }
        }

        public void ParseDocuments() 
        {
            int docId = 0;
            string currentItem = string.Empty;
            LoincDocument document = null;

            var path = $@"{basePath}/DocumentOntology.csv";
            var webRequest = WebRequest.Create(path);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (TextFieldParser parser = new TextFieldParser(content))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();
                    if(fields[0] != currentItem) 
                    {
                        currentItem = fields[0];
                        if(document != null)
                        {
                            documents.Add(document);
                        }
                        document = new LoincDocument();
                        document.LoincIdentifier = fields[0];
                    }

                    document.SetProperty(fields[2], fields[4]);
                }
            }
        }

        public void ParseDocumentsDomain(string path)
        {
            int counter = 0;
            using (TextFieldParser parser = new TextFieldParser(Path.Combine(path, "DocumentOntology.csv")))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();
                    if(fields[2] == "Document.SubjectMatterDomain") 
                    {
                        if (!dictionary.ContainsKey(fields[0]))
                        {
                            dictionary.Add(fields[0], fields[4]);
                        }
                    }
                }
            }

            List<string> allDomains = dictionary
                .Select(x => string.Concat(ToUpperFirstLetter(x.Value.Replace(',', ' ').Replace('-', ' ')).Where(c => !Char.IsWhiteSpace(c))) )
                .Distinct()
                .OrderBy(x => x)
                .ToList();

           foreach(var enumValue in Enum.GetValues(typeof(DocumentClinicalDomain)).Cast<DocumentClinicalDomain>().ToList())
            {
                if (allDomains.Contains(enumValue.ToString()))
                {
                    allDomains.Remove(enumValue.ToString());
                    Console.WriteLine();

                };
            }
            string result = string.Join("," + Environment.NewLine, allDomains.ToArray());

            System.IO.File.WriteAllLines(Path.Combine(path, "domains.txt"), allDomains);

        }

        public  string ToUpperFirstLetter(string source)
        {
            return Regex.Replace(source, @"(^\w)|(\s\w)", m => m.Value.ToUpper());
        }

        public void SetNamesForLoincDocuments(string path) 
        {
            var keyValuePairs = GetKeyValuePairs();

            foreach (var doc in documents) 
            {
                if (keyValuePairs.FirstOrDefault(x => x.Key.Replace(" ","") == doc.LoincIdentifier) != null) 
                {
                    doc.Name = keyValuePairs.FirstOrDefault(x => x.Key.Replace(" ", "") == doc.LoincIdentifier).Value;
                }
            }
        }
        public void ParseOwl(string path)
        {
            ThesaurusDAL thesaurusDAL = new ThesaurusDAL(new SReportsContext());
            FormDAL formService = new FormDAL();
            //UserService userService = new UserService();
            UserDAL userDAL = new UserDAL(new SReportsContext()); 
            User user = userDAL.GetByUsername("smladen");

            ParseDocuments();
            SetNamesForLoincDocuments(path);

            foreach (var doc in documents)
            {
               ThesaurusEntry thesaurus = new ThesaurusEntry();
                thesaurus.Translations = new List<ThesaurusEntryTranslation>() {
                    new ThesaurusEntryTranslation() {
                        Language = "en",
                        PreferredTerm = doc.Name,
                    }
                };

                thesaurus.Codes = new List<O4CodeableConcept>() { 
                    new O4CodeableConcept() {
                        Code = doc.LoincIdentifier,
                       
                        VersionPublishDate = DateTime.Now.Date
                    }
                };
                thesaurus.State = ThesaurusState.Draft;

                thesaurusDAL.InsertOrUpdate(thesaurus);

                Form form = new Form();
                form.ThesaurusId = thesaurus.ThesaurusEntryId;
                form.Title = doc.Name;
                form.Version = new Domain.Entities.Form.Version() { Major = 1, Minor = 1, Id = Guid.NewGuid().ToString() };
                form.State = FormDefinitionState.DesignPending;
                form.UserId = user.UserId;
                form.OrganizationId = 1;
                form.Language = "en";
                form.DisablePatientData = true;

                if(!string.IsNullOrWhiteSpace(doc.SubjectMatterDomain)) 
                {
                    form.DocumentProperties = new DocumentProperties();
                    form.DocumentProperties.ClinicalDomain = new List<DocumentClinicalDomain?>();
                    form.DocumentProperties.ClinicalDomain.Add((DocumentClinicalDomain)Enum.Parse(typeof(DocumentClinicalDomain),ToUpperFirstLetter(doc.SubjectMatterDomain).Replace(" ", "")));
                }
                formService.InsertOrUpdate(form, GetUserDataOut(user));
            }

            Console.WriteLine("test");

        }

        public UserData GetUserDataOut(User user) 
        {
            return new  UserData() 
            { 
                FirstName = user.FirstName,
                LastName = user.LastName,
                ActiveOrganization = user.UserConfig.ActiveOrganizationId,
                Id = user.UserId,
                Organizations = user.GetOrganizationRefs(),
                Username = user.Username 
            };            
        }

        public List<CustomKeyValuePair> GetKeyValuePairs() 
        {
            List<string> lines = new List<string>();
            var path = $@"{basePath}/DocumentOntology.owl";
            var webRequest = WebRequest.Create(path);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }

            var formNames = lines.Where(x => x.Contains("# Class")).Select(x => x.Split(':').LastOrDefault()).ToList();
            return formNames.Select(x =>
                new CustomKeyValuePair {
                    Key = x.Split('(')[0],
                    Value = x.Split('(')[1].Replace(')', ' ') 
                }
                ).ToList();
        }
            
    }
}
