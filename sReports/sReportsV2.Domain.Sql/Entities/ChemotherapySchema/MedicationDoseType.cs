using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchema
{
    public class MedicationDoseType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("MedicationDoseTypeId")]
        public int MedicationDoseTypeId { get; set; }
        public string Type { get; set; }

        [NotMapped]
        public List<string> IntervalsList { get; set; }
        public string Intervals
        {
            get
            {
                return this.IntervalsList == null || !this.IntervalsList.Any()
                           ? null
                           : JsonConvert.SerializeObject(this.IntervalsList);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    this.IntervalsList = new List<string>();
                else
                    this.IntervalsList = JsonConvert.DeserializeObject<List<string>>(value);
            }

        }
    }
}
