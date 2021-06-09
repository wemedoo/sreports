using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Encounter
{
    public class Encounter : EntitiesBase.Entity
    {
        public int Id { get; set; }
        public int EpisodeOfCareId { get; set; }
        public int Status { get; set; }
        public int Class { get; set; }
        public int Type { get; set; }
        public int ServiceType { get; set; }
        public PeriodDatetime Period { get; set; }
        public string PatientId { get; set; }

        public void ReplaceThesauruses(int oldThesaurus, int newThesaurus)
        {
            this.Status = this.Status == oldThesaurus ? newThesaurus : this.Status;
            this.Class = this.Class == oldThesaurus ? newThesaurus : this.Class;
            this.Type = this.Type == oldThesaurus ? newThesaurus : this.Type;
            this.ServiceType = this.ServiceType == oldThesaurus ? newThesaurus : this.ServiceType;
        }
    }
}
