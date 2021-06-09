using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.DigitalGuideline.EvidenceProperties
{
    [BsonIgnoreExtraElements]
    public class EvidenceProperties
    {
        public List<Publication> Publications { get; set; }  
        public EvidenceCategory EvidenceCategory { get; set; }
    }
}
