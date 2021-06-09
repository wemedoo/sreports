using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DAL.Sql.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using sReportsV2.SqlDomain.Implementations;
using sReportsV2.SqlDomain.Interfaces;
using sReportsV2.Domain.Sql.Entities.CodeSystem;
using sReportsV2.DAL.Sql.Interfaces;

namespace sReportsV2.UMLS.Classes
{
    public class SqlImporter
    {
        public int insertedThousands=0;
        public string basePath = string.Empty;
        public List<string> loadedConcepts = new List<string>();

        public Dictionary<RankKey, string> rankDictionary = new Dictionary<RankKey, string>();
        public Dictionary<string, List<string>> defDictionary = new Dictionary<string, List<string>>();
        public List<ThesaurusEntry> thesauruses = new List<ThesaurusEntry>();
        public Dictionary<string, string> languages = new Dictionary<string, string>()
        {
            {"ENG", "en" },
            {"FRE", "fr" },
            {"GER", "de" },
            {"ITA", "it" },
            {"SPA", "es" },
            {"POR", "pt" }
        };

        public IThesaurusDAL thesaurusDAL;
        public IThesaurusTranslationDAL translationDAL;
        //public SimilarTermService similiarTermService = new SimilarTermService();
        public IO4CodeableConceptDAL o4codeableConceptDAL ;
        public ICodeSystemDAL codeSystemDAL;

        public SqlImporter() 
        {
        }

        public SqlImporter(IThesaurusDAL thesaurusDAL, IThesaurusTranslationDAL translationDAL, IO4CodeableConceptDAL o4codeableConceptDAL, ICodeSystemDAL codeSystemDAL) 
        {
            this.thesaurusDAL = thesaurusDAL;
            this.translationDAL = translationDAL;
            this.o4codeableConceptDAL = o4codeableConceptDAL;
            this.codeSystemDAL = codeSystemDAL;
        }

        public int bulkInsertStep = 50000;
        public List<CodeSystem> codeSystems;

        public void Import(string path)
        {
            if (thesaurusDAL.GetAllCount() == 0) 
            {
                codeSystems = codeSystemDAL.GetAll();

                basePath = path;

                LoadMRRANKIntoMemory();
                LoadMRDEFIntoMemory();
                LoadMRCONSOIntoMemory();
            }
        
        }

        public void ImportCodingSystems(string path) 
        {
            if (codeSystemDAL.GetAllCount() == 0) 
            {
                basePath = path;
                LoadMRSABIntoMemory();
            }
        }

        public void LoadMRCONSOIntoMemory()
        {

            string currentConcept = string.Empty;
            int currentHigherRank = 0;
            ThesaurusEntry thesaurus = new ThesaurusEntry("");
            try
            {
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
                                CodeSystemId = codeUmls.Id,
                                Code = parts[0],
                                Value = parts[14],
                                VersionPublishDate = DateTime.Now
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
                                CodeSystemId = code.Id
                            });
                        }

                    }

                    InsertThesauruses(thesauruses);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }


        }

        public void SetDefinitionOfConcept(string sab, string tty, int currentHigherRank, string definition, string language, ThesaurusEntry thesaurus)
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


        public string GetDefinition(string atomIdentifier)
        {
            return defDictionary.ContainsKey(atomIdentifier) ? string.Join(Environment.NewLine, defDictionary[atomIdentifier]) : string.Empty;
        }


        public void InsertIfBatchIsFull()
        {
            if (thesauruses.Count() == bulkInsertStep)
            {
                Console.WriteLine(++insertedThousands);
                InsertThesauruses(thesauruses);
                thesauruses = new List<ThesaurusEntry>();
            }
        }

        public void InsertThesauruses(List<ThesaurusEntry> thesauruses) 
        {
            thesaurusDAL.InsertMany(thesauruses);
            var bulkedThesauruses = thesaurusDAL.GetLastBulkInserted(thesauruses.Count());
            translationDAL.InsertMany(thesauruses, bulkedThesauruses);
            o4codeableConceptDAL.InsertMany(thesauruses, bulkedThesauruses);
        }

        public void LoadMRDEFIntoMemory()
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
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void LoadMRRANKIntoMemory()
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
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        public void LoadMRSABIntoMemory()
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

                        if (!string.IsNullOrEmpty(parts[11]) && parts[11].Split(';').Count() > 11 &&!string.IsNullOrWhiteSpace(parts[11].Split(';')[11]) && !string.IsNullOrWhiteSpace(parts[4]) && !codingSystems.Any(x => x.SAB == parts[3])) 
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
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
