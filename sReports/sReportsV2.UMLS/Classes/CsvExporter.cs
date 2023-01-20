using sReportsV2.Common.Helpers;
using sReportsV2.Domain.Entities.ThesaurusEntry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace sReportsV2.UMLS.Classes
{
    public class CsvExporter
    {
        public static int insertedThousands = 0;
        public static string basePath = string.Empty;
        public static List<string> loadedConcepts = new List<string>();

        public static Dictionary<RankKey, string> rankDictionary = new Dictionary<RankKey, string>();
        public static Dictionary<string, List<string>> defDictionary = new Dictionary<string, List<string>>();
        public static List<ThesaurusEntry> thesauruses = new List<ThesaurusEntry>();
        public static Dictionary<string, string> languages = new Dictionary<string, string>()
        {
            {"ENG", "en" },
            {"FRE", "fr" },
            {"GER", "de" },
            {"ITA", "it" },
            {"SPA", "es" },
            {"POR", "pt" }
        };

        public static List<string> terms = new List<string>();
        public static string currentRootSource = string.Empty;
        public static List<UmlsConcept> concepts = new List<UmlsConcept>();
        public static Dictionary<string, string> semantycTypeDictionary = new Dictionary<string, string>();


        public static List<UmlsCsvEntity> csvEntries = new List<UmlsCsvEntity>();
        public static int conceptCount = 0;
        public static int errorCount = 0;

        public CsvExporter() { }
        public void Import(string path)
        {
            basePath = path;
            LoadMRRANKIntoMemory();
            LoadMRDEFIntoMemory();
            LoadMRSTYIntoMemory();
            LoadMRCONSOIntoMemory();
        }

        private List<UmlsCsvEntity> GetCsvEntities()
        {
            return csvEntries;
        }

        private void SetTermsCsv(List<string> values)
        {
            terms = values;
        }

        private static void LoadMRCONSOIntoMemory()
        {
            string currentConcept = string.Empty;
            UmlsConcept concept = new UmlsConcept();
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
                            AddToCsvEntriesIfValid(concept);
                            conceptCount++;
                            currentConcept = parts[0];
                            concept = new UmlsConcept() 
                            {
                                CUI = parts[0],
                                TTY = parts[12],
                                Atoms = new List<UmlsAtom>()
                            };

                        }
                        if (languages.ContainsKey(parts[1]))
                        {
                            UmlsAtom atom = new UmlsAtom() 
                            {
                                Language = parts[1],
                                Definition = GetDefinition(parts[7]),
                                Name = parts[14],
                                AUI = parts[7],
                                Source = parts[11]
                            };
                            concept.Atoms.Add(atom);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                errorCount++;
                HandleException(e, "MRCONSO.RRF");
            }
        }

        private static void AddToCsvEntriesIfValid(UmlsConcept concept)
        {
            if (concept != null && concept.Atoms != null) 
            {
                foreach (string term in terms) 
                {
                    if(concept.Atoms.Select(y => y.Name).ToList().Any(z => z.Contains(term))) 
                    {
                        csvEntries.Add(new UmlsCsvEntity()
                        {
                            UI = concept.CUI,
                            Name = concept.Atoms.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Name))?.Name,
                            Definition = concept.Atoms.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x.Definition))?.Definition,
                            Atoms = GetAtomsForCsv(concept.Atoms),
                            SemanticType = semantycTypeDictionary[concept.CUI],
                            Term = term
                        });

                        break;
                    }
                }
            }
        }

        private static string GetAtomsForCsv(List<UmlsAtom> atoms)
        {
            string result = string.Empty;
            foreach (UmlsAtom atom in atoms)
            {
                result += $"{atom?.Name} ({atom?.AUI}- {atom?.Source} - {atom?.Language})" + "\n";
            }

            return result;
        }



        private static string GetDefinition(string atomIdentifier)
        {
            return defDictionary.ContainsKey(atomIdentifier) ? string.Join(Environment.NewLine, defDictionary[atomIdentifier]) : string.Empty;
        }
        private static void LoadMRDEFIntoMemory()
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
            catch (Exception e)
            {
                HandleException(e, "MRDEF.RRF");
            }
        }

        private static void LoadMRRANKIntoMemory()
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
                        if (rankDictionary.Select(x => x.Key).Where(y => y.Equals(key)).ToList().Count() == 0)
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
        private static void LoadMRSTYIntoMemory()
        {
            try
            {
                var path = $@"{basePath}/MRSTY.RRF";
                var webRequest = WebRequest.Create(path);

                using (var response = webRequest.GetResponse())
                using (var content = response.GetResponseStream())
                using (var reader = new StreamReader(content))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split('|');
                        if (!semantycTypeDictionary.ContainsKey(parts[0])) 
                        {
                            semantycTypeDictionary.Add(parts[0], parts[3]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HandleException(e, "MRSTY.RRF");
            }
        }

        private static void HandleException(Exception ex, string fileName)
        {
            LogHelper.Error($"The file ({fileName}) could not be read, exception message: {ex.Message}");
            LogHelper.Error(ex.StackTrace);
        }
    }
}
