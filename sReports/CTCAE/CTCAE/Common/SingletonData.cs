using CTCAE.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CTCAE.Common
{
    sealed class SingletonData
    {
        private static SingletonData instance;
        private readonly List<CTCAEModel> ctceaModels = new List<CTCAEModel>();
        private SingletonData()
        {
            this.PopulateCTCAEModels();
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
            return this.ctceaModels.OrderBy(i => i.CTCAETerm).ToList();
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
                ctceaModels.Add(ctcaeModel);
            }
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
    }
}
