using AutoMapper;
using CSVExport.Models;
using Microsoft.VisualBasic.FileIO;
using sReportsV2.DTOs.Umls.DatOut;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using UmlsClient.UMLSClasses;
using UMLSClient.Client;

namespace sReportsV2.Controllers
{
    public class CSVExportController : BaseController
    {
        public static List<CSVModel> csvModels = new List<CSVModel>();
        public static List<SecondCSV> secondCsvs = new List<SecondCSV>();
        // GET: CSVExport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoadCSV()
        {
            return View();
        }
     
        public void SetFirstCSV(HttpPostedFileBase file)
        {
            csvModels.Clear();
            List<string[]> allRows = new List<string[]>();
            GetCSVRows(file, allRows);
            SetFirstCsvList(allRows);
        }

        public void SetSecondCSV(HttpPostedFileBase file)
        {
            secondCsvs.Clear();
            List<string[]> allRows = new List<string[]>();
            GetCSVRows(file, allRows);
            SetSecondCsvList(allRows);
        }

        public ActionResult ExportCSV()
        {
            var csv = csvModels;
            var secondCSV = secondCsvs.GroupBy(x => x.PID);
            var sb = new StringBuilder();
            sb.AppendLine("PID, 1.RT, last RT, DAY_1, DAY_2, DAY_3, DAY_4, DAY_5, DAY_6, DAY_7, DAY_8, DAY_9, DAY_10, DAY_11, DAY_12, DAY_13, DAY_14," +
                "DAY_15, DAY_16, DAY_17, DAY_18, DAY_19, DAY_20, DAY_21, DAY_22, DAY_23, DAY_24, DAY_25, DAY_26, DAY_27, DAY_28," +
                "DAY_29, DAY_30, DAY_31, DAY_32, DAY_33, DAY_34, DAY_35, DAY_36, DAY_37, DAY_38, DAY_39, DAY_40, DAY_41, DAY_42");
            foreach (var item in csv)
            {
                var csvSecondModel = secondCSV.FirstOrDefault(x => x.Key.Equals(item.PID));
                if (csvSecondModel != null)
                {
                    List<DateTime> list = SetGroupedItems(item, csvSecondModel);
                    SetProperties(item, list);
                }
                WriteDataToCSV(item, sb);
            }

            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv");
        }

        public ActionResult ExportFromUMLS(string term)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Ui, Route source, Name, Definition, Atoms, Semantic Types");
            Client umlsClient = new Client();
            int currentPage = 1;
            while (currentPage != -1)
            {
                UmlsDataOut<SearchResultDataOut> result = Mapper.Map<UmlsDataOut<SearchResultDataOut>>(umlsClient.GetSearchResult(term, 100, currentPage));
                List<ConceptSearchResultDataOut> umlsList = new List<ConceptSearchResultDataOut>();
                for (int i = 0; i < result.Result.Results.Count; i++)
                    umlsList.Add(result.Result.Results[i]);
                
                foreach (var umls in umlsList)
                {
                    string definition = GetDefinitions(umlsClient, umls);
                    string atom = GetAtoms(umlsClient, umls);
                    string semanticTypes = umls.Ui != "NONE" ? GetSemanticTypes(umlsClient, umls) : "";
                    if(umls.Ui != "NONE")
                        sb.AppendLine(umls.Ui + "," + umls.RootSource + "," + "\"" + umls.Name + "\"" + "," + "\"" + definition + "\"" + "," + "\"" + atom + "\"" + "," + "\"" + semanticTypes + "\"");
                }
                bool hasNoResults = result.Result.Results.Count == 0 || result.Result.Results[0].Name.Equals("NO RESULTS"); 
                currentPage = hasNoResults ? -1 : currentPage + 1;
            }

            return File(new UTF8Encoding().GetBytes(sb.ToString()), "text/csv");
        }

        private string GetDefinitions(Client umlsClient, ConceptSearchResultDataOut umls) 
        {
            string definition = "";
            var definitions = Mapper.Map<UmlsDataOut<List<ConceptDefinitionDataOut>>>(umlsClient.GetConceptDefinition(umls.Ui));
            if (definitions != null)
            {
                foreach (var def in definitions.Result)
                {
                    if (def.RootSource == "NCI")
                        definition += def.Value + "\n";
                }
            }
            return definition;
        }

        private string GetAtoms(Client umlsClient, ConceptSearchResultDataOut umls)
        {
            string atom = "";
            var atoms = Mapper.Map<UmlsDataOut<List<AtomDataOut>>>(umlsClient.GetAtomsResult(umls.Ui));
            if (atoms != null)
            {
                foreach (var at in atoms.Result)
                {
                    if (at.RootSource == "NCI")
                        atom += at.Name + "\n"; 
                }
            }
            return atom;
        }

        private string GetSemanticTypes(Client umlsClient, ConceptSearchResultDataOut umls)
        {
            string semanticType = "";
            UMLSSemanticTypes semanticTypes = umlsClient.GetSemanticTypes(umls.Ui);
            if (semanticTypes != null)
                semanticType = semanticTypes.Result.Name;

            return semanticType;
        }

        private void GetCSVRows(HttpPostedFileBase file, List<string[]> allRows)
        {
            StreamReader csvreader = new StreamReader(file.InputStream);
            while (!csvreader.EndOfStream)
            {
                var line = csvreader.ReadLine();
                var values = line.Split(',');
                allRows.Add(values);
            }
        }

        private void SetFirstCsvList(List<string[]> allRows)
        {
            allRows.RemoveAt(0);
            allRows.RemoveAt(0);
            allRows.RemoveAt(0);
            foreach (var row in allRows)
            {
                CSVModel cSVModel = new CSVModel();
                cSVModel.PID = row[0];
                cSVModel.BeginDate = DateTime.Parse(row[1]);
                cSVModel.LastDate = DateTime.Parse(row[2]).AddHours(23).AddMinutes(59).AddSeconds(59);
                csvModels.Add(cSVModel);
            }
        }

        private void SetSecondCsvList(List<string[]> allRows)
        {
            allRows.RemoveAt(0);
            foreach (var row in allRows)
            {
                SecondCSV cSVModel = new SecondCSV();
                cSVModel.PID = row[0];
                cSVModel.DateTime = DateTime.Parse(row[1]);
                secondCsvs.Add(cSVModel);
            }
        }

        private void WriteDataToCSV(CSVModel item, StringBuilder sb) 
        {
            sb.AppendLine(item.PID + "," + item.BeginDate.ToString("MM/dd/yyyy") + "," + item.LastDate.ToString("MM/dd/yyyy") + "," + item.Day1 + "," + item.Day2 + "," + item.Day3 + "," + item.Day4 + ","
                    + item.Day5 + "," + item.Day6 + "," + item.Day7 + "," + item.Day8 + "," + item.Day9 + "," + item.Day10 + "," + item.Day11 + "," + item.Day12 + ","
                    + item.Day13 + "," + item.Day14 + "," + item.Day15 + "," + item.Day16 + "," + item.Day17 + "," + item.Day18 + "," + item.Day19 + "," + item.Day20
                    + "," + item.Day21 + "," + item.Day22 + "," + item.Day23 + "," + item.Day24 + "," + item.Day25 + "," + item.Day26 + "," + item.Day27 + "," + item.Day28 + "," + item.Day29 + "," + item.Day30
                    + "," + item.Day31 + "," + item.Day32 + "," + item.Day33 + "," + item.Day34 + "," + item.Day35 + "," + item.Day36 + "," + item.Day37 + "," + item.Day38 + "," + item.Day39 + "," + item.Day40
                    + "," + item.Day41 + "," + item.Day42);
        }

        private List<DateTime> SetGroupedItems(CSVModel item, IGrouping<string, SecondCSV> csvSecondModel) 
        {
            var groupedByDate = csvSecondModel.GroupBy(x => new { x.DateTime.Day, x.DateTime.Month, x.DateTime.Year });
            List<DateTime> list = new List<DateTime>();
            for (int i = 0; i < groupedByDate.Count(); i++)
            {
                if (groupedByDate.ElementAt(i).Select(x => x.DateTime).FirstOrDefault() >= item.BeginDate && groupedByDate.ElementAt(i).Select(x => x.DateTime).FirstOrDefault() <= item.LastDate)
                    list.Add(groupedByDate.ElementAt(i).Select(x => x.DateTime).FirstOrDefault());
            }
            return list;
        }

        private void SetProperties(CSVModel item, List<DateTime> list) 
        {
            for (int i = 0; i < list.Count; i++)
            {
                PropertyInfo prop = item.GetType().GetProperty($"Day{i + 1}");
                prop.SetValue(item, list[i].ToString());
            }
        }
    }
}