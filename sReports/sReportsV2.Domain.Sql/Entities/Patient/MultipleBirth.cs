using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class MultipleBirth
    {
        public int Id { get; set; }

        public int Number { get; set; }
        public bool isMultipleBorn { get; set; }

        public MultipleBirth(int number, bool ismultipleborn)
        {
            this.Number = number;
            this.isMultipleBorn = ismultipleborn;
        }
        public MultipleBirth()
        {
        }
    }
}
