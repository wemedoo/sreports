using sReportsV2.Domain.Entities.Form;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            sReportsV2.Domain.Mongo.MongoConfiguration.ConnectionString = ConfigurationManager.AppSettings["MongoDB"];
            //FormGenerator generator = new FormGenerator();
            //Form form = generator.GetFormFromCsv("fafafa");

            
        }
    }
}
