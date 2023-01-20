using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReports.PathoLink.Entities
{
    public class PathoLinkField
    {
        public string o40MtId { get; set; }

        public string value { get; set; }

        public string name { get; set; }

        public string type { get; set; }

        public string defaultValue { get; set; }

        public void RemoveFormIdFromO4MTId()
        {
            string[] splitted = o40MtId.Split('-').Skip(1).ToArray();
            this.o40MtId = string.Join("-", splitted);
        }
    }
}
