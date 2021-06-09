using CTCAE.Models;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CTCAE.Common
{
    sealed class SingletonData
    {
        private static SingletonData instance;
        private readonly List<CTCAEModel> ctcaeModels = new List<CTCAEModel>();
        private List<SelectItemModel> templatesList = new List<SelectItemModel>();
        private SingletonData()
        {
            this.PopulateCTCAEModels();
            this.PopulateTemplate();
        }
        public static SingletonData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SingletonData();
                }
                return instance;
            }
        }
        public List<CTCAEModel> GetCTCAEModels()
        {
            return this.ctcaeModels.OrderBy(i => i.CTCAETerm).ToList();
        }
        private void PopulateCTCAEModels()
        {
            List<string[]> csvRows = GetRowsFromCsv();
            foreach (var it in csvRows)
            {
                CTCAEModel ctcaeModel = new CTCAEModel();
                ctcaeModel.MedDraCode = it[0];
                ctcaeModel.CTCAETerm = it[2];
                ctcaeModel.Grade1 = it[3];
                ctcaeModel.Grade2 = it[4];
                ctcaeModel.Grade3 = it[5];
                ctcaeModel.Grade4 = it[6];
                ctcaeModel.Grade5 = it[7];
                ctcaeModels.Add(ctcaeModel);
            }
        }

        private void PopulateTemplate()
        {
            this.templatesList = GetTemplateList();
        }

        private List<string[]> GetRowsFromCsv()
        {
            List<string[]> allRows = new List<string[]>();
            var feedDirectoryPath = $"{AppDomain.CurrentDomain.GetData("DataDirectory")}\\CTCAE_v5.csv";
            using (TextFieldParser parser = new TextFieldParser(feedDirectoryPath))
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
            allRows.RemoveAt(0);

            return allRows;
        }

        public List<SelectItemModel> GetTemplateList() 
        {
            string json = File.ReadAllText($"{AppDomain.CurrentDomain.GetData("DataDirectory")}\\templates.json");
            List<SelectJsonModel> savedTemplates = JsonConvert.DeserializeObject<List<SelectJsonModel>>(json);
            List<SelectItemModel> templatesForRead = MappingFromJsonObject(savedTemplates);

            return templatesForRead;
        }

        private List<SelectItemModel> MappingFromJsonObject(List<SelectJsonModel> savedTemplates)
        {
            List<SelectItemModel> templatesForRead = new List<SelectItemModel>();
            List<CTCAEModel> ctcaeModel = GetCTCAEModels();
            for (int i = 0; i < savedTemplates.Count; i++)
            {
                SelectItemModel item = new SelectItemModel();
                item.Id = savedTemplates[i].Id;
                item.Label = savedTemplates[i].Label;
                for (int j = 0; j < savedTemplates[i].DefaultList.Count; j++)
                {
                    CTCAEModel ctcae = new CTCAEModel();
                    ctcae.MedDraCode = savedTemplates[i].DefaultList[j];
                    ctcae.CTCAETerm = ctcaeModel.Where(x => x.MedDraCode == ctcae.MedDraCode).FirstOrDefault().CTCAETerm;
                    ctcae.Grade1 = ctcaeModel.Where(x => x.MedDraCode == ctcae.MedDraCode).FirstOrDefault().Grade1;
                    ctcae.Grade2 = ctcaeModel.Where(x => x.MedDraCode == ctcae.MedDraCode).FirstOrDefault().Grade2;
                    ctcae.Grade3 = ctcaeModel.Where(x => x.MedDraCode == ctcae.MedDraCode).FirstOrDefault().Grade3;
                    ctcae.Grade4 = ctcaeModel.Where(x => x.MedDraCode == ctcae.MedDraCode).FirstOrDefault().Grade4;
                    ctcae.Grade5 = ctcaeModel.Where(x => x.MedDraCode == ctcae.MedDraCode).FirstOrDefault().Grade5;
                    item.DefaultList.Add(ctcae);
                }
                templatesForRead.Add(item);
            }

            return templatesForRead;
        }

    }
}
