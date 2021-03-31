using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sReportsV2.DTOs.Common.DTO
{
    public class ContactDTO
    {
        public string Relationship { get; set; }
        public NameDTO Name { get; set; }
        public AddressDTO Address { get; set; }
        public string Gender { get; set; }
        public List<TelecomDTO> Telecoms { get; set; }

        public ContactDTO()
        {
        }
        public ContactDTO(string relationship, NameDTO name, AddressDTO address, string gender, List<TelecomDTO> telecoms)
        {
            this.Relationship = relationship;
            this.Name = name;
            this.Address = address;
            this.Gender = gender;
            this.Telecoms = telecoms;
        }
    }
}