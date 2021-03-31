using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Entities.PatientEntities
{
    public class Contact
    {
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
      
    }
}
