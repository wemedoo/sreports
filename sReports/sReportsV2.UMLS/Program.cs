using sReportsV2.Domain.Entities.UMLSEntities;
using sReportsV2.Domain.Mongo;
using sReportsV2.Domain.Services.Implementations;
using sReportsV2.Domain.Services.Interfaces;
using sReportsV2.Domain.Sql.Entities.ThesaurusEntry;
using sReportsV2.DAL.Sql.Sql;
using sReportsV2.UMLS.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.UMLS
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new SReportsContext()) 
            {
                
                var thesauruses = context.Thesauruses.ToList();
            }
                Console.ReadKey();
        }
        

      
    }
}
