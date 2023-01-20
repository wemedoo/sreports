using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.OutsideUser
{
    public class OutsideUser : EntitiesBase.Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("OutsideUserId")]
        public int OutsideUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Institution { get; set; }
        public string InstitutionAddress { get; set; }
        [ForeignKey("AddressId")]
        public Address Address { get; set; }
        public int? AddressId { get; set; }
    }
}
