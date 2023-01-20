using sReportsV2.Common.SmartOncologyEnums;
using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.ChemotherapySchemaInstance
{
    public class ChemotherapySchemaInstanceVersion : Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ChemotherapySchemaInstanceVersionId")]
        public int ChemotherapySchemaInstanceVersionId { get; set; }
        public int ChemotherapySchemaInstanceId { get; set; }
        [ForeignKey("CreatorId")]
        public virtual sReportsV2.Domain.Sql.Entities.User.User Creator { get; set; }
        public int CreatorId { get; set; }
        public int FirstDelayDay { get; set; }
        public int DelayFor { get; set; }
        public string ReasonForDelay { get; set; }
        public string Description { get; set; }
        public ChemotherapySchemaInstanceActionType ActionType {get; set;}
    }
}
