using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Common
{
    public class Period
    {
        //[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Start { get; set; }

        //[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? End { get; set; }
    }
}
