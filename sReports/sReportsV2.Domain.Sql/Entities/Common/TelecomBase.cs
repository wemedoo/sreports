using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public abstract class TelecomBase : EntitiesBase.Entity
    {
        public string System { get; set; }
        public string Value { get; set; }
        public string Use { get; set; }

        public void Copy(TelecomBase telecom)
        {
            this.System = telecom.System;
            this.Value = telecom.Value;
            this.Use = telecom.Use;
        }
    }
}
