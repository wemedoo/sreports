using sReportsV2.Domain.Mongo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.LoincParser
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoConfiguration.ConnectionString = ConfigurationManager.AppSettings["MongoDB"];
            LoincParser.LoincParser parser = new sReportsV2.LoincParser.LoincParser.LoincParser();
            parser.ImportThesaurusesAndFormsFromLoinc(Directory.GetCurrentDirectory());
        }
    }
}
