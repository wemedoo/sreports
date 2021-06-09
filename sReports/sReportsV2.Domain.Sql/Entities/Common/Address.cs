using sReportsV2.Domain.Sql.EntitiesBase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public class Address
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [StringLength(200)]
        public string Street { get; set; }
                
        public int? StreetNumber { get; set; }

        public Address() { }

        public Address(string city, string state, string postalCode, string country)
        {
            this.City = city;
            this.State = state;
            this.PostalCode = postalCode;
            this.Country = country;
        }

        public override string ToString()
        {
            return Country + "-" + PostalCode + " " + City;
        }

        public void SetAddress(Address address) 
        {
            if (address != null) 
            {
                this.City = address.City;
                this.State = address.State;
                this.PostalCode = address.PostalCode;
                this.Country = address.Country;
                this.Street = address.Street;
                this.StreetNumber = address.StreetNumber;
            }
        }
    }
}
