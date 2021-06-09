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
        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime Start { get; set; }

        [BsonDateTimeOptions(DateOnly = true)]
        public DateTime? End { get; set; }
    }

    public class PeriodDatetime
    {
        public DateTime Start { get; set; }

        public DateTime? End { get; set; }
    }
}

