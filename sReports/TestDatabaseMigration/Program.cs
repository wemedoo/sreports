using sReportsV2.Domain.Entities.Form;
using sReportsV2.Domain.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestDatabaseMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            sReportsV2.Domain.Mongo.MongoConfiguration.ConnectionString = "mongodb+srv://smladen:Mali2#3@sreportsdev-iedrt.mongodb.net/admin?retryWrites=true";
            FormService formService = new FormService();
            List<Form> forms =  formService.GetAll(new sReportsV2.Domain.Entities.Form.FormFilterData() { ActiveLanguage = "en", Page  =1, PageSize = 100});
        }
    }
}
