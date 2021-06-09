using sReportsV2.Domain.Sql.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Patient
{
    public class Contact
    {
        public int Id { get; set; }
        public string Relationship { get; set; }
        public Name Name { get; set; }
        public Address Address { get; set; }
        public string Gender { get; set; }
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

        public void SetId(Contact c) 
        {
            Id = c.Id;
        }

        public void SetAddress(Address address)
        {
            if (this.Address != null)
            {
                this.Address.SetAddress(address);
            }
            else
            {
                this.Address = address;
            }
        }

    }
}
