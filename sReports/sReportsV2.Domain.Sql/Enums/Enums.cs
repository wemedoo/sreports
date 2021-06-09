using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.SqlDomain.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GlobalUserSource
    {
        Google,
        Microsoft,
        Internal
    }
}
