using DocumentsParser.Csv.ThesaurusTranslation;
using sReportsV2.Domain.Mongo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentsParser
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoConfiguration.ConnectionString = "mongodb+srv://smladen:Mali2#3@sreportsdev-iedrt.mongodb.net/admin?retryWrites=true";
        }
    }
}
