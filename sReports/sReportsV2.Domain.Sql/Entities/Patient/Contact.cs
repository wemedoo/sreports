using sReportsV2.Domain.Sql.Entities.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class Contact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        [Column("ContactId")]
        public int ContactId { get; set; }
        public string Relationship { get; set; }
        public Name Name { get; set; }
        [ForeignKey("AddressId")]
        public Address Address { get; set; }
        public string Gender { get; set; }
        public int? AddressId { get; set; }

        public List<Telecom> Telecoms { get; set; }

        public Contact()
        {
        }
        public Contact(string relationship, Name name, Address address, string gender, List<Telecom> telecoms)
        {
            this.Relationship = relationship;
            this.Name = name;
            this.Address = address;
            this.Gender = gender;
            this.Telecoms = telecoms;
        }

        public void Copy(Contact contact)
        {
            if(contact != null)
            {
                CopyAddress(contact.Address);
                SetTelecoms(contact.Telecoms);
                this.Gender = contact.Gender;
                this.Name.SetName(contact.Name);
                this.Relationship = contact.Relationship;
            }
        }

        private void CopyAddress(Address address)
        {
            if(address != null)
            {
                if (this.Address == null)
                {
                    this.Address = new Address();
                }

                this.Address.Copy(address);
            }
        }

        private void SetTelecoms(List<Telecom> telecoms)
        {
            foreach (var telecom in telecoms.Where(x => x.TelecomId == 0).ToList())
            {
                this.Telecoms.Add(telecom);
            }
        }
    }
}
