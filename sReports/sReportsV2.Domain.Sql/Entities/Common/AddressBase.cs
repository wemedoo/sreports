using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sReportsV2.Domain.Sql.Entities.Common
{
    public abstract class AddressBase : EntitiesBase.Entity
    {
        [StringLength(100)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(10)]
        public string PostalCode { get; set; }

        public int? CountryId { get; set; }
        [ForeignKey("CountryId")]
        public CustomEnum Country { get; set; }

        [StringLength(200)]
        public string Street { get; set; }

        public int? StreetNumber { get; set; }


        public int? AddressTypeId { get; set; }
        [ForeignKey("AddressTypeId")]
        public CustomEnum AddressType { get; set; }

        public void Copy(AddressBase address)
        {
            if (address != null)
            {
                this.City = address.City;
                this.State = address.State;
                this.PostalCode = address.PostalCode;
                this.Street = address.Street;
                this.StreetNumber = address.StreetNumber;
                this.AddressTypeId = address.AddressTypeId;
                this.CountryId = address.CountryId;
            }
        }
    }
}
