using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.DAL.Sql.Interfaces;
using sReportsV2.Common.Helpers;
using System.Configuration;

namespace sReportsV2.UMLS.Classes
{
    public class SqlImporter
    {
        private int insertedThousands;
        private string basePath;
        private readonly int bulkInsertStep;

        private readonly Dictionary<RankKey, string> rankDictionary;
        private readonly Dictionary<string, List<string>> defDictionary;
        private readonly Dictionary<string, string> languages;
        private List<ThesaurusEntry> thesauruses;

        private readonly IThesaurusDAL thesaurusDAL;
        private readonly IThesaurusTranslationDAL translationDAL;
        private readonly IO4CodeableConceptDAL o4codeableConceptDAL ;
        private readonly ICodeSystemDAL codeSystemDAL;
        private readonly IAdministrativeDataDAL administrativeDataDAL;

        public SqlImporter(IThesaurusDAL thesaurusDAL, IThesaurusTranslationDAL translationDAL, IO4CodeableConceptDAL o4codeableConceptDAL, ICodeSystemDAL codeSystemDAL, IAdministrativeDataDAL administrativeDataDAL) 
        {
            this.thesaurusDAL = thesaurusDAL;
            this.translationDAL = translationDAL;
            this.o4codeableConceptDAL = o4codeableConceptDAL;
            this.codeSystemDAL = codeSystemDAL;
            this.administrativeDataDAL = administrativeDataDAL;

            this.basePath = string.Empty;
            this.insertedThousands = 0;
            this.bulkInsertStep = 50000;
            this.thesauruses = new List<ThesaurusEntry>();
            this.rankDictionary = new Dictionary<RankKey, string>();
            this.defDictionary = new Dictionary<string, List<string>>();
            this.languages = new Dictionary<string, string>()
            {
                {"ENG", "en" },
                {"FRE", "fr" },
                {"GER", "de" },
                {"ITA", "it" },
                {"SPA", "es" },
                {"POR", "pt" }
            };
        }

        public void Import()
        {
            if (thesaurusDAL.GetAllCount() == 0) 
            {
                SetBasePath();

                LoadMRRANKIntoMemory();
                LoadMRDEFIntoMemory();
                LoadMRCONSOIntoMemory();
            }
        
        }

        public void ImportCodingSystems() 
        {
            if (codeSystemDAL.GetAllCount() == 0) 
            {
                SetBasePath();

                LoadMRSABIntoMemory();
            }
        }

        private void SetBasePath()
        {            
            try
            {
                bool isAzureInstance = Convert.ToBoolean(ConfigurationManager.AppSettings["AzureInstance"]);
                this.basePath = isAzureInstance ? BlobStorageHelper.GetUrl("UMLS") : string.Empty;
            }
            catch (Exception ex)
            {
                HandleException(ex, "SetBasePath");
            }
        }

        private void LoadMRCONSOIntoMemory()
        {

            string currentConcept = string.Empty;
            int currentHigherRank = 0;
            ThesaurusEntry thesaurus = new ThesaurusEntry("");
            try
            {
                List<CodeSystem> codeSystems = codeSystemDAL.GetAll();
                var path = $@"{basePath}/MRCONSO.RRF";
                var webRequest = WebRequest.Create(path);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (currentConcept != parts[0])
                        {
                            if (!string.IsNullOrWhiteSpace(currentConcept)) 
                            {
                                thesauruses.Add(thesaurus);
                            }
                            InsertIfBatchIsFull();
                            currentConcept = parts[0];
                            currentHigherRank = 0;
                            thesaurus = new ThesaurusEntry(parts[0]);
                            var codeUmls = codeSystems.FirstOrDefault(x => x.SAB == "MTH");
                            thesaurus.SetCode(new O4CodeableConcept()
                            {
                                CodeSystemId = codeUmls.CodeSystemId,
                                Code = parts[0],
                                Value = parts[14],
                                VersionPublishDate = DateTime.Now,
                                EntryDateTime = DateTime.Now,
                            });
                        }
                        if (languages.ContainsKey(parts[1]))
                        {
                            string definition = GetDefinition(parts[7]);
                            thesaurus.SetTranslation(definition, languages[parts[1]]);

                            SetDefinitionOfConcept(parts[11], parts[12], currentHigherRank, definition, parts[1], thesaurus);
                        }
                        

                        var code = codeSystems.FirstOrDefault(x => parts[11].Contains(x.SAB));
                        if (code != null) 
                        {
                            thesaurus.SetCode(new O4CodeableConcept()
                            {
                                Code = parts[13],
                                Value = parts[14],
                                VersionPublishDate = DateTime.Now,
                                CodeSystemId = code.CodeSystemId,
                                EntryDateTime = DateTime.Now,
                            });
                        }

                    }

                    InsertThesauruses(thesauruses);
                }
            }
            catch (Exception e)
            {
                HandleException(e, "MRCONSO.RRF");
            }
        }

        private void SetDefinitionOfConcept(string sab, string tty, int currentHigherRank, string definition, string language, ThesaurusEntry thesaurus)
        {
            RankKey rankKey = new RankKey()
            {
                SAB = sab,
                TTY = tty
            };

            if (rankDictionary.ContainsKey(rankKey))
            {
                int rankOfNewAtom = Int32.Parse(rankDictionary[rankKey]);

                if (!string.IsNullOrWhiteSpace(definition) && rankOfNewAtom > currentHigherRank)
                {
                    currentHigherRank = rankOfNewAtom;
                    thesaurus.Translations.FirstOrDefault(x => x.Language == languages[language]).Definition = definition;
                }
            }
        }

        private string GetDefinition(string atomIdentifier)
        {
            return defDictionary.ContainsKey(atomIdentifier) ? string.Join(Environment.NewLine, defDictionary[atomIdentifier]) : string.Empty;
        }

        private void InsertIfBatchIsFull()
        {
            if (thesauruses.Count() == bulkInsertStep)
            {
                LogHelper.Info($"{++insertedThousands}");
                InsertThesauruses(thesauruses);
                thesauruses = new List<ThesaurusEntry>();
            }
        }

        private void InsertThesauruses(List<ThesaurusEntry> thesauruses) 
        {
            thesaurusDAL.InsertMany(thesauruses);
            var bulkedThesauruses = thesaurusDAL.GetLastBulkInserted(thesauruses.Count());
            translationDAL.InsertMany(thesauruses, bulkedThesauruses);
            o4codeableConceptDAL.InsertMany(thesauruses, bulkedThesauruses);
            administrativeDataDAL.InsertMany(thesauruses, bulkedThesauruses);
            administrativeDataDAL.InsertManyVersions(thesauruses, bulkedThesauruses);
        }

        private void LoadMRDEFIntoMemory()
        {
            try
            {
                var path = $@"{basePath}/MRDEF.RRF";
                var webRequest = WebRequest.Create(path);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line)) 
                        {
                            string[] parts = line.Split('|');
                            if (!defDictionary.ContainsKey(parts[1]))
                            {
                                defDictionary.Add(parts[1], new List<string>() { parts[5] });
                            }
                            else
                            {
                                defDictionary[parts[1]].Add(parts[5]);
                            }
                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, "MRDEF.RRF");
            }
        }

        private void LoadMRRANKIntoMemory()
        {
            try
            {
                var path = $@"{basePath}/MRRANK.RRF";
                var webRequest = WebRequest.Create(path);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        RankKey key = new RankKey() { SAB = parts[1], TTY = parts[2] };
                        if (rankDictionary.Any(x => x.Key.Equals(key)))
                        {
                            rankDictionary.Add(key, parts[0]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, "MRRANK.RRF");
            }
        }

        private void LoadMRSABIntoMemory()
        {
            List<CodeSystem> codingSystems = new List<CodeSystem>();
            try
            {
                var path = $@"{basePath}/MRSAB.RRF";
                var webRequest = WebRequest.Create(path);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');

                        if (!string.IsNullOrEmpty(parts[11]) && !string.IsNullOrWhiteSpace(parts[4]) && !codingSystems.Any(x => x.SAB == parts[3])) 
                        {
                            codingSystems.Add(new CodeSystem()
                            {
                                Value = parts[11].Split(';')[11],
                                Label = parts[4],
                                SAB = parts[3]
                            });
                        }
                    }
                }
                codeSystemDAL.InsertMany(codingSystems);
            }
            catch (Exception e)
            {
                HandleException(e, "MRSAB.RRF");
            }
        }

        private void HandleException(Exception ex, string fileName)
        {
            LogHelper.Error($"The file ({fileName}) could not be read, exception message: {ex.Message}");
            LogHelper.Error(ex.StackTrace);
        }
    }
}
