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

                this.Address.City = address.City;
                this.Address.Country = address.Country;
                this.Address.StreetNumber = address.StreetNumber;
                this.Address.PostalCode = address.PostalCode;
                this.Address.State = address.State;
                this.Address.StreetNumber = address.StreetNumber;
            }
        }

        private void SetTelecoms(List<Telecom> telecoms)
        {
            foreach (var telecom in telecoms.Where(x => x.Id == 0).ToList())
            {
                this.Telecoms.Add(telecom);
            }
        }
    }
}
