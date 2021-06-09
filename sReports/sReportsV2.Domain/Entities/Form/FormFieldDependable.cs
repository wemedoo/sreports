using MongoDB.Bson.Serialization.Attributes;
using sReportsV2.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.Form
{
    [BsonIgnoreExtraElements]
    public class FormFieldDependable
    {
        public string Condition { get; set; }
        public FormFieldDependableType? ActionType { get; set; }
        public string ActionParams { get; set; }
    }
}
