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
    public class MRCONSOEntity : Entity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string CUI { get; set; }
        public string LAT { get; set; }
        public string TS { get; set; }
        public string LUI { get; set; }
        public string STT { get; set; }
        public string SUI { get; set; }
        public string ISPREF { get; set; }
        public string AUI { get; set; }
        public string SAUI { get; set; }
        public string SCUI { get; set; }
        public string SDUI { get; set; }
        public string SAB { get; set; }
        public string TTY { get; set; }
        public string CODE { get; set; }
        public string STR { get; set; }
        public string SRL { get; set; }
        public string SUPPRESS { get; set; }
        public string CVF { get; set; }

        public MRCONSOEntity() { }
        public MRCONSOEntity(string cui, string lat, string ts, string lui, string stt, string sui, string ispref, string aui, string saui, string scui, string sdui, string sab, string tty, string code, string str, string srl, string suppress, string cvf) 
        {
            this.CUI = cui;
            this.LAT = lat;
            this.TS = ts;
            this.LUI = lui;
            this.STT = stt;
            this.SUI = sui;
            this.ISPREF = ispref;
            this.AUI = aui;
            this.SAUI = saui;
            this.SCUI = scui;
            this.SDUI = sdui;
            this.SAB = sab;
            this.TTY = tty;
            this.CODE = code;
            this.STR = str;
            this.SRL = srl;
            this.SUPPRESS = suppress;
            this.CVF = cvf;
            this.IsDeleted = false;
            this.Copy(null);
        }


    }
}
