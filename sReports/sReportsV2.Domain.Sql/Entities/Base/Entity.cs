using sReportsV2.Domain.Sql.Entities.User;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.EntitiesBase
{
    public class Entity
    {
        public bool Active { get; set; }
        public bool IsDeleted { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public DateTime EntryDatetime { get; set; }
        public DateTime? LastUpdate { get; set; }
        [NotMapped]
        public DateTime StartDateTime { get; set; }
        [NotMapped]
        public DateTime EndDateTime { get; set; }
        public int? CreatedById { get; set; }  // Cannot rename it to UserId -> it would conflict with User Entity
        [ForeignKey("CreatedById")]
        public User CreatedBy { get; set; }


        public void SetLastUpdate()
        {
            this.LastUpdate = DateTime.Now;
        }

        public void SetCreatedById(int? createdById)
        {
            this.CreatedById = createdById;
        }

        private void SetEntryDatetime()
        {
            this.EntryDatetime = DateTime.Now;
        }

        public Entity()
        {
            SetEntryDatetime();
            Active = true;
        }
    }
}
