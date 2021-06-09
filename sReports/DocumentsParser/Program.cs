using DocumentsParser.Csv.ThesaurusTranslation;
using sReportsV2.Domain.Mongo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsParser
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoConfiguration.ConnectionString = ConfigurationManager.AppSettings["MongoDB"]; ;
        }
    }
}
