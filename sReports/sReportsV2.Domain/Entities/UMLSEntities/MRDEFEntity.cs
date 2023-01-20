using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.UMLSEntities
{
    // ---------------------------- NOT USED ANYMORE ---------------------------------------
    public class MRDEFEntity : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CUI { get; set; }
        public string AUI { get; set; }
        public string ATUI { get; set; }
        public string SATUI { get; set; }
        public string SAB { get; set; }
        public string DEF { get; set; }
        public string SUPPRESS { get; set; }
        public string CVF { get; set; }

        public MRDEFEntity() { }
        public MRDEFEntity(string cui, string aui, string atui,string satui, string sab, string def, string suppress, string cvf) 
        {
            this.CUI = cui;
            this.AUI = aui;
            this.ATUI = atui;
            this.SATUI = satui;
            this.SAB = sab;
            this.DEF = def;
            this.SUPPRESS = suppress;
            this.CVF = cvf;
            this.IsDeleted = false;
            this.Copy(null);
        }
    }
}
